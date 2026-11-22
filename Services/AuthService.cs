using DriveDesk.Data;
using DriveDesk.Validation;

namespace DriveDesk.Services;

public record AuthResult(bool Success, string? Error, User? User);

public class AuthService
{
    private const int MaxAttempts = 5;
    private static readonly TimeSpan LockTime = TimeSpan.FromMinutes(5);
    private const string BadCreds = "Неверный логин или пароль";

    public AuthResult Login(string login, string password)
    {
        login = login.Trim();
        if (login.Length == 0 || password.Length == 0)
            return new(false, "Введите логин и пароль", null);

        using var db = new AppDbContext();
        var user = db.Users.FirstOrDefault(u => u.Login == login); // NOCASE: регистр не важен

        if (user == null)
        {
            AuditService.Write(null, "LoginFailed", "User", $"Неизвестный логин: {login}");
            return new(false, BadCreds, null);
        }
        if (!user.IsActive)
            return new(false, "Учётная запись заблокирована администратором", null);
        if (user.LockoutUntil is { } until && until > DateTime.Now)
            return new(false, $"Слишком много попыток. Повторите через {Math.Ceiling((until - DateTime.Now).TotalMinutes)} мин.", null);

        if (!PasswordHasher.Verify(password, user.PasswordHash, user.Salt))
        {
            user.FailedAttempts++;
            if (user.FailedAttempts >= MaxAttempts)
            {
                user.LockoutUntil = DateTime.Now + LockTime;
                user.FailedAttempts = 0;
            }
            db.SaveChanges();
            AuditService.Write(user.Id, "LoginFailed", "User", $"Неверный пароль: {login}");
            return new(false, BadCreds, null);
        }

        user.FailedAttempts = 0;
        user.LockoutUntil = null;
        db.SaveChanges();
        AuditService.Write(user.Id, "Login", "User", $"Вход: {login}");
        return new(true, null, user);
    }

    /// <summary>Возвращает текст ошибки или null при успехе.</summary>
    public string? ChangePassword(int userId, string newPassword)
    {
        using var db = new AppDbContext();
        var user = db.Users.Find(userId);
        if (user == null) return "Пользователь не найден";

        var error = PasswordRules.Validate(user.Login, newPassword);
        if (error != null) return error;

        (user.PasswordHash, user.Salt) = PasswordHasher.Hash(newPassword);
        user.MustChangePassword = false;
        db.SaveChanges();
        AuditService.Write(userId, "PasswordChanged", "User", $"Смена пароля: {user.Login}");
        return null;
    }
}
