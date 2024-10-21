namespace EasyChat.Models
{
    public class FileModel
    {
        /// <summary>
        /// 文件名，主要用来获取文件后缀
        /// </summary>
        public string fileName { get; set; } = "";

        /// <summary>
        /// 文件大小
        /// </summary>
        public string fileSize { get; set; } = "";

        /// <summary>
        /// 发送方本地文件路径
        /// 当发送方发送文件时，先用MQTT发送消息，告诉接收方文件大小和格式
        /// 当接收方点击下载按钮，才会进行Socket通讯
        /// </summary>
        public string clientFilePath { get; set; } = "";
    }
}
