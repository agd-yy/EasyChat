using System.Text;
using EasyChat.Extensions;
using EasyChat.Models;
using EasyChat.Utilities;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using Newtonsoft.Json;

namespace EasyChat.Service;

public class MyMqttClient : SingletonBase<MyMqttClient>
{
    public string MyClientUID { get; private set; } = Guid.NewGuid().ToString();
    public List<string> subTopics = [];
    private HashSet<string> topicSet = [];

    private MyMqttClient()
    {
        topicSet.Add(MqttContent.WHO_ONLINE);
        topicSet.Add(MqttContent.ONLINE);
        topicSet.Add(MqttContent.MESSAGE + MyClientUID);
        topicSet.Add(MqttContent.MESSAGE_ALL);
    }

    public IMqttClient? mqttClient { get; private set; }

    // 页面事件
    public event Action<string>? ReceiveMsgEvent;
    public event Action<MsgModel>? OnlinePersonEvent;

    /// <summary>
    ///     启动客户端
    /// </summary>
    /// <param name="clientUID"></param>
    /// <returns></returns>
    public async void StartClient(string ip)
    {
        // 创建Mqtt客户端工厂
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        // 配置Mqtt客户端选项
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(ip, MqttContent.SERVER_PORT) // 设置MQTT服务器地址和端口
            .WithClientId(MyClientUID)
            .WithCredentials(MqttContent.SERVER_USER, MqttContent.SERVER_PW) // 设置用户名密码
            .Build();

        try
        {
            // 连接到MQTT服务器
            await mqttClient.ConnectAsync(options);
            if (mqttClient.IsConnected) ReceiveMsgEvent?.Invoke($">> 成功连接到服务器 {Environment.NewLine}");
        }
        catch
        {
            ReceiveMsgEvent?.Invoke($">> 连接服务器失败 {Environment.NewLine}");
        }

        // 订阅主题
        InitSubTopic();
        mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(HandleMSG);
        // 重连机制
        mqttClient.UseDisconnectedHandler(async e =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5)); // 等5s
            try
            {
                // 重新连接
                await mqttClient.ConnectAsync(options);
                if (mqttClient.IsConnected) ReceiveMsgEvent?.Invoke($">> 成功重连到服务器 {Environment.NewLine}");
                // 重新订阅
                InitSubTopic();
            }
            catch
            {
                ReceiveMsgEvent?.Invoke($">> 重连服务器失败 {Environment.NewLine}");
            }
        });
    }

    /// <summary>
    /// 新增订阅主题
    /// </summary>
    /// <param name="topic"></param>
    public void AddTopic(string topic)
    {
        topicSet.Add(topic);
        SubOnlineServer(topic);
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="topic"></param>
    public void RemoveTopic(string topic)
    {
        topicSet.Remove(topic);
        try
        {
            mqttClient.UnsubscribeAsync(topic);
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
        foreach (var topic in topicSet)
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
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel).Build());
            subTopics.Add(topic);
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
    /// <param name="clientUID">昵称</param>
    public async void sendMsg(string topic, MsgModel msgModel)
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

        await mqttClient.PublishAsync(message);
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
                var ReceiveMsgStr =
                     $">> {msgModel.SendTime:yyyy-MM-dd HH:mm:ss}---From {msgModel.NickName}{Environment.NewLine}";
                ReceiveMsgStr += $">> {msgModel.Msg}{Environment.NewLine}";
                ReceiveMsgEvent?.Invoke(ReceiveMsgStr);
            }
            // 收到其他客户端在线消息
            else if (args.ApplicationMessage.Topic.Equals(MqttContent.ONLINE))
            {
                msgModel.Uids.Remove(MyClientUID);
                OnlinePersonEvent?.Invoke(msgModel);
            }
            //其他客户端指定消息
            else if (args.ApplicationMessage.Topic.Contains(MyClientUID))
            {
                var ReceiveMsgStr =
                    $">> {msgModel.SendTime:yyyy-MM-dd HH:mm:ss}---From {msgModel.NickName}{Environment.NewLine}";
                ReceiveMsgStr += $">> {msgModel.Msg}{Environment.NewLine}";
                ReceiveMsgEvent?.Invoke(ReceiveMsgStr);
                //MessageBox.Show("有您的指名消息❤~");
            }
        }
        catch
        {
            // 解密失败
        }
    }
}