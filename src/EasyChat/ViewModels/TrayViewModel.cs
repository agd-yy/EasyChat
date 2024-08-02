using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasyChat.ViewModels;

public partial class TrayViewModel : ObservableObject
{
    [RelayCommand]
    private void Open(Window window)
    {
        window.Show();
        window.Activate();
    }

    [RelayCommand]
    private void Hide(Window window)
    {
        window.Hide();
    }

    [RelayCommand]
    private void Exit()
    {
        try
        {
            Application.Current.Shutdown();
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
}