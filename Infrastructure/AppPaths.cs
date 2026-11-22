using System.IO;

namespace DriveDesk.Infrastructure;

public static class AppPaths
{
    public static readonly string DataDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DriveDesk");
    public static string DbPath => Path.Combine(DataDir, "drivedesk.db");
    public static string LogPath => Path.Combine(DataDir, "error.log");

    public static void Log(Exception ex)
    {
        try
        {
            Directory.CreateDirectory(DataDir);
            File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}\n\n");
        }
        catch { /* логирование не должно ронять приложение */ }
    }
}
