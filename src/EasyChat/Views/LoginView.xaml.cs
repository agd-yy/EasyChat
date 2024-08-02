using System.Windows.Input;
using EasyChat.Utilities;
using EasyChat.ViewModels;

namespace EasyChat.Views;

public partial class LoginView
{
    public LoginView()
    {
        InitializeComponent();
        DataContext = Singleton<LoginViewModel>.Instance;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed) DragMove();
    }
}