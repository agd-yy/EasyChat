﻿
namespace EasyChat.Models
{
    public class UserModel
    {
        public string nickName { get; set; } = "";
        public string image { get; set; } = "";

        public string uid { get; set; } = "";

        public bool isOnline { get; set; } = true;
        public bool isGroup { get; set; } = false;

        public string ipAddress { get; set; } = "";
        public int port { get; set; } = MqttContent.SOCKET_PORT;
    }
}
