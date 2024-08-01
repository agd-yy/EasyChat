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

    // 客户端上线 发送上线主题 group/online + 自身uid
    // 同时订阅消息 group/online (获取上线客户端)； 还有 group/whoOnline(其他客户端寻求在线客户端)
    // message/# (全局消息)； message/自己UID (别人发送给自己的消息)
    // 发送消息 指定客户端发送主题 message/指定UID ，若不指定UID，则发送全体消息 message/#

    // 开发计划
    // 界面优化；消息提醒（闪烁或者声音提醒）；聊天页面按用户分组；点击在线用户进入聊天界面
    // 开发车间版，新增登录界面，设置服务器IP地址

    // 新增trueID作为客户端唯一标识(可以用作登录认证，或者仅记录UUID，
    // 这个最好是固定值，方便昵称修改，重复的昵称也可以存在)
    // 客户端自带一些默认头像，暂时不支持自己上传，用来让别人自己选(用base64封装起来)
    // 1.收到客户端在线请求消息时，将用户名与自己内存的trueId比对,然后更新昵称；
    // 2.给每个客户端私发消息时，取trueId, 
    // 3.topic 格式改成：消息类型/对象trueId
    // 4.消息体格式改成json可以转obj：本人trueId/本人头像/本人昵称/消息内容

    // 在线用户先改成 Set 集合
}