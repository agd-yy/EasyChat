using EasyChat.Models;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasyChat.ViewModels.SubVms;

public partial class UserListVm : ObservableObject
{
    public Action<ChatModel>? OnSelected { get; set; }
    public Action<ChatModel>? RightClicked { get; set; }

    [ObservableProperty] private BindingList<ChatModel> _users = [];

    [RelayCommand] private void Selected(ChatModel user) => OnSelected?.Invoke(user);

    [RelayCommand] private void RightClick(ChatModel user) => OnSelected?.Invoke(user);
}