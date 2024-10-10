using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace EasyChat.Utilities;

public class NetworkUtilities
{
    private static readonly string[] AllHost = ["127.0.0.1"];
    private static List<string> activeIPs = new List<string>(); // 存储活动IP地址

    public static IList<string> GetIps()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ips =
            (from ip in host.AddressList
                where ip.AddressFamily == AddressFamily.InterNetwork
                select ip.ToString()).Union(AllHost);
        return ips.ToList();
    }

    /// <summary>
    /// 扫描本地网络,会干爆内存，慎用
    /// </summary>
    /// <param name="localIP"></param>
    public static async Task ScanLocalNetwork(string localIP)
    {
        string subnetMask = GetSubnetMask(localIP);

        if (string.IsNullOrEmpty(localIP) || string.IsNullOrEmpty(subnetMask))
        {
            return;
        }
        // 获取扫描范围的IP数量
        int ipCount = GetIPCount(subnetMask);
        if (ipCount > 2000)
        {
            return ;
        }
        // 获取子网的网络地址
        string networkAddress = GetNetworkAddress(localIP, subnetMask);

        List<Task> pingTasks = new List<Task>();

        for (int i = 1; i < ipCount; i++) // 从 1 开始，避免扫描网络地址
        {
            string pingIP = IncrementIPAddress(networkAddress, i); // 生成要Ping的IP地址

            // 将 Ping 操作放入 Task 列表，并发执行
            pingTasks.Add(PingIPAddressAsync(pingIP));
        }

        // 等待所有 Ping 操作完成
        await Task.WhenAll(pingTasks);
    }

    #region private 方法
    private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(20);
    private static async Task PingIPAddressAsync(string ipAddress)
    {
        await semaphoreSlim.WaitAsync(); // 控制并发量
        using (Ping ping = new Ping())
        {
            try
            {
                // 同步等待 Ping 结果，超时时间 1000ms
                PingReply reply = await ping.SendPingAsync(ipAddress, 1000);

                if (reply.Status == IPStatus.Success)
                {
                    lock (activeIPs) // 加锁避免多线程操作冲突
                    {
                        activeIPs.Add(ipAddress);
                        System.Diagnostics.Debug.WriteLine(ipAddress);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                semaphoreSlim.Release(); // 释放信号量
            }
        }
    }

    /// <summary>
    /// 获取本地IP的子网掩码
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    private static string GetSubnetMask(string ipAddress)
    {
        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (var ip in ni.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.ToString() == ipAddress)
                {
                    return ip.IPv4Mask.ToString();
                }
            }
        }
        return "";
    }

    /// <summary>
    /// 计算子网内的IP数量
    /// </summary>
    /// <param name="subnetMask"></param>
    /// <returns></returns>
    private static int GetIPCount(string subnetMask)
    {
        var maskBytes = IPAddress.Parse(subnetMask).GetAddressBytes();
        int zeroCount = 0;

        foreach (var b in maskBytes)
        {
            zeroCount += Convert.ToString(b, 2).PadLeft(8, '0').Count(c => c == '0');
        }

        return (int)Math.Pow(2, zeroCount);
    }

    /// <summary>
    /// 获取网络地址
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="subnetMask"></param>
    /// <returns></returns>
    private static string GetNetworkAddress(string ipAddress, string subnetMask)
    {
        byte[] ipBytes = IPAddress.Parse(ipAddress).GetAddressBytes();
        byte[] maskBytes = IPAddress.Parse(subnetMask).GetAddressBytes();

        byte[] networkAddressBytes = new byte[ipBytes.Length];
        for (int i = 0; i < ipBytes.Length; i++)
        {
            networkAddressBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }

        return new IPAddress(networkAddressBytes).ToString();
    }

    /// <summary>
    /// 生成递增的IP地址
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="increment"></param>
    /// <returns></returns>
    private static string IncrementIPAddress(string ipAddress, int increment)
    {
        byte[] ipBytes = IPAddress.Parse(ipAddress).GetAddressBytes();
        for (int i = ipBytes.Length - 1; i >= 0; i--)
        {
            int value = ipBytes[i] + increment;
            ipBytes[i] = (byte)(value % 256);
            increment = value / 256;
        }
        return new IPAddress(ipBytes).ToString();
    }
    #endregion
}