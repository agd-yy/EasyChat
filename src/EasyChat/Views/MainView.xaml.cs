using System.ComponentModel;

namespace EasyChat.Views;

public partial class MainView
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }
}