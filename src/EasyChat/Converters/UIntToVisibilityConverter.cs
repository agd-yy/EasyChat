using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EasyChat.Converters;

public class UIntToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not uint intValue) return Visibility.Collapsed;
        return intValue == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}