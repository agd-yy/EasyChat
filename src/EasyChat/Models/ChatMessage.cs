using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyChat.Handle;

namespace EasyChat.Models;

/// <summary>
/// 聊天内容对象
/// </summary>
public partial class ChatMessage : ObservableObject
{
    [ObservableProperty] private string? nickName;

    [ObservableProperty] private string? image;

    [ObservableProperty] private string? message;

    [ObservableProperty] private string? time;

    [ObservableProperty] private bool isMyMessage;

    [ObservableProperty] private string? separatorTitle;

    [ObservableProperty] private string color = "#ff82a3";

    [ObservableProperty] private bool isFile;

    [ObservableProperty] private string fileName = "";

    [ObservableProperty] private string fileSize = "";

    [ObservableProperty] private string filePath = "";

    [ObservableProperty] private bool isReceived;

    [RelayCommand]
    private void FileReceive()
    {
        EventHelper.Instance.ReceiveFile(this);
    }
}