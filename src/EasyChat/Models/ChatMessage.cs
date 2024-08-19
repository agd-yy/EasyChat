namespace EasyChat.Models;

public class ChatMessage
{
    public ChatModel ChatModel { get; set; } = new();
    public string? NickName { get; set; }
    public string? Image { get; set; }
    public string? Message { get; set; }
    public string? Time { get; set; }
    public bool IsMyMessage { get; set; }
    public string? SeparatorTitle { get; set; }
    public string Color { get; set; } = "#ff82a3";
}