using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace EasyChat.Utilities;

public static class WindowUtilities
{
    public static AxisAngleRotation3D? GetAxr(this Window window)
    {
        return window.FindName("Axr") is AxisAngleRotation3D axr ? axr : null;
    }

    public static DoubleAnimation GetAnimation(double target, TimeSpan timeSpan)
    {
        return new DoubleAnimation
        {
            Duration = new Duration(timeSpan),
            To = target
        };
    }
}