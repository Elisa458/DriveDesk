using System.Windows;
using DriveDesk.Infrastructure;
using DriveDesk.ViewModels;

namespace DriveDesk.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _vm = new();

    public LoginWindow()
    {
        InitializeComponent();
        DataContext = _vm;
        Pwd.PasswordChanged += (_, _) => _vm.Password = Pwd.Password;
        _vm.LoggedIn += OnLoggedIn;
    }

    private void OnLoggedIn()
    {
        // Курсант/инструктор с временным паролем обязан его сменить
        if (Session.User!.MustChangePassword)
        {
            var dlg = new ChangePasswordWindow { Owner = this };
            if (dlg.ShowDialog() != true)
            {
                Session.User = null;
                _vm.ErrorText = "Для продолжения необходимо сменить пароль";
                return;
            }
        }
        new MainWindow().Show();
        Close();
    }
}
