using System.IO;
using System.Net.Sockets;

namespace EasyChat.Service
{
    public class SocketClient
    {
        private static SocketClient? _client;

        private SocketClient()
        {
        }

        public static SocketClient GetInctance()
        {
            if (_client == null)
            {
                _client = new SocketClient();
            }
            return _client;
        }

        // 发送文件
        public async Task SendFileAsync(string filePath, string ip, int port)
        {
            //if (!File.Exists(filePath))
            //{
            //    //System.Diagnostics.Debug.WriteLine("文件不存在");
            //    return;
            //}
            TcpClient client = new TcpClient(AddressFamily.InterNetwork);
            try
            {
                client.Connect(ip, port);
                using NetworkStream networkStream = client.GetStream();
                FileInfo fileInfo = new FileInfo(filePath);

                // 获取文件名和文件大小
                string fileName = fileInfo.Name;
                long fileSize = fileInfo.Length;

                // 发送文件名长度
                byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
                byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
                await networkStream.WriteAsync(fileNameLengthBytes, 0, fileNameLengthBytes.Length);

                // 发送文件名
                await networkStream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);

                // 发送文件大小
                byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
                await networkStream.WriteAsync(fileSizeBytes, 0, fileSizeBytes.Length);

                //System.Diagnostics.Debug.WriteLine($"开始发送文件：{fileName}, 大小：{fileSize} bytes");

                // 发送文件内容
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;

                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await networkStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"文件发送失败：{ex.Message}");
            }
            finally
            {
                client.Close();
                client.Dispose();
            }
        }
    }
}
