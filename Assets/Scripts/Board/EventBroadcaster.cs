using System;

public static class EventBroadcaster
{
    public static event Action<int> OnBoardDetected;
    public static void InvokeOnBoardDetected(int id)
    {
        OnBoardDetected?.Invoke(id);
    }

    public static event Action<int> OnBoardLost;
    public static void InvokeOnBoardLost(int id)
    {
        OnBoardLost?.Invoke(id);
    }

    public static event Action<int> OnMarkDetected;
    public static void InvokeOnMarkerDetected(int id)
    {
        OnMarkDetected?.Invoke(id);
    }

    public static event Action<int> OnMarkLost;
    public static void InvokeOnMarkerLost(int id)
    {
        OnMarkLost?.Invoke(id);
    }
}
