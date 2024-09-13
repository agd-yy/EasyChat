using System.Windows;
using System.Windows.Threading;
using EasyChat.Views;

namespace EasyChat;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        #region 异常处理
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        #endregion
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

    #region 异常处理
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;
        var msg = $"UI线程异常\n{ex.Message}\n{ex.StackTrace}";
        MessageBox.Show(msg, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception ex) return;
        var msg = $"非UI线程异常\n{ex.Message}\n{ex.StackTrace}";
        MessageBox.Show(msg, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var ex = e.Exception;
        var msg = $"Task异常{ex.Message}\n\n{ex.StackTrace}";
        MessageBox.Show(msg, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
    }
    #endregion
}