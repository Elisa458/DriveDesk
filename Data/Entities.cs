namespace DriveDesk.Data;

public enum Role { Student, Instructor, Admin }
public enum LessonType { Theory, Driving }
public enum LessonStatus { Planned, Done, Cancelled }
public enum ExamStage { InternalTheory, InternalDriving, GaiTheory, GaiTrack, GaiCity }

public class Category
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public int TheoryHours { get; set; }
    public int DrivingHours { get; set; }
}

public class StudyGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public DateTime StartDate { get; set; }
}

public class Vehicle
{
    public int Id { get; set; }
    public string Model { get; set; } = "";
    public string PlateNumber { get; set; } = "";
    public string Transmission { get; set; } = "";
}

public class Instructor
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public int? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
}

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public DateTime EnrollDate { get; set; }
    public DateTime MedicalValidUntil { get; set; }
    public int GroupId { get; set; }
    public StudyGroup Group { get; set; } = null!;
    public int? InstructorId { get; set; }
    public Instructor? Instructor { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();
    public Role Role { get; set; }
    public bool IsActive { get; set; } = true;
    public int FailedAttempts { get; set; }
    public DateTime? LockoutUntil { get; set; }
    public bool MustChangePassword { get; set; }
    public int? StudentId { get; set; }
    public Student? Student { get; set; }
    public int? InstructorId { get; set; }
    public Instructor? Instructor { get; set; }
}

public class Lesson
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public int? InstructorId { get; set; }
    public Instructor? Instructor { get; set; }
    public int? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public LessonType Type { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationMin { get; set; }
    public LessonStatus Status { get; set; }
    public string? Comment { get; set; }
}

public class Exam
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public ExamStage Stage { get; set; }
    public DateTime Date { get; set; }
    public int Score { get; set; }
    public bool Passed { get; set; }
    public int Attempt { get; set; }
}

public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = "";
    public string Entity { get; set; } = "";
    public string Details { get; set; } = "";
    public DateTime Timestamp { get; set; }
}
