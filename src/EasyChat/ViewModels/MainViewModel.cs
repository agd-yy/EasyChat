using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyChat.Controls;
using EasyChat.Models;
using EasyChat.Service;
using EasyChat.Utilities;
using EasyChat.ViewModels.SubVms;

namespace EasyChat.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly MyMqttClient _myClient = MyMqttClient.Instance;
    private readonly LoginViewModel _loginViewModel = Singleton<LoginViewModel>.Instance;
    private readonly ChatModel _myChatModel = new ChatModel();
    private readonly Dictionary<string, ChatModel> _chatModelDic = new Dictionary<string, ChatModel>();

    public MainViewModel()
    {
        // 客户端名绑定界面
        var nickName = string.IsNullOrEmpty(_loginViewModel.UserName) ? _myClient.MyClientUid : _loginViewModel.UserName;
        _myChatModel = new ChatModel
        {
            UserModel = new UserModel() {
                uid = _myClient.MyClientUid,
                nickName = nickName,
                image = MqttContent.GetRandomImg()
            },
            Message = "",
            TagName = "",
            MessageCount = 0
        };
        UserListVm.Users.Add(_myChatModel);
        UserOrGroupCombine();
        SubscribeUid = nickName;
        //启动客户端
        _myClient.StartClient(_loginViewModel.IpAddr, _myChatModel.UserModel);

        _myClient.OnlinePersonEvent += ClientChangeOnlinePerson;
        _myClient.ReceiveMsgEvent += ClientChangeReceiveMsg;
        // 用户选择回调
        UserListVm.OnSelected += UserSelect;

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
        Application.Current.Dispatcher.Invoke(() =>
        {
            UserListVm.Users.Clear();
            foreach (var userModel in msgModel.userModels)
            {
                UserListVm.Users.Add(new ChatModel()
                {
                    GroupName = userModel.nickName,
                    UserModel = userModel
                });
            }
            UserOrGroupCombine();
        });
    }

    /// <summary>
    ///  客户端接受消息
    /// </summary>
    /// <param name="newMsg"></param>
    private void ClientChangeReceiveMsg(MsgModel newMsg)
    {
        // 先取这个消息是谁的，加到对应的消息里，
        // 更新最新一条消息，如果当前选中的对话框不是这个用户，则未读消息++
        if(newMsg.isGroupMsg)
        {
            //List<ChatMessage> list = new List<ChatMessage>();
            //list.Add(new ChatMessage { nickName = msgModel.userModel.nickName, image = msgModel.userModel.image });
            //list.Add(new ChatMessage { message = msgModel.msg, time = msgModel.sendTime.ToString(),
            //isMyMessage = msgModel.userModel.uid == MyClientUid});
        }
        else
        {
            //List<ChatMessage> list = new List<ChatMessage>();
            //list.Add(new ChatMessage { nickName = msgModel.userModel.nickName, image = msgModel.userModel.image });
            //list.Add(new ChatMessage { message = msgModel.msg, time = msgModel.sendTime.ToString() });
        }
        Application.Current.Dispatcher.Invoke(() =>
        {
            //foreach (ChatMessage msg in needAdd)
            //{
            //    ChatMessages.Add(msg);
            //}
        });
    }

    /// <summary>
    /// 将UserListVm.Users 和内存的消息合并
    /// 如果_chatModelDic=存在UserListVm.Users没有的用户，那么就是用户下线
    /// </summary>
    private void UserOrGroupCombine()
    {
        

    }

    private void UserSelect(ChatModel chatModel)
    {

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
                    _myClient.SendMsg(MqttContent.MESSAGE + SendTopic,
                        new MsgModel
                        {
                            userModel = _myChatModel.UserModel,
                            sendTime = DateTime.Now,
                            message = SendMsg
                            // 这里确认是组消息还是私消息 TODO
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
                    EcMsgBox.Show("昵称不合规");
                    return;
                }

                _myChatModel.UserModel.nickName = SubscribeUid;
                EcMsgBox.Show("昵称修改成功");
            });
        }
        catch (Exception ex)
        {
            EcMsgBox.Show(ex.Message);
        }
    }

    [RelayCommand]
    private void Online()
    {
        // 先清空当前在线客户端的信息
        OnlinePerson = "";
        // 询问在线的机子
        _myClient.SendMsg(MqttContent.WHO_ONLINE, new MsgModel
        {
            userModel = new UserModel() {
                uid = _myClient.MyClientUid,
                nickName = _myChatModel.UserModel.nickName,
                image = _myChatModel.UserModel.image
            },
            sendTime = DateTime.Now
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
    public UserListVm UserListVm { get; } = new();

    // 用户对应聊天框
    public BindingList<ChatMessage> ChatMessages { get; set; } = [];

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