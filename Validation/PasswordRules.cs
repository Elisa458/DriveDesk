namespace DriveDesk.Validation;

public static class PasswordRules
{
    /// <summary>Возвращает текст ошибки или null, если пароль подходит.</summary>
    public static string? Validate(string login, string password)
    {
        if (password.Length < 8) return "Пароль должен содержать не менее 8 символов";
        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit)) return "Пароль должен содержать букву и цифру";
        if (string.Equals(password, login, StringComparison.OrdinalIgnoreCase)) return "Пароль не должен совпадать с логином";
        return null;
    }
}
