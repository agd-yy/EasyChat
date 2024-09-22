using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyChat.Controls;
using EasyChat.Handle;
using EasyChat.Models;
using EasyChat.Service;
using EasyChat.Utilities;
using EasyChat.ViewModels.SubVms;

namespace EasyChat.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly MyMqttClient _myClient = MyMqttClient.Instance;
    private readonly LoginViewModel _loginViewModel = Singleton<LoginViewModel>.Instance;
    private EventHelper _eventHelper = EventHelper.Instance;
    private int _allMessageCount = 0;
    // <uid , uid对应全部消息>
    private readonly Dictionary<string, List<ChatMessage>> _chatMessageDic = new Dictionary<string, List<ChatMessage>>();

    public MainViewModel()
    {
        // 客户端名绑定界面
        var nickName = string.IsNullOrEmpty(_loginViewModel.UserName) ? _myClient.MyClientUid : _loginViewModel.UserName;
        MyChatModel = new ChatModel
        {
            Uid = _myClient.MyClientUid,
            NickName = nickName,
            Image = MqttContent.GetRandomImg(),
            Message = "",
            TagName = "",
            MessageCount = 0
        };
        UserListVm.Users.Add(MyChatModel);
        //启动客户端
        _myClient.StartClient(_loginViewModel.IpAddr, MyChatModel);
        _myClient.OnlinePersonEvent += ClientChangeOnlinePerson;
        _myClient.ReceiveMsgEvent += ClientChangeReceiveMsg;
        // 用户选择回调
        UserListVm.OnSelected += UserSelect;

        _myClient.AddTopic(MqttContent.GROUP);
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
                    Uid = userModel.uid,
                    Image = userModel.image,
                    NickName = userModel.isOnline? userModel.nickName : userModel.nickName + MqttContent.OFFLINE_STRING,
                    IsOnline = userModel.isOnline,
                    IsGroup = userModel.isGroup
                });
                if (!_chatMessageDic.ContainsKey(userModel.uid))
                {
                    _chatMessageDic.Add(userModel.uid, new List<ChatMessage>());
                }
            }
            UserListVm.Users = new BindingList<ChatModel>(UserListVm.Users.OrderByDescending(m => m.Uid == MyChatModel.Uid).ThenBy(m => m.Uid).ToList());
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
            Image = string.IsNullOrEmpty(newMsg.userModel.image) ? MqttContent.GetRandomImg() : newMsg.userModel.image,
            Message = newMsg.message,
            Time = newMsg.sendTime.ToString(),
            IsMyMessage = newMsg.userModel.uid == MyChatModel.Uid
        };
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (newMsg.isGroupMsg)
            {
                DealGroupMessage(newMsg, chats);
            }
            else
            {
                DealPersonMessage(newMsg, chats);
            }
            
            if (!string.IsNullOrEmpty(ChatObj.Uid))
            {
                ChatMessages.Messages = new BindingList<ChatMessage>(_chatMessageDic[ChatObj.Uid]);
            }
        });
    }

    /// <summary>
    /// 处理群消息
    /// </summary>
    /// <param name="newMsg"></param>
    /// <param name="chats"></param>
    private void DealGroupMessage(MsgModel newMsg, ChatMessage chats)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            newMsg.groupName = newMsg.groupName == string.Empty ? "群聊" : newMsg.groupName;
            if (_chatMessageDic.ContainsKey(newMsg.groupName))
            {
                _chatMessageDic[newMsg.groupName].Add(chats);
                if (newMsg.groupName != ChatObj.Uid)
                {
                    UserListVm.Users.Where(x => x.Uid == newMsg.groupName).First().MessageCount++;
                    _allMessageCount++;
                    _eventHelper.StartBlink();
                }
            }
            else
            {
                _chatMessageDic.Add(newMsg.groupName, new List<ChatMessage> { chats });
                var chatmodel = new ChatModel
                {

                    Uid = newMsg.groupName,
                    NickName = newMsg.groupName,
                    Image = MqttContent.GetRandomImg(),
                    IsGroup = true,
                    Message = newMsg.message,
                    TagName = "",
                    MessageCount = 1
                };
                _allMessageCount++;
                UserListVm.Users.Add(chatmodel);
            }
            UserListVm.Users.Where(x => x.Uid == newMsg.groupName).First().Message = newMsg.message;
        });
    }
    
    /// <summary>
    /// 处理私发消息
    /// </summary>
    /// <param name="newMsg"></param>
    /// <param name="chats"></param>
    private void DealPersonMessage(MsgModel newMsg, ChatMessage chats)
    {
        if (_chatMessageDic.ContainsKey(newMsg.userModel.uid))
        {
            _chatMessageDic[newMsg.userModel.uid].Add(chats);
            if (newMsg.userModel.uid != ChatObj.Uid)
            {
                UserListVm.Users.Where(x => x.Uid == newMsg.userModel.uid).First().MessageCount++;
                _allMessageCount++;
                _eventHelper.StartBlink();
            }
        }
        else
        {
            _chatMessageDic.Add(newMsg.userModel.uid, new List<ChatMessage> { chats });
        }
        UserListVm.Users.Where(x => x.Uid == newMsg.userModel.uid).First().Message = newMsg.message;
    }

    private void UserSelect(ChatModel chatModel)
    {
        if (chatModel == null)
        {
            return;
        }
        try
        {
            SendTopic = chatModel.Uid;
            _allMessageCount -= chatModel.MessageCount;
            if(_allMessageCount == 0)
            {
                _eventHelper.StopBlink();
            }
            chatModel.MessageCount = 0;
            ChatObj = new ChatModel()
            {
                NickName = chatModel.NickName,
                Uid = chatModel.Uid,
                Image = chatModel.Image,
                IsGroup = chatModel.IsGroup,
                MessageCount = 0
            };
            ChatMessages.Messages = new BindingList<ChatMessage>(_chatMessageDic[chatModel.Uid]);
        }
        catch (Exception)
        {
        }
    }

    #region Commands
    [RelayCommand]
    private void Send()
    {
        try
        {
            if (!string.IsNullOrEmpty(SendMsg))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var isGroupMsg = string.IsNullOrEmpty(ChatObj.NickName) || ChatObj.IsGroup;
                    var topic = isGroupMsg ? MqttContent.GROUP : MqttContent.MESSAGE + SendTopic;
                    var msgModel = new MsgModel
                    {
                        userModel = new UserModel()
                        {
                            uid = MyChatModel.Uid,
                            nickName = MyChatModel.NickName,
                            image = MyChatModel.Image
                        },
                        sendTime = DateTime.Now,
                        message = SendMsg,
                        isGroupMsg = isGroupMsg
                    };
                    _myClient.SendMsg(topic, msgModel);
                    if (!isGroupMsg && !MyChatModel.Uid.Equals(SendTopic))
                    {
                        if (_chatMessageDic.ContainsKey(SendTopic))
                        {
                            _chatMessageDic[SendTopic].Add(new ChatMessage
                            {
                                NickName = MyChatModel.NickName,
                                Image = MyChatModel.Image,
                                Message = msgModel.message,
                                Time = msgModel.sendTime.ToString(),
                                IsMyMessage = true
                            });
                            UserListVm.Users.Where(x => x.Uid == SendTopic).First().MessageCount = 0;
                            UserListVm.Users.Where(x => x.Uid == SendTopic).First().Message = msgModel.message;
                            ChatMessages.Messages = new BindingList<ChatMessage>(_chatMessageDic[SendTopic]);
                        }
                    }
                    SendMsg = "";
                });
            }
        }
        catch (Exception ex)
        {
            EcMsgBox.Show(ex.Message);
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
    ///     当前聊天对象
    /// </summary>
    [ObservableProperty] private ChatModel _chatObj = new ChatModel();

    #endregion
}