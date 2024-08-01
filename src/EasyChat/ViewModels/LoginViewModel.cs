using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyChat.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    /// <summary>
    ///     IP
    /// </summary>
    [ObservableProperty] private string _ipAddr = string.Empty;

    /// <summary>
    ///     密码
    /// </summary>
    [ObservableProperty] private string _password = string.Empty;

    /// <summary>
    ///     用户名
    /// </summary>
    [ObservableProperty] private string _userName = string.Empty;
}