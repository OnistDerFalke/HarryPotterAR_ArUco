using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static event Action<string> OnMarkDetected;
    public static void InvokeOnMarkerDetected(string id)
    {
        OnMarkDetected?.Invoke(id);
    }

    public static event Action<string> OnMarkLost;
    public static void InvokeOnMarkerLost(string id)
    {
        OnMarkLost?.Invoke(id);
    }
}
