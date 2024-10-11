using System.IO;
using System.Net.Sockets;
using System.Net;

namespace EasyChat.Service
{
    public class SocketServer
    {
        private TcpListener _tcpListener;
        private string _saveDirectory;

        public SocketServer(int port, string saveDirectory)
        {
            _saveDirectory = saveDirectory;
            _tcpListener = OkPort(port);
        }

        private TcpListener OkPort(int port)
        {
            try
            {
                var tcpListener = new TcpListener(IPAddress.Any, port);
                return tcpListener;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                return OkPort(port++);
            }
        }

        // 开始监听
        public async Task StartListeningAsync()
        {
            _tcpListener.Start();
            while (true)
            {
                TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                _ = Task.Run(() => ReceiveFileAsync(client));
            }
        }

        // 接收文件
        private async Task ReceiveFileAsync(TcpClient client)
        {
            try
            {
                using (NetworkStream networkStream = client.GetStream())
                {
                    // 读取文件名长度
                    byte[] fileNameLengthBuffer = new byte[4];
                    await networkStream.ReadAsync(fileNameLengthBuffer, 0, fileNameLengthBuffer.Length);
                    int fileNameLength = BitConverter.ToInt32(fileNameLengthBuffer, 0);

                    // 读取文件名
                    byte[] fileNameBuffer = new byte[fileNameLength];
                    await networkStream.ReadAsync(fileNameBuffer, 0, fileNameBuffer.Length);
                    string fileName = System.Text.Encoding.UTF8.GetString(fileNameBuffer);

                    // 读取文件大小
                    byte[] fileSizeBuffer = new byte[8];
                    await networkStream.ReadAsync(fileSizeBuffer, 0, fileSizeBuffer.Length);
                    long fileSize = BitConverter.ToInt64(fileSizeBuffer, 0);

                    //Console.WriteLine($"正在接收文件：{fileName}, 大小：{fileSize} bytes");

                    // 准备文件保存路径
                    string savePath = Path.Combine(_saveDirectory, fileName);

                    // 接收文件内容并写入到本地文件
                    using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[4096];
                        long totalBytesReceived = 0;
                        int bytesRead;

                        while (totalBytesReceived < fileSize && (bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesReceived += bytesRead;
                        }
                    }

                    //Console.WriteLine($"文件接收完成，保存路径：{savePath}");
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"文件接收失败：{ex.Message}");
            }
            finally
            {
                client.Close();  // 及时关闭客户端连接，释放资源
            }
        }

        // 停止监听
        public void StopListening()
        {
            _tcpListener.Stop();
        }
    }
}
