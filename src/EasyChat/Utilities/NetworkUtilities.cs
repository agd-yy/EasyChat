using System.Net;
using System.Net.Sockets;

namespace EasyChat.Utilities;

public class NetworkUtilities
{
    private static readonly string[] AllHost = ["127.0.0.1"];

    public static IList<string> GetIps()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ips =
            (from ip in host.AddressList
                where ip.AddressFamily == AddressFamily.InterNetwork
                select ip.ToString()).Union(AllHost);
        return ips.ToList();
    }
}