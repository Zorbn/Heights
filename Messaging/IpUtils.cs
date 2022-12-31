using System.Net;
using System.Net.Sockets;

namespace Messaging;

public static class IpUtils
{
    public const int DefaultPort = 8052;
    
    public static IPAddress GetIp(string ip)
    {
        IPHostEntry host = Dns.GetHostEntry(ip);
        var addressIndex = 0;
        for (var i = 0; i < host.AddressList.Length; i++)
        {
            IPAddress address = host.AddressList[i];
            if (address.AddressFamily != AddressFamily.InterNetwork) continue;
            addressIndex = i;
            break;
        }

        return host.AddressList[addressIndex];
    }
}