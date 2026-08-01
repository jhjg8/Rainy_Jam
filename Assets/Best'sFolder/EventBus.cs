using System;

public static class EventBus<T> where T : struct
{
    public static event Action<T> OnEvent;

    public static void Raise(T eventArgs)
    {
        OnEvent?.Invoke(eventArgs);
    }

    public static void Register(Action<T> handler)
    {
        OnEvent += handler;
    }

    public static void Unregister(Action<T> handler)
    {
        OnEvent -= handler;
    }
}