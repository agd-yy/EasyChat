using System.Windows;
using System.Windows.Controls;

namespace EasyChat.view
{
    /// <summary>
    /// Login2.xaml 的交互逻辑
    /// </summary>
    public partial class Login2 : UserControl
    {
        public Login2()
        {
            InitializeComponent();
        }
        // 定义一个事件，用于通知主登录窗口
        public event RoutedEventHandler ButtonClicked;

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }
    }
}
