using System.Windows;

namespace EasyChat.Views;

public partial class Login2
{
    public Login2()
    {
        InitializeComponent();
    }

    // 定义一个事件，用于通知主登录窗口
    public event RoutedEventHandler ButtonClicked;

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonClicked?.Invoke(this, e);
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonClicked?.Invoke(this, e);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonClicked?.Invoke(this, e);
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonClicked?.Invoke(this, e);
    }
}