using MQTT_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            LoginButton.Visibility = Visibility.Visible;
        }
    }
}
