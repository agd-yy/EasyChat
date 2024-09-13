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
    // <uid , uid对应全部消息>
    private readonly Dictionary<string, List<ChatMessage>> _chatMessageDic = new Dictionary<string, List<ChatMessage>>();

    public MainViewModel()
    {
        // 客户端名绑定界面
        var nickName = string.IsNullOrEmpty(_loginViewModel.UserName) ? _myClient.MyClientUid : _loginViewModel.UserName;
        MyChatModel = new ChatModel
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
        UserListVm.Users.Add(MyChatModel);
        //启动客户端
        _myClient.StartClient(_loginViewModel.IpAddr, MyChatModel.UserModel);
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
        ChatMessage chats = new ChatMessage
        {
            NickName = newMsg.userModel.nickName,
            Image = newMsg.userModel.image,
            Message = newMsg.message,
            Time = newMsg.sendTime.ToString(),
            IsMyMessage = newMsg.userModel.uid == MyChatModel.UserModel.uid
        };
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (newMsg.isGroupMsg)
            {
                newMsg.groupName = newMsg.groupName == string.Empty ? "群聊" : newMsg.groupName;
                if (_chatMessageDic.ContainsKey(newMsg.groupName))
                {
                    _chatMessageDic[newMsg.groupName].Add(chats);
                    UserListVm.Users.Where(x => x.UserModel.uid == newMsg.groupName).First().MessageCount++;
                }
                else
                {
                    _chatMessageDic.Add(newMsg.groupName, new List<ChatMessage> { chats});
                    var chatmodel = new ChatModel
                    {
                        UserModel = new UserModel()
                        {
                            uid = newMsg.groupName,
                            nickName = newMsg.groupName,
                            image = MqttContent.GetRandomImg(),
                            isGroup = true
                        },
                        Message = newMsg.message,
                        TagName = "",
                        MessageCount = 1
                    };
                    UserListVm.Users.Add(chatmodel);
                }
            }
            else
            {
                if (_chatMessageDic.ContainsKey(newMsg.userModel.uid))
                {
                    _chatMessageDic[newMsg.userModel.uid].Add(chats);
                    UserListVm.Users.Where(x => x.UserModel.uid == newMsg.userModel.uid).First().MessageCount++;
                }
                else
                {
                    _chatMessageDic.Add(newMsg.userModel.uid, new List<ChatMessage> { chats });
                }
            }
            if (!string.IsNullOrEmpty(ChatObj.UserModel.nickName))
            {       
                ChatMessages.Messages = new BindingList<ChatMessage>(_chatMessageDic[ChatObj.UserModel.nickName]);
            }
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
        ChatObj.UserModel.nickName = chatModel.UserModel.nickName;
        ChatMessages.Messages = new BindingList<ChatMessage>(_chatMessageDic[chatModel.UserModel.uid]);
    }
    #region Commands
    [RelayCommand]
    private void Send()
    {
        try
        {
            if (!string.IsNullOrEmpty(SendMsg))
                Task.Run(() =>
                {
                    _myClient.SendMsg(MqttContent.MESSAGE + SendTopic,
                        new MsgModel
                        {
                            userModel = MyChatModel.UserModel,
                            sendTime = DateTime.Now,
                            message = SendMsg,
                            isGroupMsg = string.IsNullOrEmpty(ChatObj.UserModel.nickName) || ChatObj.UserModel.isGroup
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
    public MessageListVm ChatMessages { get; set; } = new();

    /// <summary>
    ///     发送信息
    /// </summary>
    [ObservableProperty] private string _sendMsg = string.Empty;

    /// <summary>
    ///     发送主题
    /// </summary>
    [ObservableProperty] private string _sendTopic = string.Empty;


    /// <summary>
    ///     用户
    /// </summary>
    [ObservableProperty] private ChatModel _myChatModel = new ChatModel();
    /// <summary>
    ///     当前聊天对象的昵称
    /// </summary>
    [ObservableProperty] private ChatModel _chatObj = new ChatModel();

    #endregion
}