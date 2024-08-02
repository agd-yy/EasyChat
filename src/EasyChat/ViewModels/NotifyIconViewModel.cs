using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyChat.Views;

namespace EasyChat.ViewModels;

/// <summary>
///     图标相关
/// </summary>
public partial class NotifyIconViewModel : ObservableObject
{
    /// <summary>
    ///     如果窗口没显示，就显示窗口
    /// </summary>
    [RelayCommand]
    public void ShowWindow()
    {
        MainView? old = null;
        foreach (var w in Application.Current.Windows)
            if (w is MainView mw)
                old = mw;
        if (old == null) Application.Current.MainWindow = old = new MainView();
        old.Show();
    }

    /// <summary>
    ///     隐藏窗口
    /// </summary>
    [RelayCommand]
    public void HideWindow()
    {
        Application.Current.MainWindow?.Hide();
    }


    /// <summary>
    ///     关闭软件
    /// </summary>
    [RelayCommand]
    public void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}