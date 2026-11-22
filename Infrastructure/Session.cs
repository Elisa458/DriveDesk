using DriveDesk.Data;

namespace DriveDesk.Infrastructure;

/// <summary>Текущий вошедший пользователь.</summary>
public static class Session
{
    public static User? User { get; set; }
    public static bool IsAdmin => User?.Role == Role.Admin;
}
