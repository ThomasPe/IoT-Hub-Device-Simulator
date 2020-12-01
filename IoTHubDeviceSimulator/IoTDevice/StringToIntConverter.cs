using System;
using Windows.UI.Xaml.Data;

namespace IoTHubDeviceSimulator.IoTDevice
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return $"{value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var incomming = value as string;
            if (string.IsNullOrWhiteSpace(incomming))
                return 1;
            else
                return int.Parse(incomming);
        }
    }
}
