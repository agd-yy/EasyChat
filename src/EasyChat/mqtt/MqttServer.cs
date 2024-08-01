using System.Text;
using MQTTnet;
using MQTTnet.Server;

namespace MQTT_Server;

public class MqttService
{
    private static readonly MqttService mqttService = new();

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
                    // p 表示正在发起的一个链接的上下文
                    // 用户名和密码验证
                    // 大部分情况下，我们应该使用客户端加密 token 验证，也就是可客户端 ID 对应的密钥加密后的 token
                    /*if (p.Username != "admin" || p.Password != "123456")
                    {
                        // 验证失败，告诉客户端，鉴权失败
                        p.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                    }*/
                }
            );


            // 2. 创建 MqttServerOptions 的实例，用来定制 MQTT 的各种参数
            var options = new MqttServerOptions();

            // 3. 设置各种选项
            // 客户端鉴权
            options.ConnectionValidator = connectionValidatorDelegate;

            // 设置服务器端地址和端口号
            options.DefaultEndpointOptions.Port = 1883;

            // 4. 创建 MqttServer 实例
            var mqttServer = new MqttFactory().CreateMqttServer();

            // 5. 设置 MqttServer 的属性
            // 设置消息订阅通知
            mqttServer.ClientSubscribedTopicHandler =
                new MqttServerClientSubscribedTopicHandlerDelegate(
                    (Action<MqttServerClientSubscribedTopicEventArgs>)SubScribedTopic);
            // 设置消息退订通知
            mqttServer.ClientUnsubscribedTopicHandler =
                new MqttServerClientUnsubscribedTopicHandlerDelegate(
                    (Action<MqttServerClientUnsubscribedTopicEventArgs>)UnScribedTopic);
            // 设置消息处理程序
            mqttServer.UseApplicationMessageReceivedHandler(
                (Action<MqttApplicationMessageReceivedEventArgs>)MessageReceived);
            // 设置客户端连接成功后的处理程序
            mqttServer.UseClientConnectedHandler((Action<MqttServerClientConnectedEventArgs>)ClientConnected);
            // 设置客户端断开后的处理程序
            mqttServer.UseClientDisconnectedHandler((Action<MqttServerClientDisconnectedEventArgs>)ClientDisConnected);

            // 启动服务器
            await mqttServer.StartAsync(options);

            Console.WriteLine("服务器启动成功！直接按回车停止服务");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.Write($"服务器启动失败:{ex}");
        }
    }


    // 客户端发起订阅主题通知
    private static void SubScribedTopic(MqttServerClientSubscribedTopicEventArgs args)
    {
        // 获取客户端识别码
        var clientId = args.ClientId;
        // 获取客户端发起的订阅主题
        var topic = args.TopicFilter.Topic;

        Console.WriteLine($"客户端[{clientId}]已订阅主题:{topic}");
    }

    // 客户端取消主题订阅通知
    private static void UnScribedTopic(MqttServerClientUnsubscribedTopicEventArgs args)
    {
        // 获取客户端识别码
        var clientId = args.ClientId;
        // 获取客户端发起的订阅主题
        var topic = args.TopicFilter;

        Console.WriteLine($"客户端[{clientId}]已退订主题:{topic}");
    }

    // 接收客户端发送的消息
    private static void MessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        // 获取消息的客户端识别码
        var clientId = args.ClientId;
        // 获取消息的主题
        var topic = args.ApplicationMessage.Topic;
        // 获取发送的消息内容
        var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
        // 获取消息的发送级别(Qos)
        var qos = args.ApplicationMessage.QualityOfServiceLevel;
        // 获取消息的保持形式
        var retain = args.ApplicationMessage.Retain;

        Console.WriteLine($"客户端[{clientId}] >> 主题: [{topic}] 内容: [{payload}] Qos: [{qos}] Retain:[{retain}]");
    }

    // 客户端连接成功后的的处理通知
    private static void ClientConnected(MqttServerClientConnectedEventArgs args)
    {
        // 获取客户端识别码
        var clientId = args.ClientId;

        Console.WriteLine($"新客户端[{clientId}] 加入");
    }

    // 客户端断开连接通知
    private static void ClientDisConnected(MqttServerClientDisconnectedEventArgs args)
    {
        // 获取客户端识别码
        var clientId = args.ClientId;

        Console.WriteLine($"新客户端[{clientId}] 已经离开");
    }

    #endregion
}