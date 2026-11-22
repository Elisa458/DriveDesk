using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriveDesk.Infrastructure;
using DriveDesk.Services;

namespace DriveDesk.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _auth = new();

    [ObservableProperty] private string _login = "";
    [ObservableProperty] private string _errorText = "";

    // PasswordBox не поддерживает привязку, пароль передаётся из code-behind
    public string Password { get; set; } = "";

    public event Action? LoggedIn;

    [RelayCommand]
    private void Submit()
    {
        var result = _auth.Login(Login, Password);
        if (!result.Success)
        {
            ErrorText = result.Error ?? "";
            return;
        }
        Session.User = result.User;
        ErrorText = "";
        LoggedIn?.Invoke();
    }
}
