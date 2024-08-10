using EasyChat.ViewModels;
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
                WindowState = WindowState.Normal;
                IsMaximized = false;
            }
            else
            {
                WindowState = WindowState.Maximized;
                IsMaximized = true;
            }
        }
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
    private void Minimize(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    private void Maximize(object sender, RoutedEventArgs e)
    {
        if (IsMaximized)
        {
            WindowState = WindowState.Normal;
            IsMaximized = false;
        }
        else
        {
            WindowState = WindowState.Maximized;
            IsMaximized = true;
        }
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        Hide();
    }
}