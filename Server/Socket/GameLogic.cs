using SharedLibrary.Networking;

namespace Server.Socket;

public static class GameLogic
{
    public static void Update()
    {
        ThreadManager.UpdateMain();
    }
}