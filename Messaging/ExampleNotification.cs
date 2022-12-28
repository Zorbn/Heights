using System;

namespace Messaging;

public static class ExampleNotification
{
    public static void HandleNotification(int fromId, Data data)
    {
        if (data is not ExampleNotificationData notificationData) return;
            
        Console.WriteLine($"Message received: {notificationData.Text}");
    }
}