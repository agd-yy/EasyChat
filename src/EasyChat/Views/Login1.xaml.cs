using System.Windows;

namespace EasyChat.Views;

public partial class Login1
{
    public Login1()
    {
        InitializeComponent();
    }

    // 定义一个事件，用于通知主登录窗口
    public event RoutedEventHandler ButtonClicked;

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonClicked?.Invoke(this, e);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonClicked?.Invoke(this, e);
    }

    private void ServerButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonClicked?.Invoke(this, e);
    }

    private void ClientButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonClicked?.Invoke(this, e);
    }
}