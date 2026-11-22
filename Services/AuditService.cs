using DriveDesk.Data;

namespace DriveDesk.Services;

public static class AuditService
{
    public static void Write(int? userId, string action, string entity, string details)
    {
        try
        {
            using var db = new AppDbContext();
            db.AuditLogs.Add(new AuditLog { UserId = userId, Action = action, Entity = entity, Details = details, Timestamp = DateTime.Now });
            db.SaveChanges();
        }
        catch { /* сбой журнала не должен ронять приложение */ }
    }
}
