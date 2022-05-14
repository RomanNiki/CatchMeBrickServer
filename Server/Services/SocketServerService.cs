using System.Net;
using System.Net.Sockets;
using Server.Services.Interfaces;
using Server.Socket;
using SharedLibrary;

namespace Server.Services;

public sealed class SocketServerService : ISocketServerService
{
    public delegate void PacketHandler(int from, Packet packet);

    public static Dictionary<int, PacketHandler>? PacketHandlers;
    private static bool ServerStarted { get; set; }
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static readonly Dictionary<int, Client> Clients = new();

    private static TcpListener? _tcpListener;
    private static UdpClient? _udpListener;

    public (bool success, string content) Start(int maxPlayers, int port)
    {
        var ip = IPAddress.Any;
        if (ServerStarted)
        {
            return (false, $"{Port}");
        }

        ServerStarted = true;
        MaxPlayers = maxPlayers;
        Port = port;

        Console.WriteLine("Starting room...");
        InitializeServerData();

        _tcpListener = new TcpListener(ip, port);
        _tcpListener.Start();
        _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

        _udpListener = new UdpClient(port);
        _udpListener.BeginReceive(UDPReceiveCallBack, null);

        Console.WriteLine($"Tcp room created on {ip}: {port}");
        return (true, $"Tcp room created on {ip}: {port}");
    }

    private void UDPReceiveCallBack(IAsyncResult result)
    {
        try
        {
            var clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var data = _udpListener.EndReceive(result, ref clientEndPoint);
            _udpListener.BeginReceive(UDPReceiveCallBack, null);

            if (data.Length < 4)
            {
                return;
            }

            using var packet = new Packet(data);
            var clientId = packet.ReadInt();

            if (clientId == 0)
            {
                return;
            }

            if (Clients[clientId].Udp.EndPoint == null)
            {
                Clients[clientId].Udp.Connect(clientEndPoint);
                return;
            }

            if (Clients[clientId].Udp.EndPoint.ToString() == clientEndPoint.ToString())
            {
                Clients[clientId].Udp.HandleData(packet);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void TcpConnectCallback(IAsyncResult result)
    {
        var client = _tcpListener.EndAcceptTcpClient(result);
        _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
        Console.WriteLine($"incoming  connection from {client.Client.RemoteEndPoint}...");

        for (var i = 1; i <= MaxPlayers; i++)
        {
            if (Clients[i].Tcp.Socket == null)
            {
                Clients[i].Tcp.Connect(client);
                return;
            }
        }

        Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: server full!");
    }
    
    public static void SendUDPData(IPEndPoint endPoint, Packet packet)
    {
        try
        {
            if (endPoint != null)
            {
                _udpListener.BeginSend(packet.ToArray(), packet.Length(), endPoint, null, null);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error sending data to {endPoint} via UDP: {e}");
        }
    }

    private static void InitializeServerData()
    {
        for (var i = 1; i <= MaxPlayers; i++)
        {
            Clients.Add(i, new Client(i));
        }

        PacketHandlers = new Dictionary<int, PacketHandler>
        {
            {(int) ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived},
            {(int) ClientPackets.PlayerMovement, ServerHandle.UDPTestReceived}
        };
        Console.WriteLine("Init packets");
    }
}