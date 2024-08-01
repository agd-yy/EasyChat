using EasyChat.Utilities;

namespace EasyChat.Models;

/// <summary>
///     用户相关(用户名修改，
/// </summary>
public class UserHandle : SingletonBase<UserHandle>
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";

    public string ServiceIp { get; set; } = "";
}