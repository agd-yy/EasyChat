using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyChat.Controls;
using EasyChat.Models;
using EasyChat.Service;
using EasyChat.Utilities;

namespace EasyChat.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly MyMqttClient _myClient = MyMqttClient.Instance;

    private readonly LoginViewModel loginViewModel = Singleton<LoginViewModel>.Instance;
    // 客户端的昵称
    private string _nickName;

    public MainViewModel()
    {
        // 客户端名绑定界面
        _nickName = string.IsNullOrEmpty(loginViewModel.UserName) ? _myClient.MyClientUID : loginViewModel.UserName;
        ChatModels.Add(new ChatModel {Uid = _myClient.MyClientUID, NickName = _nickName, 
            Message = "", TagName = "NI", MessageCount = 0, Image = "/Resources/Images/p1.jpg" });
        SubscribeUid = _nickName;
        //启动客户端
        _myClient.StartClient(loginViewModel.IpAddr);

        _myClient.OnlinePersonEvent += ClientChangeOnlinePerson;
        _myClient.ReceiveMsgEvent += ClientChangeReceiveMsg;

        ChatMessages = new ObservableCollection<ChatMessage>
        {
            new ChatMessage { SeparatorTitle = "Yesterday" },
            new ChatMessage { NickName = "Name1", Image = "/Resources/Images/p11.jpg" },
            new ChatMessage { Message = "Hello my friends", Time = "3:10 PM", Color = "#ff82a3" },
            new ChatMessage { Message = "Hi Maud, Are you ok?", Time = "4:15 PM", IsMyMessage = true },
            new ChatMessage { Message = "Guys we have a plan to choose best way", Time = "4:15 PM", IsMyMessage = true },
            new ChatMessage { SeparatorTitle = "Today" },
            new ChatMessage { NickName = "Name2", Image = "/Resources/Images/p10.jpg" },
            new ChatMessage { Message = "Can you share your opinion?", Time = "6:39 PM", Color = "#c490ff" },
            new ChatMessage { NickName = "Name3", Image = "/Resources/Images/p12.jpg" },
            new ChatMessage { Message = "Yes Russell, just dont talk about it with others.", Time = "3:25 PM", Color = "#68cfa3" },
            new ChatMessage { Message = "Our plan have a new tactics ...", Time = "3:25 PM", Color = "#68cfa3" },
            new ChatMessage { Message = "I'm waiting for Maud for comeback to the chat", Time = "3:26 PM", IsMyMessage = true }
        };
    }

    /// <summary>
    ///     客户端修改页面在线用户
    /// </summary>
    /// <param name="msgModel"></param>
    private void ClientChangeOnlinePerson(MsgModel msgModel)
    {
        if (msgModel == null)
        {
            return;
        }
        OnlinePerson = string.Join(Environment.NewLine, msgModel.Uids);
    }

    /// <summary>
    ///     客户端修改页面信息
    /// </summary>
    /// <param name="needAdd"></param>
    private void ClientChangeReceiveMsg(string needAdd)
    {
        ReceiveMsg += needAdd;
    }

    #region Commands
    [RelayCommand]
    private void Send()
    {
        try
        {
            if (!string.IsNullOrEmpty(SendMsg) /* || !string.IsNullOrEmpty(SendTopic)*/)
                Task.Run(() =>
                {
                    _myClient.sendMsg(MqttContent.MESSAGE + SendTopic,
                        new MsgModel
                        {
                            Uid = _myClient.MyClientUID,
                            SendTime = DateTime.Now,
                            Msg = SendMsg,
                            NickName = _nickName
                        });
                    SendMsg = "";
                });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private void Subscribe()
    {
        try
        {
            Task.Run(() =>
            {
                // 重命名自己 --注意规避特殊字符
                if (string.IsNullOrEmpty(SubscribeUid) || SubscribeUid.Contains(MqttContent.SUB_STRING))
                {
                    MessageBox.Show("昵称不合规", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _nickName = SubscribeUid;
                MessageBox.Show("昵称修改成功");
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private void Online()
    {
        // 先清空当前在线客户端的信息
        OnlinePerson = "";
        // 询问在线的机子
        _myClient.sendMsg(MqttContent.WHO_ONLINE, new MsgModel
        {
            Uid = _myClient.MyClientUID,
            SendTime = DateTime.Now,
            NickName = _nickName
        });
    }

    [RelayCommand]
    private void ClearText()
    {
        ReceiveMsg = "";
    }

    [RelayCommand]
    private void Enter()
    {
        Send();
    }

    [RelayCommand]
    private void Nothing()
    {
        EcMsgBox.Show("这个功能还没做");
    }
    #endregion

    #region Property

    public ObservableCollection<ChatModel> ChatModels { get; } = new ObservableCollection<ChatModel>();


    public ObservableCollection<ChatMessage> ChatMessages { get; set; } = new ObservableCollection<ChatMessage>();

    /// <summary>
    ///     发送信息
    /// </summary>
    [ObservableProperty] private string _sendMsg = string.Empty;

    /// <summary>
    ///     发送主题
    /// </summary>
    [ObservableProperty] private string _sendTopic = string.Empty;


    /// <summary>
    ///     用户名
    /// </summary>
    [ObservableProperty] private string _subscribeUid = string.Empty;


    /// <summary>
    ///     接收消息
    /// </summary>
    [ObservableProperty] private string _receiveMsg = string.Empty;

    /// <summary>
    ///     在线人员
    /// </summary>
    [ObservableProperty] private string _onlinePerson = string.Empty;

    /// <summary>
    ///     在线人员
    /// </summary>
    [ObservableProperty] private string _chat = string.Empty;

    #endregion
}