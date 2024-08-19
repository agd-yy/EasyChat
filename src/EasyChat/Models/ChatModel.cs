using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyChat.Models;

public partial class ChatModel : ObservableObject
{
    [ObservableProperty] private string _color = "#ffad2c";

    // 一般来说，这个名字就是显示在界面上的聊天对象的名字，或者群名字
    [ObservableProperty] private string _groupName = "";


    [ObservableProperty] private string _image = "";

    [ObservableProperty] private string _message = "";

    [ObservableProperty] private int _messageCount;

    // 昵称
    [ObservableProperty] private string _nickName = "";

    [ObservableProperty] private string _tagName = "";
    [ObservableProperty] private string _uid = "";
}