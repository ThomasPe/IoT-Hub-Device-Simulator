using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Services.Store;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IoTHubDeviceSimulator
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<IoTDevice> Devices { get; set; } = new ObservableCollection<IoTDevice>();

        public MainPage()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(800, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
                try
                {
                    var d = new IoTDevice(dialog.ConnectionString);
                    Devices.Add(d);
                }
                catch
                {

                }
            }
            UpdateStoredDevices();
        }

        private void DeviceDelete_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            IoTDevice device = b.DataContext as IoTDevice;
            Devices.Remove(device);
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
            RaisePropertyChanged(nameof(Devices));
        }

        private void UpdateStoredDevices()
        {
            RaisePropertyChanged(nameof(Devices));
            SettingsService.SetValue("devices", JsonConvert.SerializeObject(Devices));
        }

        private void DeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeviceList.SelectedIndex = -1;
        }

        private void DeviceSettings_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            IoTDevice device = b.DataContext as IoTDevice;
            Debug.WriteLine(device.Name);
        }

        private void DeviceStop_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            IoTDevice device = b.DataContext as IoTDevice;
            device.Stop();
        }

        private void DeviceStart_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            IoTDevice device = b.DataContext as IoTDevice;
            device.Start();
        }

        private async void GoToGitHub_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://github.com/ThomasPe/IoT-Hub-Device-Simulator");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void RateAppBtn_Click(object sender, RoutedEventArgs e)
        {
            var storeContext = StoreContext.GetDefault();
            StoreRateAndReviewResult result = await storeContext.RequestRateAndReviewAppAsync();
        }
    }
}
