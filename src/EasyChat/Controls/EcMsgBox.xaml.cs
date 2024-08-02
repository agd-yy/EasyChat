using System.Windows;
using System.Windows.Input;

namespace EasyChat.Controls;

public partial class EcMsgBox
{
    public EcMsgBox(string message)
    {
        InitializeComponent();
        MessageTextBlock.Text = message;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    public static void Show(string message)
    {
        var messageBox = new EcMsgBox(message);
        messageBox.ShowDialog();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed) DragMove();
    }
}