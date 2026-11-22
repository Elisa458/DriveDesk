using System.IO;
using System.Windows;
using DriveDesk.Data;
using DriveDesk.Infrastructure;
using DriveDesk.Views;

namespace DriveDesk;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Глобальный обработчик: пользователь видит нейтральное сообщение, детали уходят в лог
        DispatcherUnhandledException += (_, a) =>
        {
            AppPaths.Log(a.Exception);
            MessageBox.Show("Произошла непредвиденная ошибка. Подробности записаны в журнал.",
                "DriveDesk", MessageBoxButton.OK, MessageBoxImage.Error);
            a.Handled = true;
        };

        try
        {
            Directory.CreateDirectory(AppPaths.DataDir);
            using var db = new AppDbContext();
            db.Database.EnsureCreated();   // автосоздание БД при первом запуске
            Seeder.Seed(db);               // тестовые данные, если БД пустая
        }
        catch (Exception ex)
        {
            AppPaths.Log(ex);
            MessageBox.Show("Не удалось открыть базу данных.", "DriveDesk",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        new LoginWindow().Show();
    }
}
