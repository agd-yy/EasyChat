namespace EasyChat.Models;

/// <summary>
///     MQTT 静态常量类
/// </summary>
public static class MqttContent
{
    // 1.主题订阅相关
    // 上线 客户端上线发送/客户端订阅
    public const string ONLINE = "group/online";

    // 其他客户端寻求在线客户端
    public const string WHO_ONLINE = "group/whoOnline";

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


    // 客户端上线 发送上线主题 group/online + 自身uid
    // 同时订阅消息 group/online (获取上线客户端)； 还有 group/whoOnline(其他客户端寻求在线客户端)
    // message/# (全局消息)； message/自己UID (别人发送给自己的消息)
    // 发送消息 指定客户端发送主题 message/指定UID ，若不指定UID，则发送全体消息 message/#

    // 开发计划
    // 界面优化；消息提醒（闪烁或者声音提醒）；聊天页面按用户分组；点击在线用户进入聊天界面
    // 开发车间版，新增登录界面，设置服务器IP地址

    // 客户端自带一些默认头像，暂时不支持自己上传，用来让别人自己选(用base64封装起来)

}