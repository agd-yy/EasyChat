using System.Text;
using EasyChat.Extensions;
using EasyChat.Models;
using EasyChat.Utilities;
using MQTTnet;
using MQTTnet.Server;

namespace EasyChat.Service;

public class MqttService
{
    private static readonly MqttService mqttService = new();
    private static IMqttServer? server;
    // 记录当前在线客户端
    private static List<UserModel> onlineClientUids = [];

    private MqttService()
    {
        StartServer();
    }

    // 单例
    public static MqttService CreateMqttService()
    {
        return mqttService;
    }

    #region server

    private static async void StartServer()
    {
        try
        {
            // 1. 创建 MQTT 连接验证，用于连接鉴权
            var connectionValidatorDelegate = new MqttServerConnectionValidatorDelegate(
            p =>
            {
                // 大部分情况下，我们应该使用客户端加密 token 验证，也就是可客户端 ID 对应的密钥加密后的 token
                if (p.Username != MqttContent.SERVER_USER && p.Password != MqttContent.SERVER_PW)
                {
                    // 验证失败，告诉客户端，鉴权失败
                    p.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                }
            });

            // 2. 创建 MqttServerOptions 的实例，用来定制 MQTT 的各种参数
            var options = new MqttServerOptions();

            // 3. 设置各种选项
            // 客户端鉴权
            options.ConnectionValidator = connectionValidatorDelegate;
            // 设置服务器端地址和端口号
            options.DefaultEndpointOptions.Port = MqttContent.SERVER_PORT;

            // 4. 创建 MqttServer 实例
            server = new MqttFactory().CreateMqttServer();

            // 5. 设置 MqttServer 的属性
            // 设置消息订阅通知
            server.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(SubScribedTopic);
            // 设置消息退订通知
            server.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(UnScribedTopic);
            // 设置消息处理程序
            server.UseApplicationMessageReceivedHandler(MessageReceived);
            // 设置客户端连接成功后的处理程序
            server.UseClientConnectedHandler(ClientConnected);
            // 设置客户端断开后的处理程序
            server.UseClientDisconnectedHandler(ClientDisConnected);

            // 启动服务器
            await server.StartAsync(options);
        }
        catch
        {
        }
    }

    /// <summary>
    /// 客户端发起订阅主题通知
    /// 以后用来加群
    /// </summary>
    /// <param name="args"></param>
    private static void SubScribedTopic(MqttServerClientSubscribedTopicEventArgs args)
    {
        var userModel = JsonExtension.Deserialize<UserModel>(args.ClientId);
        if (userModel != null)
        {
            // 获取客户端识别码
            var clientId = userModel.uid;
            // 获取客户端发起的订阅主题
            var topic = args.TopicFilter.Topic;
            Console.WriteLine($"客户端[{clientId}]已订阅主题:{topic}");
        }

    }

    /// <summary>
    /// 客户端取消主题订阅通知
    /// 以后用来退群
    /// </summary>
    /// <param name="args"></param>
    private static void UnScribedTopic(MqttServerClientUnsubscribedTopicEventArgs args)
    {
        // 获取客户端识别码
        var clientId = args.ClientId;
        // 获取客户端发起的订阅主题
        var topic = args.TopicFilter;

        Console.WriteLine($"客户端[{clientId}]已退订主题:{topic}");
    }

    /// <summary>
    /// 接收客户端发送的消息
    /// </summary>
    /// <param name="args"></param>
    private static void MessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        var userModel = JsonExtension.Deserialize<ChatModel>(args.ClientId);
        if (userModel == null)
        {
            return;
        }
        // 获取消息的客户端识别码
        var clientId = userModel.Uid;
        // 获取消息的主题
        var topic = args.ApplicationMessage.Topic;
        // 获取发送的消息内容
        var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
        // 获取消息的发送级别(Qos)
        var qos = args.ApplicationMessage.QualityOfServiceLevel;
        // 获取消息的保持形式
        var retain = args.ApplicationMessage.Retain;
        // 收到其他客户端询问在线的机子
        if (MqttContent.WHO_ONLINE.Equals(topic))
        {
            var msg = EncryptUtilities.Encrypt(new MsgModel()
            {
                userModels = onlineClientUids,
                sendTime = DateTime.Now
            }.Serialize());
            ServierPublish(MqttContent.ONLINE, msg);
        }
        Console.WriteLine($"客户端[{clientId}] >> 主题: [{topic}] 内容: [{payload}] Qos: [{qos}] Retain:[{retain}]");
    }

    /// <summary>
    /// 客户端连接成功后的的处理通知
    /// </summary>
    /// <param name="args"></param>
    private static void ClientConnected(MqttServerClientConnectedEventArgs args)
    {
        var userModel = JsonExtension.Deserialize<UserModel>(args.ClientId);
        if (userModel == null)
        {
            return;
        }    
        userModel.isOnline = true;
        if (!onlineClientUids.Any(o => o.uid == userModel.uid))
        {
            onlineClientUids.Add(userModel);
        }
        onlineClientUids.RemoveAll(o => o.isOnline == false);
        var msg = EncryptUtilities.Encrypt(new MsgModel()
        {
            userModels = onlineClientUids,
            sendTime = DateTime.Now
        }.Serialize());
        ServierPublish(MqttContent.ONLINE, msg);
    }

    /// <summary>
    /// 客户端断开连接通知
    /// </summary>
    /// <param name="args"></param>
    private static void ClientDisConnected(MqttServerClientDisconnectedEventArgs args)
    {
        var userModel = JsonExtension.Deserialize<ChatModel>(args.ClientId);
        if (userModel == null)
        {
            return;
        }
        // 从 onlineClientUids中获取uid == userModel.uid的对象，然后修改isOnline状态
        onlineClientUids.Where(o => o.uid == userModel.Uid).First().isOnline = false;
        var msg = EncryptUtilities.Encrypt(new MsgModel()
        {
            userModels = onlineClientUids,
            sendTime = DateTime.Now
        }.Serialize());
        ServierPublish(MqttContent.ONLINE, msg);

    }

    private static void ServierPublish(string topic, string msg)
    {
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(msg)
            .WithExactlyOnceQoS() // 设置QoS
            .WithRetainFlag()
            .Build();

        // 发布消息
        server.PublishAsync(mqttMessage);
    }

    #endregion
}