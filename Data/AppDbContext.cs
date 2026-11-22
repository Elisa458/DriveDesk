using DriveDesk.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DriveDesk.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<StudyGroup> Groups => Set<StudyGroup>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={AppPaths.DbPath}");

    // Все enum хранятся строками: БД читается глазами
    protected override void ConfigureConventions(ModelConfigurationBuilder cb)
        => cb.Properties<Enum>().HaveConversion<string>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Логин без учёта регистра + уникальность
        b.Entity<User>().Property(u => u.Login).UseCollation("NOCASE");
        b.Entity<User>().HasIndex(u => u.Login).IsUnique();
        b.Entity<User>().ToTable(t => t.HasCheckConstraint("CK_Users_Failed", "FailedAttempts >= 0"));

        b.Entity<Category>().HasIndex(c => c.Code).IsUnique();
        b.Entity<Category>().ToTable(t => t.HasCheckConstraint("CK_Categories_Hours", "TheoryHours > 0 AND DrivingHours > 0"));

        b.Entity<StudyGroup>().ToTable("Groups");
        b.Entity<StudyGroup>().HasIndex(g => g.Name).IsUnique();

        b.Entity<Vehicle>().HasIndex(v => v.PlateNumber).IsUnique();
        b.Entity<Instructor>().HasIndex(i => i.Phone).IsUnique();
        b.Entity<Student>().HasIndex(s => s.Phone).IsUnique();

        b.Entity<Lesson>().ToTable(t => t.HasCheckConstraint("CK_Lessons_Duration", "DurationMin BETWEEN 45 AND 120"));

        b.Entity<Exam>().HasIndex(e => new { e.StudentId, e.Stage, e.Attempt }).IsUnique();
        b.Entity<Exam>().ToTable(t => t.HasCheckConstraint("CK_Exams_Values", "Attempt >= 1 AND Score >= 0"));

        // Каскадное удаление запрещено везде: связанные записи удалить нельзя
        foreach (var fk in b.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
}
