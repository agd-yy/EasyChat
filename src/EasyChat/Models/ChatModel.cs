using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyChat.Models
{
    public class ChatModel : ObservableObject
    {
        // 昵称
        private string _nickName = "";
        public string NickName
        {
            get => _nickName;
            set => SetProperty(ref _nickName, value);
        }
        private string _uid = "";
        public string Uid
        {
            get => _uid;
            set => SetProperty(ref _uid, value);
        }

        private string _message = "";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private string _color = "#ffad2c";
        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private string _tagName = "";
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

        private string _image = "";
        public string Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        // 一般来说，这个名字就是显示在界面上的聊天对象的名字，或者群名字
        private string _groupName = "";
        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }
    }
}