using EasyChat.Models;

namespace EasyChat.Handle;

/// <summary>
///     消息处理类
/// </summary>
public static class MsgHandle
{
    /// <summary>
    ///     处理消息
    /// </summary>
    /// <param name="payload">消息体</param>
    /// <param name="person">发消息的用户名</param>
    /// <returns></returns>
    public static string DealMsg(string payload, out string person)
    {
        var msgs = payload.Split(MqttContent.SUB_STRING);
        person = msgs.Length == 1 ? MqttContent.MESSAGE : msgs[1];
        return msgs[0];
    }
}