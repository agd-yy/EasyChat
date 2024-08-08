using EasyChat.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace EasyChat.Views;

public partial class MainView
{
    public MainView()
    {
        InitializeComponent();
        txtMessage.KeyDown += TxtMessage_KeyDown;
    }
    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }

    }

    bool IsMaximized = false;
    private void Boreder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            if (IsMaximized)
            {
                this.WindowState = WindowState.Normal;
                this.Width = 1250;
                this.Height = 830;

                IsMaximized = false;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                IsMaximized = true;
            }
        }
    }
    protected override void OnClosing(CancelEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // 插入换行符
                txtMessage.Text += Environment.NewLine;
                txtMessage.CaretIndex = txtMessage.Text.Length;
                e.Handled = true; // 防止系统默认行为
                return;
            }
            if (DataContext is MainViewModel viewModel && viewModel.EnterCommand.CanExecute(null))
            {
                viewModel.EnterCommand.Execute(null);
            }
        }
    }
}