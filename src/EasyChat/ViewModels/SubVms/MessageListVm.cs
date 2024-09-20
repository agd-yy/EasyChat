using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyChat.Models;
using System.ComponentModel;

namespace EasyChat.ViewModels.SubVms
{
    public partial class MessageListVm : ObservableObject
    {
        public Action<ChatMessage>? RightClicked { get; set; }

        [ObservableProperty] private BindingList<ChatMessage> _messages = [];

        [RelayCommand] private void RightClick(ChatMessage message) => RightClicked?.Invoke(message);
    }
}
