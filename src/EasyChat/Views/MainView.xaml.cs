using System.ComponentModel;
using System.Windows.Input;

namespace EasyChat.Views;

public partial class MainView
{
    public MainView()
    {
        InitializeComponent();
    }
    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }

    }
    protected override void OnClosing(CancelEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }
}