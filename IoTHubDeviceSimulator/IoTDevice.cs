using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace IoTHubDeviceSimulator
{
    public class IoTDevice
    {
        private static readonly Random _randomGenerator = new Random();

        private DeviceClient _deviceClient;
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public string Name { get; set; }
        public string ConnectionString { get; set; } = "HostName=IoTHubDeviceSimulator.azure-devices.net;DeviceId=Device1;SharedAccessKey=yz/QG7F8csKmYNDvfjSTEFTNA6gT3pFLuPtQC5j8u/M=";
        public bool IsRunnuing { get => _timer.IsEnabled; }

        public IoTDevice(string connectionString)
        {
            ConnectionString = connectionString;
            Name = ConnectionString.Split(';')[1].Replace("DeviceId=", "");
        }

        public void Start()
        {
            if (_deviceClient == null)
            {
                _deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString);
            }
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick;
        }

        private async void Timer_Tick(object sender, object e)
        {
            await SendEventAsync();
        }

        private async Task SendEventAsync()
        {
            float temperature = _randomGenerator.Next(20, 35);
            float humidity = _randomGenerator.Next(60, 80);

            string dataBuffer = $"{{\"device\":\"{Name}\",\"temperature\":{temperature},\"humidity\":{humidity}}}";

            using var eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer))
            {
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8.ToString(),
            };

            //const int TemperatureThreshold = 30;
            //bool tempAlert = temperature > TemperatureThreshold;
            //eventMessage.Properties.Add("temperatureAlert", tempAlert.ToString());

            await _deviceClient.SendEventAsync(eventMessage);

        }
    }
}
