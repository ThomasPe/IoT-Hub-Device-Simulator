using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IoTHubDeviceSimulator
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<IoTDevice> Devices { get; set; } = new ObservableCollection<IoTDevice>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetStoredDevices();
            DeviceList.ItemsSource = Devices;
        }

        private void StartAllBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var d in Devices)
            {
                d.Start();
            }
        }

        private void StopAllBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var d in Devices)
            {
                d.Stop();
            }
        }

        private async void AddDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new DeviceEditDialog();
            await dialog.ShowAsync();
            if (!string.IsNullOrEmpty(dialog.ConnectionString))
            {
                var d = new IoTDevice(dialog.ConnectionString);
                Devices.Add(d);
            }
            UpdateStoredDevices();
        }

        private void GetStoredDevices()
        {
            var devicesJson = SettingsService.GetValue("devices");
            if (!string.IsNullOrEmpty(devicesJson))
            {
                var devices = JsonConvert.DeserializeObject<ObservableCollection<IoTDevice>>(devicesJson);
                Devices.Clear();
                foreach (var d in devices)
                {
                    Devices.Add(d);
                }
            }
        }

        private void UpdateStoredDevices()
        {
            SettingsService.SetValue("devices", JsonConvert.SerializeObject(Devices));
        }
    }
}
