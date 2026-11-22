using System.Windows;
using DriveDesk.Infrastructure;
using DriveDesk.Services;

namespace DriveDesk.Views;

public partial class ChangePasswordWindow : Window
{
    public ChangePasswordWindow() => InitializeComponent();

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (NewPwd.Password != RepeatPwd.Password)
        {
            Err.Text = "Пароли не совпадают";
            return;
        }
        var error = new AuthService().ChangePassword(Session.User!.Id, NewPwd.Password);
        if (error != null)
        {
            Err.Text = error;
            return;
        }
        Session.User.MustChangePassword = false;
        DialogResult = true;
    }
}
