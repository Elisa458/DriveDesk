using System.Windows;
using DriveDesk.Data;
using DriveDesk.Infrastructure;
using DriveDesk.Services;

namespace DriveDesk.Views;

public class StudentRow
{
    public string FullName { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Group { get; init; } = "";
    public string Instructor { get; init; } = "";
}

/// <summary>Заглушка главного окна этапа 1: показывает, что права по ролям работают.</summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var user = Session.User!;
        var roleName = user.Role switch { Role.Admin => "Администратор", Role.Instructor => "Инструктор", _ => "Курсант" };
        UserInfo.Text = $"{user.Login} · {roleName}";
        Footer.Text = $"v0.1 · {DateTime.Now:dd.MM.yyyy} · роль: {roleName}";

        using var db = new AppDbContext();
        IQueryable<Student> q = db.Students;
        if (user.Role == Role.Student)
        {
            var id = user.StudentId;
            q = q.Where(s => s.Id == id);
            ListTitle.Text = "Моя карточка";
        }
        else if (user.Role == Role.Instructor)
        {
            var id = user.InstructorId;
            q = q.Where(s => s.InstructorId == id);
            ListTitle.Text = "Мои курсанты";
        }
        else ListTitle.Text = "Все курсанты";

        StudentsGrid.ItemsSource = q.OrderBy(s => s.FullName).Select(s => new StudentRow
        {
            FullName = s.FullName,
            Phone = s.Phone,
            Group = s.Group.Name,
            Instructor = s.Instructor != null ? s.Instructor.FullName : "—"
        }).ToList();
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        AuditService.Write(Session.User?.Id, "Logout", "User", "Выход");
        Session.User = null;
        new LoginWindow().Show();
        Close();
    }
}
