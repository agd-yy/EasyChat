using MQTT_Server;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasyChat.view
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        private string serviceIp;

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ServerIpTextBlock.Visibility = Visibility.Collapsed;
            ServerIpTextBox.Visibility = Visibility.Collapsed;
            LoginButton.Visibility = Visibility.Collapsed;
            ServerIpTextBox.IsEnabled = true;
            ServerIpTextBox.Text = "";

            UsernameTextBox.Visibility = Visibility.Visible;
            UsernameTextBlock.Visibility = Visibility.Visible;
            PasswordTextBlock.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Visible;
            ServerButton.Visibility = Visibility.Visible;
            ClientButton.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Collapsed;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string ip = ServerIpTextBox.Text;
            string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            if (string.IsNullOrEmpty(ip) || !Regex.IsMatch(ip, pattern))
            {
                MyMsgBox.Show("请输入有效的IP地址");
                return;
            }
            if (ip.Equals(serviceIp))
            {
                MqttService.CreateMqttService();
            }
            // Here you would typically check the username and password
            // For simplicity, we assume the login is always successful

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
            App._taskbar.Visibility = Visibility.Visible;
        }
        // 服务端按钮的处理逻辑
        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {

            // 显示 IP 地址输入框和登录按钮
            ServerIpTextBlock.Visibility = Visibility.Visible;
            ServerIpTextBox.Visibility = Visibility.Visible;
            ServerIpTextBox.IsEnabled = false;
            LoginButton.Visibility = Visibility.Visible;

            UsernameTextBox.Visibility = Visibility.Collapsed;
            UsernameTextBlock.Visibility = Visibility.Collapsed;
            PasswordTextBlock.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Collapsed;
            ServerButton.Visibility = Visibility.Collapsed;
            ClientButton.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Visible;

            string ipAddress = string.Empty;
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                    break;
                }
            }
            serviceIp = ipAddress;
            ServerIpTextBox.Text = ipAddress;
        }

        // 客户端按钮的处理逻辑
        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            // 显示 IP 地址输入框和登录按钮
            ServerIpTextBlock.Visibility = Visibility.Visible;
            ServerIpTextBox.Visibility = Visibility.Visible;
            ServerIpTextBox.IsEnabled = true;
            LoginButton.Visibility = Visibility.Visible;

            UsernameTextBox.Visibility = Visibility.Collapsed;
            UsernameTextBlock.Visibility = Visibility.Collapsed;
            PasswordTextBlock.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Collapsed;
            ServerButton.Visibility = Visibility.Collapsed;
            ClientButton.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Visible;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
