using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasyChat.ViewModels;

public partial class TrayViewModel : ObservableObject
{
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