using SharedLibrary.Networking;

namespace Server.Socket;

public class SocketServer
{
    public static bool IsStarted { get; set; }

    public SocketServer()
    {
        IsStarted = true;
        var mainThread = new Thread(new ThreadStart(MainThread));
        mainThread.Start();
    }

    private static void MainThread()
    {
        Console.WriteLine($"Main thread started. Running at {Constants.TICK_PER_SEC} tick per second.");
        var nextLoop = DateTime.Now;

        while (IsStarted)
        {
            while (nextLoop < DateTime.Now)
            {
                GameLogic.Update();

                nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                if (nextLoop > DateTime.Now)
                {
                    Thread.Sleep(nextLoop - DateTime.Now);
                }
            }
        }
    }
}