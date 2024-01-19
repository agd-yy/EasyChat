using EasyChat.Handle;
using EasyChat.MQTT;
using MQTT_Server;
using MQTTnet.Client;
using MQTTnet.Server;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EasyChat.ViewModel
{
    public class MainViewModel:SingletonUtils<MainViewModel>,INotifyPropertyChanged
    {

        private string clientUID = Guid.NewGuid().ToString();
        private static MqttService service;
        private MyMqttClient myClient = MyMqttClient.Instance();

        private MainViewModel()
        {
            // 启动服务端，临时本地当服务端
            service = MqttService.CreateMqttService();
            // 客户端id
            SubscribeUid = clientUID;
            //启动客户端
            myClient.ChangeClientUid(clientUID);
            myClient.StartClient();
            myClient.OnlinePersonEvent += ClientChangeOnlinePerson;
            myClient.ReceiveMsgEvent += ClientChangeReceiveMsg;

            // 事件绑定
            CommandInit();
        }
        /// <summary>
        /// 客户端修改页面在线用户
        /// </summary>
        /// <param name="needAdd"></param>
        private void ClientChangeOnlinePerson(string needAdd)
        {
            OnlinePerson += needAdd;
        }
        /// <summary>
        /// 客户端修改页面信息
        /// </summary>
        /// <param name="needAdd"></param>
        private void ClientChangeReceiveMsg(string needAdd)
        {
            ReceiveMsg += needAdd;
        }

        #region 页面属性
        private string mSendMsg;
        /// <summary>
        /// 发送信息
        /// </summary>
        public string SendMsg
        {
            get { return mSendMsg; }
            set { UpdateProperty(ref mSendMsg, value); }
        }

        private string mSendTopic;
        /// <summary>
        /// 发送主题
        /// </summary>
        public string SendTopic
        {
            get { return mSendTopic; }
            set { UpdateProperty(ref mSendTopic, value); }
        }

        private string mSubscribeUid;
        /// <summary>
        /// 用户名
        /// </summary>
        public string SubscribeUid
        {
            get { return mSubscribeUid; }
            set { UpdateProperty(ref mSubscribeUid, value); }
        }

        private string mReceiveMsg;
        /// <summary>
        /// 接收消息
        /// </summary>
        public string ReceiveMsg
        {
            
            get { return mReceiveMsg; }
            set { UpdateProperty(ref mReceiveMsg, value); }
        }

        private string monlinePerson;
        /// <summary>
        /// 在线人员
        /// </summary>
        public string OnlinePerson
        {
            get { return monlinePerson; }
            set { monlinePerson = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region 页面按钮事件
        public ICommand SendCommand { get;private set; }
        public ICommand SubscribeCommand { get;private set; }
        public ICommand OnlineCommand { get;private set; }
        public ICommand ClearTextCommand { get;private set; }

        private void CommandInit()
        {
            OnlineCommand = new RelayCommand(_ => { return true; }, _ =>
            {
                // 先清空当前在线客户端的信息
                OnlinePerson = "";
                // 询问在线的机子
                myClient.sendMsg(MqttContent.WHO_ONLINE, clientUID);
            });

            #region 消息推送，接收
            // 发送消息
            SendCommand = new RelayCommand(_ => { return true; }, _ => {
                try
                {
                    if (!string.IsNullOrEmpty(SendMsg)/* || !string.IsNullOrEmpty(SendTopic)*/)
                    {
                        Task.Run(() =>
                        {
                            myClient.sendMsg(MqttContent.MESSAGE + SendTopic, SendMsg);
                            SendMsg = "";
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            // 订阅消息
            SubscribeCommand = new RelayCommand(_ => { return true; }, _ =>
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

                        if (myClient.subTopics.Remove(MqttContent.MESSAGE + clientUID))
                        {
                            //取消订阅原来自己名字
                            myClient.mqttClient.UnsubscribeAsync(MqttContent.MESSAGE + clientUID);
                            // 改名后告诉其他订阅过自己原来消息的客户端
                            clientUID = SubscribeUid;
                            myClient.ChangeClientUid(clientUID);
                            myClient.SubOnlineServer(MqttContent.MESSAGE + clientUID);
                            MessageBox.Show("昵称修改成功");
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });
            #endregion
            // clear ReceiveMsg
            ClearTextCommand = new RelayCommand(_ => { return true; }, _ =>
            {
                ReceiveMsg = "";
            });
        }
        #endregion

        #region BASE
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="properValue"></param>
        /// <param name="newValue"></param>
        /// <param name="properName"></param>
        protected bool UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "")
        {
            //对比新旧值，如果相等就不更新
            if (object.Equals(properValue, newValue))
                return false;
            properValue = newValue;
            NotifyPropertyChanged(properName);
            return true;
        }


        /// <summary>
        /// 最基础的方式
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
