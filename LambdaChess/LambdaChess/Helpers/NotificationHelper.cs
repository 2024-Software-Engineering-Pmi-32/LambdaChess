namespace LambdaChess.Helpers;

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

public abstract class NotificationHelper {
    public static void TryDisplayNotification(string message, Control notificationContainer, int horizontalOffset) {
        var newNotification = CreateFlyout(message, horizontalOffset);

        // Attach the new flyout to the control
        FlyoutBase.SetAttachedFlyout(notificationContainer, newNotification);

        // Show the new notification
        FlyoutBase.ShowAttachedFlyout(notificationContainer);

        RemoveNotificationLater(notificationContainer);
    }

    private static async void RemoveNotificationLater(Control notificationContainer, int time = 3000) {
        // Wait for 3 seconds before hiding the toast
        await Task.Delay(time);

        // Hide the toast and remove it from the active list
        FlyoutBase.GetAttachedFlyout(notificationContainer)?.Hide();
    }

    private static Flyout CreateFlyout(string message, int horizontalOffset) {
        var newNotification = new Flyout {
            Placement = PlacementMode.LeftEdgeAlignedBottom,
            HorizontalOffset = -horizontalOffset,
            Content = new Border {
                CornerRadius = new CornerRadius(5),
                Child = new TextBlock {
                    Text = message
                }
            }
        };
        return newNotification;
    }
}