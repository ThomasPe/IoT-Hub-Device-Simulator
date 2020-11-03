using Windows.Storage;

namespace IoTHubDeviceSimulator
{
    public static class SettingsService
    {
        static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static string GetValue(string key)
        {
            return localSettings.Values[key] as string;
        }

        public static void SetValue(string key, string value)
        {
            localSettings.Values[key] = value;
        }
    }
}
