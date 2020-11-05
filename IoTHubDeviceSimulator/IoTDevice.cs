using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace IoTHubDeviceSimulator
{
    public class IoTDevice : INotifyPropertyChanged
    {
        private static readonly Random _randomGenerator = new Random();

        private DeviceClient _deviceClient;
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public bool IsRunning { get => _timer.IsEnabled; }

        private DateTime? _lastUpdate;
        public DateTime? LastUpdate 
        { 
            get  => _lastUpdate;
            set
            {
                _lastUpdate = value;
                RaisePropertyChanged(nameof(LastUpdate));
            }
        }

        public IoTDevice(string connectionString)
        {
            ConnectionString = connectionString;
            Name = ConnectionString.Split(';')[1].Replace("DeviceId=", "");
        }

        public void Start()
        {
            if (IsRunning)
                return;
            if (_deviceClient == null)
            {
                _deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString);
            }
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            RaisePropertyChanged(nameof(IsRunning));
        }

        public void Stop()
        {
            if (!IsRunning)
                return;
            _timer.Stop();
            RaisePropertyChanged(nameof(IsRunning));
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

            LastUpdate = DateTime.Now;
        }
    }
}
