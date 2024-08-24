namespace EasyChat.Models;

/// <summary>
/// 聊天内容对象
/// </summary>
public class ChatMessage
{
    public string? nickName { get; set; }
    public string? image { get; set; }
    public string? message { get; set; }
    public string? time { get; set; }
    public bool isMyMessage { get; set; }
    public string? separatorTitle { get; set; }
    public string color { get; set; } = "#ff82a3";
}