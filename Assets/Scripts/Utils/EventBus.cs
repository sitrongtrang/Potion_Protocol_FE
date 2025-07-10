using System;

public static class EventBus
{
    public static Action CraftItem;
    public static void Craft()
    {
        CraftItem?.Invoke();
    }
};