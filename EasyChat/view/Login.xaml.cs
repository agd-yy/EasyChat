
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System;
using EasyChat.ViewModel;
using MQTT_Server;
using System.Text.RegularExpressions;

namespace EasyChat.view
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        // 页面绑定对象
        private LoginViewModel loginView;
        public Login()
        {
            InitializeComponent();
            loginView = new LoginViewModel();
            this.DataContext = loginView;
            Login1.DataContext = loginView;
            Login2.DataContext = loginView;
        }
        // 用来判断服务端是否为本地IP
        private string serviceIp;

        private void MyUserControl_ButtonClicked(object sender, RoutedEventArgs e)
        {
            // 附加登录控件的按钮点击事件
            if (e.OriginalSource is Button button)
            {
                switch (button.Name)
                {
                    case "MinimizeButton":
                        MinimizeButton_Click(sender, e);
                        break;
                    case "BackButton":
                        BackButton_Click(sender, e);
                        break;
                    case "CloseButton":
                        CloseButton_Click(sender, e);
                        break;
                    case "ServerButton":
                        ServerButton_Click(sender, e);
                        break;
                    case "ClientButton":
                        ClientButton_Click(sender, e);
                        break;
                    case "LoginButton":
                        LoginButton_Click(sender, e);
                        break;
                }
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                To = 0d
            };
            this.axr.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = loginView.UserName;
            string password = loginView.Password;
            string ip = loginView.IpAddr;
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
            // TODO  username password ip
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
            App._taskbar.Visibility = Visibility.Visible;
        }
        // 服务端按钮的处理逻辑
        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                To = 180d
            };
            this.axr.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

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
            loginView.IpAddr = ipAddress;
            Login2.ServerIpTextBox.IsEnabled = false;
        }

        // 客户端按钮的处理逻辑
        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            // 显示 IP 地址输入框和登录按钮
            DoubleAnimation da = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                To = 180d
            };
            this.axr.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
            loginView.IpAddr = string.Empty;
            Login2.ServerIpTextBox.IsEnabled = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 鼠标左键拖拽
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
