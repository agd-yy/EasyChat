using System.Windows;

namespace EasyChat.Views;

public partial class TrayView
{
    public TrayView()
    {
        InitializeComponent();
    }

    private void TrayView_OnActivated(object? sender, EventArgs e)
    {
        Hide();
        NotifyIcon.Visibility = Visibility.Visible;
    }
}