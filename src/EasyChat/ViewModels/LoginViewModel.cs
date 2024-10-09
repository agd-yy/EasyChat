using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Media3D;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyChat.Controls;
using EasyChat.Models;
using EasyChat.Service;
using EasyChat.Utilities;

namespace EasyChat.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private bool isServer;

    /// <summary>
    ///     IP
    /// </summary>
    [ObservableProperty] private string ipAddr = string.Empty;

    /// <summary>
    ///     密码
    /// </summary>
    [ObservableProperty] private string password = string.Empty;

    /// <summary>
    ///     用户名
    /// </summary>
    [ObservableProperty] private string userName = string.Empty;

    [ObservableProperty] private BindingList<string>? ipList;

    [RelayCommand]
    private void Server(Window window)
    {
        IsServer = true;
        if (window.GetAxr() is not { } axr)
            return;
        axr.BeginAnimation(AxisAngleRotation3D.AngleProperty,
            WindowUtilities.GetAnimation(180, TimeSpan.FromMilliseconds(500)));
        IpList = new BindingList<string>(NetworkUtilities.GetIps());
        IpAddr = IpList.FirstOrDefault()??"";
    }

    [RelayCommand]
    private void Client(Window window)
    {
        IsServer = false;
        if (window.GetAxr() is not { } axr)
            return;
        axr.BeginAnimation(AxisAngleRotation3D.AngleProperty,
            WindowUtilities.GetAnimation(180, TimeSpan.FromMilliseconds(500)));
        IpAddr = string.Empty;
    }

    [RelayCommand]
    private void Login(Window window)
    {
        var pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        if (string.IsNullOrEmpty(IpAddr) || !Regex.IsMatch(IpAddr, pattern))
        {
            EcMsgBox.Show("请输入有效的IP地址");
            return;
        }
        if (IsServer)
        {
            MqttService.CreateMqttService();
        }
        MqttContent.IPADDRESS = IpAddr;
        MqttContent.USER_NAME = UserName;
        MqttContent.PASSWORD = Password;
        window.DialogResult = true;
    }

    [RelayCommand]
    private void Back(Window window)
    {
        if (window.GetAxr() is not { } axr)
            return;

        axr.BeginAnimation(AxisAngleRotation3D.AngleProperty,
            WindowUtilities.GetAnimation(0, TimeSpan.FromMilliseconds(500)));
    }

    [RelayCommand]
    private void Minimize(Window window)
    {
        window.WindowState = WindowState.Minimized;
    }

    [RelayCommand]
    private void Close(Window window)
    {
        window.DialogResult = false;
    }
}