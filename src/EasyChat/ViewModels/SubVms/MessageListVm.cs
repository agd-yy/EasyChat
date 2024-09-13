using CommunityToolkit.Mvvm.ComponentModel;
using EasyChat.Models;
using System.ComponentModel;

namespace EasyChat.ViewModels.SubVms
{
    public partial class MessageListVm : ObservableObject
    {
        [ObservableProperty] private BindingList<ChatMessage> _messages = [];
    }
}
