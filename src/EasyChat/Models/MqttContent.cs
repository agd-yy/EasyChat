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

    // 2.其他
    public const int SERVER_PORT = 1883;
    public const string SERVER_USER = "Doctor";
    public const string SERVER_PW = "233";
    public const string OFFLINE_STRING = "(已离线)";

    public static string IPADDRESS = string.Empty;
    public static string PASSWORD = string.Empty;
    public static string USER_NAME = string.Empty;

    // 生成随机头像用
    public static string GetRandomImg()
    {
        Random _random = new Random();
        return "/Resources/Images/p" + _random.Next(1, 13) + ".jpg";
    }
}