using System.Text;
using EasyChat.Extensions;
using EasyChat.Models;
using EasyChat.Utilities;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;

namespace EasyChat.Service;

public class MyMqttClient : SingletonBase<MyMqttClient>
{
    public string MyClientUid { get; private set; } = Guid.NewGuid().ToString();
    public List<string> SubTopics = [];
    private readonly HashSet<string> _topicSet = [];
    public IMqttClient? MqttClient { get; private set; }

    // 页面事件
    public event Action<List<ChatMessage>>? ReceiveMsgEvent;
    public event Action<MsgModel>? OnlinePersonEvent;


    private MyMqttClient()
    {
        _topicSet.Add(MqttContent.WHO_ONLINE);
        _topicSet.Add(MqttContent.ONLINE);
        _topicSet.Add(MqttContent.MESSAGE + MyClientUid);
        _topicSet.Add(MqttContent.MESSAGE_ALL);
    }

    /// <summary>
    ///     启动客户端
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public async void StartClient(string ip)
    {
        // 创建Mqtt客户端工厂
        var factory = new MqttFactory();
        MqttClient = factory.CreateMqttClient();

        // 配置Mqtt客户端选项
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(ip, MqttContent.SERVER_PORT) // 设置MQTT服务器地址和端口
            .WithClientId(MyClientUid)
            .WithCredentials(MqttContent.SERVER_USER, MqttContent.SERVER_PW) // 设置用户名密码
            .Build();

        try
        {
            // 连接到MQTT服务器
            await MqttClient.ConnectAsync(options);
            if (MqttClient.IsConnected)
            {
                ReceiveMsgEvent?.Invoke([new ChatMessage { Message = "连接服务器成功", Time = DateTime.Now.ToString()}]);
            }
        }
        catch
        {
            ReceiveMsgEvent?.Invoke([new ChatMessage { Message = "连接服务器失败", Time = DateTime.Now.ToString()}]);
        }

        // 订阅主题
        InitSubTopic();
        MqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(HandleMSG);
        // 重连机制
        MqttClient.UseDisconnectedHandler(async e =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5)); // 等5s
            try
            {
                // 重新连接
                await MqttClient.ConnectAsync(options);
                if (MqttClient.IsConnected) ReceiveMsgEvent?.Invoke([new ChatMessage { Message = "重新连接服务器成功", Time = DateTime.Now.ToString()}]);
                // 重新订阅
                InitSubTopic();
            }
            catch
            {
                ReceiveMsgEvent?.Invoke([new ChatMessage { Message = "重连服务器失败", Time = DateTime.Now.ToString()}]);
            }
        });
    }

    /// <summary>
    /// 新增订阅主题
    /// </summary>
    /// <param name="topic"></param>
    public void AddTopic(string topic)
    {
        _topicSet.Add(topic);
        SubOnlineServer(topic);
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="topic"></param>
    public void RemoveTopic(string topic)
    {
        _topicSet.Remove(topic);
        try
        {
            MqttClient.UnsubscribeAsync(topic);
        }
        catch
        {
            // 取消订阅失败也是连接失败导致的
        }
    }

    /// <summary>
    /// 订阅主题
    /// </summary>
    private void InitSubTopic()
    {
        foreach (var topic in _topicSet)
        {
            SubOnlineServer(topic);
        }
    }

    /// <summary>
    /// 订阅
    /// </summary>
    /// <param name="topic"></param>
    private async void SubOnlineServer(string topic)
    {
        var qosLevel = 0; // QoS级别，可以是0、1或2
        try
        {
            // 订阅消息
            await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel).Build());
            SubTopics.Add(topic);
        }
        catch
        {
            // 订阅失败一般是服务器连不上导致的
        }
    }

    /// <summary>
    ///     发送消息
    /// </summary>
    /// <param name="topic">主题</param>
    /// <param name="msgModel">消息</param>
    public async void SendMsg(string topic, MsgModel msgModel)
    {
        // 消息加密
        var msg = EncryptUtilities.Encrypt(msgModel.Serialize());
        // 发布消息
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(msg)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag()
            .Build();

        await MqttClient.PublishAsync(message);
    }

    private void HandleMSG(MqttApplicationMessageReceivedEventArgs args)
    {
        try
        {
            var msgModel = EncryptUtilities.Decrypt(Encoding.UTF8.GetString(args.ApplicationMessage.Payload))
                .Deserialize<MsgModel>();
            if (msgModel == null)
            {
                return;
            }
            // 全局消息
            if (args.ApplicationMessage.Topic.Equals(MqttContent.MESSAGE))
            {
                List<ChatMessage> list = new List<ChatMessage>();
                list.Add(new ChatMessage { NickName = msgModel.NickName, Image = msgModel.Img });
                list.Add(new ChatMessage { Message = msgModel.Msg, Time = msgModel.SendTime.ToString(),
                IsMyMessage = msgModel.Uid == MyClientUid});
                ReceiveMsgEvent?.Invoke(list);
            }
            // 收到其他客户端在线消息
            else if (args.ApplicationMessage.Topic.Equals(MqttContent.ONLINE))
            {
                msgModel.ChatModels.Where(o => o.Uid != MyClientUid);
                OnlinePersonEvent?.Invoke(msgModel);
            }
            //其他客户端指定消息
            else if (args.ApplicationMessage.Topic.Contains(MyClientUid))
            {
                List<ChatMessage> list = new List<ChatMessage>();
                list.Add(new ChatMessage { NickName = msgModel.NickName, Image = msgModel.Img });
                list.Add(new ChatMessage { Message = msgModel.Msg, Time = msgModel.SendTime.ToString() });
                ReceiveMsgEvent?.Invoke(list);
                //MessageBox.Show("有您的指名消息❤~");
            }
        }
        catch
        {
            // 解密失败
        }
    }
}