using System.Collections.ObjectModel;
using System.ComponentModel;
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
    private ChatModel myChatModel;
    // dic< uid, chatModel>
    // private Dictionary<string, ChatModel> chatModelDic = new Dictionary<string, ChatModel>();

    private readonly LoginViewModel loginViewModel = Singleton<LoginViewModel>.Instance;

    public MainViewModel()
    {
        // 客户端名绑定界面
        var nickName = string.IsNullOrEmpty(loginViewModel.UserName) ? _myClient.MyClientUID : loginViewModel.UserName;
        myChatModel = new ChatModel
        {
            Uid = _myClient.MyClientUID,
            NickName = nickName,
            Message = "",
            TagName = "",
            MessageCount = 0,
            Image = MqttContent.GetRandomImg()
        };
        ChatUsers.Add(myChatModel);
        SubscribeUid = nickName;
        //启动客户端
        _myClient.StartClient(loginViewModel.IpAddr);

        _myClient.OnlinePersonEvent += ClientChangeOnlinePerson;
        _myClient.ReceiveMsgEvent += ClientChangeReceiveMsg;

        //ChatMessages.Add(new ChatMessage { Message = "Guys we have a plan to choose best way", Time = "4:15 PM", IsMyMessage = true });
        //{
        //    //new ChatMessage { SeparatorTitle = "Yesterday" },
        //    //new ChatMessage { NickName = "Name1", Image = "/Resources/Images/p11.jpg" },
        //    //new ChatMessage { Message = "Hello my friends", Time = "3:10 PM", Color = "#ff82a3" },
        //    //new ChatMessage { Message = "Hi Maud, Are you ok?", Time = "4:15 PM", IsMyMessage = true },
        //    new ChatMessage { Message = "Guys we have a plan to choose best way", Time = "4:15 PM", IsMyMessage = true },
        //    new ChatMessage { SeparatorTitle = "Today" },
        //    new ChatMessage { NickName = "Name2", Image = "/Resources/Images/p10.jpg" },
        //    new ChatMessage { Message = "Can you share your opinion?", Time = "6:39 PM", Color = "#c490ff" },
        //    new ChatMessage { NickName = "Name3", Image = "/Resources/Images/p12.jpg" },
        //    new ChatMessage { Message = "Yes Russell, just dont talk about it with others.", Time = "3:25 PM", Color = "#68cfa3" },
        //    new ChatMessage { Message = "Our plan have a new tactics ...", Time = "3:25 PM", Color = "#68cfa3" },
        //    new ChatMessage { Message = "I'm waiting for Maud for comeback to the chat", Time = "3:26 PM", IsMyMessage = true }
        //};
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
        ChatUsers.Clear();
        foreach (var chatModel in msgModel.ChatModels)
        {
            ChatUsers.Add(chatModel);
        }
    }

    /// <summary>
    ///     客户端修改页面信息
    /// </summary>
    /// <param name="needAdd"></param>
    private void ClientChangeReceiveMsg(List<ChatMessage> needAdd)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            foreach (ChatMessage msg in needAdd)
            {
                ChatMessages.Add(msg);
            }
        });
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
                            Uid = myChatModel.Uid,
                            SendTime = DateTime.Now,
                            Msg = SendMsg,
                            NickName = myChatModel.NickName,
                            Img = myChatModel.Image
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

                myChatModel.NickName = SubscribeUid;
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
            NickName = myChatModel.NickName,
            Img = myChatModel.Image
        });
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

    // 左侧用户
    public ObservableCollection<ChatModel> ChatUsers { get; } = new ObservableCollection<ChatModel>();

    // 用户对应聊天框
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
    ///     在线人员
    /// </summary>
    [ObservableProperty] private string _onlinePerson = string.Empty;

    /// <summary>
    ///     在线人员
    /// </summary>
    [ObservableProperty] private string _chat = string.Empty;

    #endregion
}