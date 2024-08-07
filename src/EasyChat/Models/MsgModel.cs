namespace EasyChat.Models;

/// <summary>
///     消息封装体对象
/// </summary>
public class MsgModel
{
    /// <summary>
    ///     UID
    /// </summary>
    public string Uid { get; set; } = "";

    /// <summary>
    ///     头像
    /// </summary>
    public string AvatarIcon { get; set; } = "";

    /// <summary>
    ///     昵称
    /// </summary>
    public string NickName { get; set; } = "";

    /// <summary>
    ///     发消息的时间
    /// </summary>
    public DateTime SendTime { get; set; }

    /// <summary>
    ///     消息体
    /// </summary>
    public string Msg { get; set; } = "";

    public HashSet<string> Uids { get; set; } = [];
}