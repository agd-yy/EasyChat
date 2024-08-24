namespace EasyChat.Models;

/// <summary>
/// 消息封装体对象
/// </summary>
public class MsgModel
{
    /// <summary>
    /// userModel
    /// </summary>
    public UserModel userModel { get; set; } = new UserModel();

    /// <summary>
    /// 发消息的时间
    /// </summary>
    public DateTime sendTime { get; set; }

    /// <summary>
    /// 消息体
    /// </summary>
    public string message { get; set; } = "";

    /// <summary>
    /// 是否是群消息
    /// </summary>
    public bool isGroupMsg { get; set; }

    /// <summary>
    /// 群名称
    /// </summary>
    public string groupName { get; set; } = "";

    /// <summary>
    /// 所有在线用户，仅服务端需要
    /// </summary>
    public List<UserModel> userModels { get; set; } = [];
}