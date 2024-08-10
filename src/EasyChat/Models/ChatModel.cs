using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyChat.Models
{
    public class ChatModel : ObservableObject
    {
        private string _nickName;
        public string NickName
        {
            get => _nickName;
            set => SetProperty(ref _nickName, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private string _color;
        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private string _tagName;
        public string TagName
        {
            get => _tagName;
            set => SetProperty(ref _tagName, value);
        }

        private int _messageCount;
        public int MessageCount
        {
            get => _messageCount;
            set => SetProperty(ref _messageCount, value);
        }

        private string _image;
        public string Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }
    }
}