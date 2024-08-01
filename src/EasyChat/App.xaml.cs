using EasyChat.Views;
using System.Windows;

namespace EasyChat;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 创建主窗口
        var loginView = new Login();
        loginView.Show();
    }
}