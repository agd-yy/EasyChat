using EasyChat.Handle;
using EasyChat.MQTT;
using EasyChat.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EasyChat.ViewModels
{
    public class MainViewModel:SingletonUtils<MainViewModel>,INotifyPropertyChanged
    {
        private MyMqttClient myClient = MyMqttClient.Instance();
        // 客户端的昵称，以后可以添加一个登录界面，这个和登录用户名绑定
        private string clientUID = Guid.NewGuid().ToString();
        private string nickName;

        private HashSet<string> onlineClientUid = new HashSet<string>();

        private MainViewModel()
        {
            // 客户端名绑定界面
            nickName = string.IsNullOrEmpty(UserHandle.Instance().UserName) ? clientUID : UserHandle.Instance().UserName;
            SubscribeUid = nickName;
            //启动客户端
            myClient.ChangeClientUid(clientUID);
            myClient.StartClient(UserHandle.Instance().ServiceIp);

            myClient.OnlinePersonEvent += ClientChangeOnlinePerson;
            myClient.ReceiveMsgEvent += ClientChangeReceiveMsg;

            // 事件绑定
            CommandInit();
        }
        /// <summary>
        /// 客户端修改页面在线用户
        /// </summary>
        /// <param name="msgModel"></param>
        private void ClientChangeOnlinePerson(MsgModel msgModel)
        {
            onlineClientUid.Add(msgModel.Uid);
            OnlinePerson = string.Join(Environment.NewLine, onlineClientUid);
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
                myClient.sendMsg(MqttContent.WHO_ONLINE, new MsgModel()
                {
                    Uid = clientUID,
                    SendTime = DateTime.Now,
                    NickName = nickName,
                });
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
                            myClient.sendMsg(MqttContent.MESSAGE + SendTopic, 
                                new MsgModel()
                                {
                                    Uid = clientUID,
                                    SendTime = DateTime.Now,
                                    Msg = SendMsg,
                                    NickName = nickName
                                });
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
                            //myClient.mqttClient.UnsubscribeAsync(MqttContent.MESSAGE + clientUID);
                            // 改名后告诉其他订阅过自己原来消息的客户端
                            nickName = SubscribeUid;
                            myClient.ChangeClientUid(clientUID);
                            //myClient.SubOnlineServer(MqttContent.MESSAGE + clientUID);
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
