using System.Globalization;
using System.Windows.Data;

namespace EasyChat.Converters;

internal class StringToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str) return false;
        return !string.IsNullOrEmpty(str);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}