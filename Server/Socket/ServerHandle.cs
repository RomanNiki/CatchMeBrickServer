using Server.Services;
using SharedLibrary;

namespace Server.Socket;

public static class ServerHandle
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        var clientIdCheck = packet.ReadInt();
        var username = packet.ReadString();

        Console.WriteLine($"{SocketServerService.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
        if (fromClient != clientIdCheck)
        {
            Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
        }
        // TODO: send player into game
    }

    public static void UDPTestReceived(int from, Packet packet)
    {
        var message = packet.ReadString();
        Console.WriteLine($"Received test packet for udp: {message}");
    }
}