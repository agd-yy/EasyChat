namespace EasyChat.Models;

/// <summary>
///     MQTT 静态常量类
/// </summary>
public static class MqttContent
{
    // 1.主题订阅相关
    // 上线 客户端上线发送/客户端订阅
    public const string ONLINE = "message/online";

    // 其他客户端寻求在线客户端
    public const string WHO_ONLINE = "message/whoOnline";

    // 分组
    public const string GROUP = "group/";

    // 消息
    public const string MESSAGE = "message/";
    public const string MESSAGE_ALL = "message/#";

    // 2.其他
    // 分割消息与用户名的字符
    public const char SUB_STRING = '$';

    public const int SERVER_PORT = 1883;
    public const string SERVER_USER = "Doctor";
    public const string SERVER_PW = "233";

    // 生成随机头像用
    public static string GetRandomImg()
    {
        Random _random = new Random();
        return "/Resources/Images/p" + _random.Next(1, 13) + ".jpg";
    }

    // message/# (全局消息)； message/自己UID (别人发送给自己的消息)
    // 界面优化；消息提醒（闪烁或者声音提醒）；聊天页面按用户分组；点击在线用户进入聊天界面


}