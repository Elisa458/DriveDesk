namespace DriveDesk.Data;

public static class Seeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return;

        var rng = new Random(42);
        var today = DateTime.Today;

        string[] sur = { "Иванов", "Петров", "Сидоров", "Кузнецов", "Ковалёв", "Новиков", "Морозов", "Волков", "Лебедев", "Соколов" };
        string[] nm = { "Алексей", "Дмитрий", "Максим", "Андрей", "Сергей", "Артём", "Иван", "Никита", "Егор", "Павел" };
        string[] pat = { "Александрович", "Игоревич", "Сергеевич", "Андреевич", "Викторович", "Николаевич", "Олегович", "Петрович", "Михайлович", "Юрьевич" };
        string[] codes = { "29", "33", "44", "25" };
        string Name(int i) => $"{sur[i % 10]} {nm[(i * 3 + 1) % 10]} {pat[(i * 7 + 2) % 10]}";
        string Phone(int baseN, int i) => $"+375{codes[i % 4]}{baseN + i * 137}";

        // ВНИМАНИЕ: часы ориентировочные, перед защитой сверить с программой подготовки
        var cats = new[]
        {
            new Category { Code = "A", TheoryHours = 20, DrivingHours = 18 },
            new Category { Code = "B", TheoryHours = 30, DrivingHours = 36 },
            new Category { Code = "C", TheoryHours = 30, DrivingHours = 40 },
            new Category { Code = "D", TheoryHours = 30, DrivingHours = 40 },
        };

        string[] models = { "Lada Granta", "Renault Logan", "Skoda Rapid", "Kia Rio", "VW Polo", "Hyundai Solaris" };
        string[] letters = { "AB", "KM", "EH", "PT", "XO", "MM" };
        var vehicles = Enumerable.Range(0, 6).Select(i => new Vehicle
        {
            Model = models[i],
            PlateNumber = $"{1100 + i * 111} {letters[i]}-7",
            Transmission = i % 3 == 2 ? "АКПП" : "МКПП"
        }).ToList();

        var instructors = Enumerable.Range(0, 6).Select(i => new Instructor
        {
            FullName = Name(100 + i),
            Phone = Phone(2000000, i),
            Vehicle = vehicles[i]
        }).ToList();

        var groups = new[]
        {
            new StudyGroup { Name = "B-26-01", Category = cats[1], StartDate = today.AddDays(-80) },
            new StudyGroup { Name = "B-26-02", Category = cats[1], StartDate = today.AddDays(-60) },
            new StudyGroup { Name = "B-26-03", Category = cats[1], StartDate = today.AddDays(-40) },
            new StudyGroup { Name = "A-26-01", Category = cats[0], StartDate = today.AddDays(-30) },
        };

        var students = Enumerable.Range(0, 30).Select(i => new Student
        {
            FullName = Name(i),
            Phone = Phone(3000000, i),
            BirthDate = today.AddYears(-(18 + rng.Next(0, 15))).AddDays(-rng.Next(365)),
            Group = groups[i % 4],
            Instructor = instructors[i % 6],
            EnrollDate = groups[i % 4].StartDate,
            MedicalValidUntil = today.AddMonths(rng.Next(-1, 13))
        }).ToList();

        var users = new List<User> { MakeUser("admin", "Admin123", Role.Admin, false) };
        for (int i = 0; i < 6; i++)
        {
            var u = MakeUser($"instr{i + 1:00}", "Instr1234", Role.Instructor, true);
            u.Instructor = instructors[i];
            users.Add(u);
        }
        for (int i = 0; i < 10; i++)
        {
            var u = MakeUser($"student{i + 1:00}", "Student123", Role.Student, true);
            u.Student = students[i];
            users.Add(u);
        }

        var lessons = new List<Lesson>();
        for (int k = 0; k < 40; k++)
        {
            var s = students[k % 30];
            bool theory = k % 4 == 0;
            var start = today.AddDays(-10 + k / 2).AddHours(9 + k % 9);
            lessons.Add(new Lesson
            {
                Student = s,
                Instructor = s.Instructor,
                Vehicle = theory ? null : s.Instructor!.Vehicle,
                Type = theory ? LessonType.Theory : LessonType.Driving,
                StartTime = start,
                DurationMin = theory ? 90 : (k % 3 == 0 ? 90 : 60),
                Status = start < DateTime.Now ? (k % 11 == 0 ? LessonStatus.Cancelled : LessonStatus.Done) : LessonStatus.Planned
            });
        }

        var exams = new List<Exam>();
        for (int i = 0; i < 12; i++)
        {
            int score = 12 + rng.Next(0, 9);
            bool ok = score >= 18;
            exams.Add(new Exam { Student = students[i], Stage = ExamStage.InternalTheory, Date = today.AddDays(-rng.Next(5, 30)), Score = score, Passed = ok, Attempt = 1 });
            if (!ok)
                exams.Add(new Exam { Student = students[i], Stage = ExamStage.InternalTheory, Date = today.AddDays(-rng.Next(1, 4)), Score = 19, Passed = true, Attempt = 2 });
        }
        for (int i = 0; i < 3; i++)
            exams.Add(new Exam { Student = students[i], Stage = ExamStage.GaiTheory, Date = today.AddDays(-rng.Next(1, 5)), Score = 18 + i, Passed = true, Attempt = 1 });

        db.Categories.AddRange(cats);
        db.Groups.AddRange(groups);
        db.Vehicles.AddRange(vehicles);
        db.Instructors.AddRange(instructors);
        db.Students.AddRange(students);
        db.Users.AddRange(users);
        db.Lessons.AddRange(lessons);
        db.Exams.AddRange(exams);
        db.AuditLogs.Add(new AuditLog { Action = "Init", Entity = "Database", Details = "Создана БД и тестовые данные", Timestamp = DateTime.Now });
        db.SaveChanges();
    }

    private static User MakeUser(string login, string password, Role role, bool mustChange)
    {
        var (hash, salt) = Services.PasswordHasher.Hash(password);
        return new User { Login = login, PasswordHash = hash, Salt = salt, Role = role, MustChangePassword = mustChange };
    }
}
