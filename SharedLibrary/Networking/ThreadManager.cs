namespace SharedLibrary.Networking;

public static class ThreadManager
{
    private static readonly List<Action> _executeOnMainThread = new List<Action>();
    private static readonly List<Action> _executeCopiedOnMainThread = new List<Action>();
    private static bool _actionToExecuteOnMainThread;


    /// <summary>Sets an action to be executed on the main thread.</summary>
    /// <param name="action">The action to be executed on the main thread.</param>
    public static string ExecuteOnMainThread(Action action)
    {
        if (action == null)
        {
            return ("No action to execute on main thread!");
        }

        lock (_executeOnMainThread)
        {
            _executeOnMainThread.Add(action);
            _actionToExecuteOnMainThread = true;
        }

        return "good";
    }

    /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
    public static void UpdateMain()
    {
        if (_actionToExecuteOnMainThread)
        {
            _executeCopiedOnMainThread.Clear();
            lock (_executeOnMainThread)
            {
                _executeCopiedOnMainThread.AddRange(_executeOnMainThread);
                _executeOnMainThread.Clear();
                _actionToExecuteOnMainThread = false;
            }

            foreach (var tread in _executeCopiedOnMainThread)
            {
                tread.Invoke();
            }
        }
    }
    
}