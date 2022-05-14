using Server.Services;
using SharedLibrary;

namespace Server.Socket;

public static class ServerSend
{
    public static void Welcome(int toClient, string message)
    {
        using var packet = new Packet((int)ServerPackets.Welcome);
        packet.Write(message);
        packet.Write(toClient);

        SendTcpData(toClient, packet);
    }

    public static void UDPTest(int to)
    {
        using (var packet = new Packet((int)ServerPackets.PlayerHealth))
        {
            packet.Write("a test packet for udp");

            SendUDPData(to, packet);
        }
    }

    private static void SendUDPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        SocketServerService.Clients[toClient].Udp.SendData(packet);
    }
    
    private static void SendUDPDataToAll(int exceptClient,Packet packet)
    {
        packet.WriteLength();
        for (var i = 1; i <= SocketServerService.MaxPlayers; i++)
        {
            if (i != exceptClient) 
                SocketServerService.Clients[i].Udp.SendData(packet);
        }
    }
    
    private static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (var i = 1; i <= SocketServerService.MaxPlayers; i++)
        {
            SocketServerService.Clients[i].Udp.SendData(packet);
        }
    }

    private static void SendTcpData(int toClient, Packet packet)
    {
        packet.WriteLength();
        SocketServerService.Clients[toClient].Tcp.SendData(packet);
    }

    private static void SendTcpDataToAll(int exceptClient,Packet packet)
    {
        packet.WriteLength();
        for (var i = 1; i <= SocketServerService.MaxPlayers; i++)
        {
            if (i != exceptClient) 
                SocketServerService.Clients[i].Tcp.SendData(packet);
        }
    }
    
    private static void SendTcpDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (var i = 1; i <= SocketServerService.MaxPlayers; i++)
        {
            SocketServerService.Clients[i].Tcp.SendData(packet);
        }
    }
}