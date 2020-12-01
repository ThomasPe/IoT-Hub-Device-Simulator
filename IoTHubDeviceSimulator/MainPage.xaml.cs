using IoTHubDeviceSimulator.IoTDevice;
using IoTHubDeviceSimulator.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Foundation;
using Windows.Services.Store;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IoTHubDeviceSimulator
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private readonly ILogger _logger;

        public ObservableCollection<Device> Devices { get; set; } = new ObservableCollection<Device>();

        public ObservableCollection<string> Logs { get; private set; } = new ObservableCollection<string>();

        public MainPage()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(800, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            _logger = (ILogger)ServiceProvider.Container.GetService(typeof(ILogger<MainPage>));
            ActionSink.OnLog += (message) => Logs.Add(message);
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

            _logger.LogInformation($"Loaded {Devices.Count} devices.");
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
            if (dialog.Device != null)
            {
                Devices.Add(dialog.Device);
            }
            UpdateStoredDevices();
        }

        private void DeviceDelete_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            Device device = b.DataContext as Device;
            Devices.Remove(device);
            UpdateStoredDevices();
        }

        private void GetStoredDevices()
        {
            var devicesJson = SettingsService.GetValue("devices");
            if (!string.IsNullOrEmpty(devicesJson))
            {
                var devices = JsonConvert.DeserializeObject<ObservableCollection<Device>>(devicesJson);
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

        private async void DeviceSettings_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            Device device = b.DataContext as Device;

            var dialog = new DeviceEditDialog
            {
                Device = device,
                IsEditMode = true
            };
            await dialog.ShowAsync();
            UpdateStoredDevices();
        }

        private void DeviceStop_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            Device device = b.DataContext as Device;
            device.Stop();
        }

        private void DeviceStart_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            Device device = b.DataContext as Device;
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
