using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyChat.Models;
using EasyChat.MQTT;

namespace EasyChat.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // 客户端的昵称，以后可以添加一个登录界面，这个和登录用户名绑定
    private readonly string _clientUid = Guid.NewGuid().ToString();
    private readonly MyMqttClient _myClient = MyMqttClient.Instance;
    private readonly HashSet<string> _onlineClientUid = [];
    private string _nickName;


    public MainViewModel()
    {
        // 客户端名绑定界面
        _nickName = string.IsNullOrEmpty(UserHandle.Instance.UserName) ? _clientUid : UserHandle.Instance.UserName;
        SubscribeUid = _nickName;
        //启动客户端
        _myClient.ChangeClientUid(_clientUid);
        _myClient.StartClient(UserHandle.Instance.ServiceIp);

        _myClient.OnlinePersonEvent += ClientChangeOnlinePerson;
        _myClient.ReceiveMsgEvent += ClientChangeReceiveMsg;
    }

    /// <summary>
    ///     客户端修改页面在线用户
    /// </summary>
    /// <param name="msgModel"></param>
    private void ClientChangeOnlinePerson(MsgModel msgModel)
    {
        _onlineClientUid.Add(msgModel.Uid);
        OnlinePerson = string.Join(Environment.NewLine, _onlineClientUid);
    }

    /// <summary>
    ///     客户端修改页面信息
    /// </summary>
    /// <param name="needAdd"></param>
    private void ClientChangeReceiveMsg(string needAdd)
    {
        ReceiveMsg += needAdd;
    }


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
                            Uid = _clientUid,
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

                if (_myClient.subTopics.Remove(MqttContent.MESSAGE + _clientUid))
                {
                    //取消订阅原来自己名字
                    //myClient.mqttClient.UnsubscribeAsync(MqttContent.MESSAGE + clientUID);
                    // 改名后告诉其他订阅过自己原来消息的客户端
                    _nickName = SubscribeUid;
                    _myClient.ChangeClientUid(_clientUid);
                    //myClient.SubOnlineServer(MqttContent.MESSAGE + clientUID);
                    MessageBox.Show("昵称修改成功");
                }
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
            Uid = _clientUid,
            SendTime = DateTime.Now,
            NickName = _nickName
        });
    }

    [RelayCommand]
    private void ClearText()
    {
        ReceiveMsg = "";
    }

    #region Property

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

    #endregion
}