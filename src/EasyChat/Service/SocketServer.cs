using System.IO;
using System.Net.Sockets;
using System.Net;
using EasyChat.Controls;

namespace EasyChat.Service
{
    public class SocketServer
    {
        private static SocketServer? _instance;
        private TcpListener _tcpListener;

        private SocketServer(int port)
        {
            _tcpListener = OkPort(port);
            _tcpListener.Start();
        }
        public static SocketServer GetInstance(int port)
        {
            if (_instance == null)
            {
                _instance = new SocketServer(port);
            }
            return _instance;
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
                return OkPort(++port);
            }
        }

        // 开始监听
        public async Task StartReceiveAsync(string savedirectory)
        {
            while (true)
            {
                TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                _ = Task.Run(() => ReceiveFileAsync(client, savedirectory));
            }
        }

        // 接收文件
        private async Task ReceiveFileAsync(TcpClient client, string savedirectory)
        {
            try
            {
                using (NetworkStream networkStream = client.GetStream())
                {
                    networkStream.ReadTimeout = 5000;
                    // 读取文件名长度
                    byte[] fileNameLengthBuffer = new byte[4];
                    await networkStream.ReadAsync(fileNameLengthBuffer, 0, fileNameLengthBuffer.Length);
                    int fileNameLength = BitConverter.ToInt32(fileNameLengthBuffer, 0);

                    // 读取文件名
                    byte[] fileNameBuffer = new byte[fileNameLength];
                    await networkStream.ReadAsync(fileNameBuffer, 0, fileNameBuffer.Length);

                    // 读取文件大小
                    byte[] fileSizeBuffer = new byte[8];
                    await networkStream.ReadAsync(fileSizeBuffer, 0, fileSizeBuffer.Length);
                    long fileSize = BitConverter.ToInt64(fileSizeBuffer, 0);

                    // 接收文件内容并写入到本地文件
                    using (FileStream fileStream = new FileStream(savedirectory, FileMode.Create, FileAccess.Write))
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
                    EcMsgBox.Show($"文件接收完成，保存路径：{savedirectory}");
                    //System.Diagnostics.Debug.WriteLine($"文件接收完成，保存路径：{savedirectory}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"文件接收失败：{ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        public void StopListening()
        {
            _tcpListener.Stop();
        }
    }
}
