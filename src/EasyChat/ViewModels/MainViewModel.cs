using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyChat.Controls;
using EasyChat.Handle;
using EasyChat.Models;
using EasyChat.Service;
using EasyChat.ViewModels.SubVms;
using System.IO;

namespace EasyChat.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly MyMqttClient _myClient = MyMqttClient.Instance;
    private EventHelper _eventHelper = EventHelper.Instance;
    private string _sendTopic = string.Empty;
    // 新消息总数
    private int _allNewMessageCount = 0;
    // <uid , uid对应全部消息>
    private readonly Dictionary<string, List<ChatMessage>> _chatMessageDic = new Dictionary<string, List<ChatMessage>>();

    public MainViewModel()
    {
        // 客户端名绑定界面
        var nickName = string.IsNullOrEmpty(MqttContent.USER_NAME) ? _myClient.MyClientUid : MqttContent.USER_NAME;
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
        _myClient.StartClient(MqttContent.IPADDRESS, MyChatModel);
        _myClient.OnlinePersonEvent += ClientChangeOnlinePerson;
        _myClient.ReceiveMsgEvent += ClientChangeReceiveMsg;
        // 用户选择回调
        UserListVm.OnSelected += UserSelect;
        UserListVm.RightClicked += Nothing;

        _myClient.AddTopic(MqttContent.GROUP);
        GC.Collect();
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
    /// 处理群消息 需要增加新建群聊的功能
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
                    _allNewMessageCount++;
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
                _allNewMessageCount++;
                _eventHelper.StartBlink();
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
                _allNewMessageCount++;
                _eventHelper.StartBlink();
            }
        }
        else
        {
            _chatMessageDic.Add(newMsg.userModel.uid, new List<ChatMessage> { chats });
        }
        UserListVm.Users.Where(x => x.Uid == newMsg.userModel.uid).First().Message = newMsg.message;
    }

    private void DealReceiveImageOrFile(MsgModel newMsg)
    {
        string base64String = newMsg.message;

        // 1. 将 Base64 字符串转换为二进制文件
        byte[] fileBytes = Convert.FromBase64String(base64String);

    }

    private void DealSendImageOrFile(string topic, bool isGroupMsg)
    {
        byte[] fileBytes = File.ReadAllBytes("path/to/file");
        int chunkSize = 1024 * 1024; // 每个分片的大小 (1 MB)
        int totalChunks = (fileBytes.Length + chunkSize - 1) / chunkSize;
        if (totalChunks > 100)
        {
            return;
        }
        for (int i = 0; i < totalChunks; i++)
        {
            int offset = i * chunkSize;
            int length = Math.Min(chunkSize, fileBytes.Length - offset);
            byte[] chunk = new byte[length];
            Array.Copy(fileBytes, offset, chunk, 0, length);
            var msgModel = new MsgModel
            {
                userModel = new UserModel()
                {
                    uid = MyChatModel.Uid,
                    nickName = MyChatModel.NickName,
                    image = MyChatModel.Image
                },
                sendTime = DateTime.Now,
                message = Convert.ToBase64String(chunk),
                isGroupMsg = isGroupMsg,
                isImageOrFile = true,
                thisChunk = i,
                totalChunks = totalChunks
            };
            _myClient.SendMsg(topic, msgModel);
        }
    }

    private void UserSelect(ChatModel chatModel)
    {
        if (chatModel == null)
        {
            return;
        }
        try
        {
            _sendTopic = chatModel.Uid;
            _allNewMessageCount -= chatModel.MessageCount;
            if(_allNewMessageCount == 0)
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
    private void Minimize(Window window)
    {
        window.WindowState = WindowState.Minimized;
        GC.Collect();
    }

    [RelayCommand]
    private void Maximize(Window window)
    {
        if (window.WindowState == WindowState.Maximized)
        {
            window.WindowState = WindowState.Normal;
            IsMaximized = false;
            GC.Collect();
        }
        else
        {
            window.WindowState = WindowState.Maximized;
            IsMaximized = true;
        }
    }

    [RelayCommand]
    private void ToggleTopmost(Window window)
    {
        window.Topmost = !window.Topmost;
        IsTopmost = window.Topmost;
    }

    [RelayCommand]
    private void Hide(Window window)
    {
        window.Hide();
    }

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
                    var topic = isGroupMsg ? MqttContent.GROUP : MqttContent.MESSAGE + _sendTopic;
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
                    if (!isGroupMsg && !MyChatModel.Uid.Equals(_sendTopic))
                    {
                        if (_chatMessageDic.ContainsKey(_sendTopic))
                        {
                            _chatMessageDic[_sendTopic].Add(new ChatMessage
                            {
                                NickName = MyChatModel.NickName,
                                Image = MyChatModel.Image,
                                Message = msgModel.message,
                                Time = msgModel.sendTime.ToString(),
                                IsMyMessage = true
                            });
                            UserListVm.Users.Where(x => x.Uid == _sendTopic).First().MessageCount = 0;
                            UserListVm.Users.Where(x => x.Uid == _sendTopic).First().Message = msgModel.message;
                            ChatMessages.Messages = new BindingList<ChatMessage>(_chatMessageDic[_sendTopic]);
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

    [RelayCommand]
    private void ImageClick()
    {
        MyChatModel.Image = MqttContent.GetRandomImg();
    }
    
    #endregion

    #region Property
    [ObservableProperty]
    private bool _isMaximized;

    [ObservableProperty]
    private bool _isTopmost;

    // 左侧用户
    public UserListVm UserListVm { get; } = new();

    // 用户对应聊天框
    public MessageListVm ChatMessages { get; set; } = new();

    /// <summary>
    ///     发送信息
    /// </summary>
    [ObservableProperty] private string _sendMsg = string.Empty;

    /// <summary>
    ///     用户自己
    /// </summary>
    [ObservableProperty] private ChatModel _myChatModel = new ChatModel();
    /// <summary>
    ///     当前聊天对象
    /// </summary>
    [ObservableProperty] private ChatModel _chatObj = new ChatModel();

    #endregion
}