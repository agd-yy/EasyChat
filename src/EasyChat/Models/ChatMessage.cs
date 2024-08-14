using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace EasyChat.Models
{
    public class ChatMessage 
    {
        public string NickName { get; set; }
        public string Image { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public bool IsMyMessage { get; set; }
        public string SeparatorTitle { get; set; }
        public string Color { get; set; }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
