using System.Windows;
using EasyChat.Views;

namespace EasyChat;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 创建主窗口
        var loginView = new LoginView();
        if (loginView.ShowDialog() == true)
        {
            var mainWindow = new MainView();
            mainWindow.Show();
        }
        else
        {
            Shutdown();
        }
    }
}