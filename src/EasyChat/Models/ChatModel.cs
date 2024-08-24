﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyChat.Models;

// 界面左侧展示的用户model
public partial class ChatModel : ObservableObject
{
    [ObservableProperty] private string color = "#ffad2c";

    // 一般来说，这个名字就是显示在界面上的聊天对象的名字，或者群名字
    [ObservableProperty] private string groupName = "";

    // 最后一条消息
    [ObservableProperty] private string message = "";

    // 未读消息数量
    [ObservableProperty] private int messageCount;

    // 群组名，私聊就是对方的昵称
    [ObservableProperty] private string tagName = "";

    // 当前聊天对象的ui等信息
    [ObservableProperty] private UserModel userModel = new UserModel();

    //  当前聊天对象对应的全部聊天信息
    //[ObservableProperty] private List<ChatMessage> chatMessages = new List<ChatMessage>();
}