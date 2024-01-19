using EasyChat.ViewModel;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyChat.MQTT
{
    public class MyMqttClient
    {
        public IMqttClient mqttClient { get; private set; }
        private string IPAddress = "127.0.0.1";
        private int port = 1883;
        //private string IPAddress = "47.116.66.46";
        //private int port = 10087;
        public List<string> subTopics = new List<string>();
        private string myClientUID;

        // 页面事件
        public event Action<string> ReceiveMsgEvent;
        public event Action<string> OnlinePersonEvent;

        /// <summary>
        /// 启动客户端
        /// </summary>
        /// <param name="clientUID"></param>
        /// <returns></returns>
        public async void StartClient()
        {
            if (string.IsNullOrEmpty(myClientUID))
            {
                return;
            }
            // 创建Mqtt客户端工厂
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            // 配置Mqtt客户端选项
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(IPAddress, port) // 设置MQTT服务器地址和端口
                .WithClientId(myClientUID)
                .WithCredentials("admin", "123456")// 设置用户名密码
                .Build();

            try
            {
                // 连接到MQTT服务器
                await mqttClient.ConnectAsync(options);
                if (mqttClient.IsConnected)
                {
                    ReceiveMsgEvent?.Invoke($">> 成功连接到服务器 {Environment.NewLine}");
                }
            }
            catch
            {
                ReceiveMsgEvent?.Invoke($">> 连接服务器失败 {Environment.NewLine}");
            }
            // 订阅主题
            SubTopic();
            mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(HandleMSG);
            // 重连机制
            mqttClient.UseDisconnectedHandler(async e =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));// 等5s
                try
                {
                    // 重新连接
                    await mqttClient.ConnectAsync(options);
                    if (mqttClient.IsConnected)
                    {
                        ReceiveMsgEvent?.Invoke($">> 成功重连到服务器 {Environment.NewLine}");
                    }
                    // 重新订阅
                    SubTopic();
                }
                catch
                {
                    ReceiveMsgEvent?.Invoke($">> 重连服务器失败 {Environment.NewLine}");
                }
            });
        }

        /// <summary>
        /// 修改用户昵称
        /// </summary>
        /// <param name="clientUID"></param>
        public void ChangeClientUid(string clientUID)
        {
            myClientUID = clientUID;
        }
        /// <summary>
        /// 订阅主题
        /// </summary>
        private void SubTopic()
        {
            // 订阅获取在线用户和询问在线用户
            SubOnlineServer(MqttContent.WHO_ONLINE);
            SubOnlineServer(MqttContent.ONLINE);
            // 订阅给自身UID发消息的主题
            SubOnlineServer(MqttContent.MESSAGE + myClientUID);
            // 订阅全局消息，调试用
            SubOnlineServer(MqttContent.MESSAGE_ALL);
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topic"></param>
        public async void SubOnlineServer(string topic)
        {
            int qosLevel = 0; // QoS级别，可以是0、1或2
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
        /// 发送消息
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="msgstr">消息</param>
        /// <param name="clientUID">昵称</param>
        public async void sendMsg(string topic, string msgstr)
        {
            // 发布消息
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(msgstr + MqttContent.SUB_STRING + myClientUID)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message);
        }

        public void HandleMSG(MqttApplicationMessageReceivedEventArgs args)
        {
            // 全局消息
            if (args.ApplicationMessage.Topic.Equals(MqttContent.MESSAGE))
            {
                var msg = MsgHandle.DealMsg(Encoding.UTF8.GetString(args.ApplicationMessage.Payload), out string person);
                var ReceiveMsgStr = $">> {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}---From {person}{Environment.NewLine}";
                ReceiveMsgStr += $">> {msg}{Environment.NewLine}";
                ReceiveMsgEvent?.Invoke(ReceiveMsgStr);
                MessageBox.Show("有全体消息啦！");
            }
            // 收到其他客户端询问在线的机子
            else if (args.ApplicationMessage.Topic.Equals(MqttContent.WHO_ONLINE))
            {
                var msg = MsgHandle.DealMsg(Encoding.UTF8.GetString(args.ApplicationMessage.Payload), out string person);
                if (!myClientUID.Equals(person))
                {
                    // 发送本机在线
                    sendMsg(MqttContent.ONLINE, myClientUID);
                }
            }
            // 收到其他客户端在线消息
            else if (args.ApplicationMessage.Topic.Equals(MqttContent.ONLINE))
            {
                var msg = MsgHandle.DealMsg(Encoding.UTF8.GetString(args.ApplicationMessage.Payload), out string person);
                if (!myClientUID.Equals(person))
                {
                    OnlinePersonEvent?.Invoke($"{person}{Environment.NewLine}");
                }
            }
            //其他客户端指定消息
            else if (args.ApplicationMessage.Topic.Contains(myClientUID))
            {
                var msg = MsgHandle.DealMsg(Encoding.UTF8.GetString(args.ApplicationMessage.Payload), out string person);
                var ReceiveMsgStr = $">> {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}---From {person}{Environment.NewLine}";
                ReceiveMsgStr += $">> {msg}{Environment.NewLine}";
                ReceiveMsgEvent?.Invoke(ReceiveMsgStr);
                MessageBox.Show("有您的指名消息❤~");
            }
        }
    }
}
