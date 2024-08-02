using System.Net;
using System.Net.Sockets;

namespace EasyChat.Utilities;

public class NetworkUtilities
{
    public static string GetIp() => GetIps().FirstOrDefault() ?? "127.0.0.1";

    public static List<string> GetIps()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ips = (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).ToList();
        return ips;
    }
}