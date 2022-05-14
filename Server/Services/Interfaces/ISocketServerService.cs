namespace Server.Services.Interfaces;

public interface ISocketServerService
{
    (bool success, string content) Start(int maxPlayers, int port);
}