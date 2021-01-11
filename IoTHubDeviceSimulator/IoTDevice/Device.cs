using IoTHubDeviceSimulator.Services;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace IoTHubDeviceSimulator.IoTDevice
{
    public class Device : INotifyPropertyChanged
    {
        private static readonly Random _randomGenerator = new Random();
        private readonly ILogger _logger;

        private DeviceClient _deviceClient;
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string _connectionString;
        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value;
                SetNameFromConnectionString();
                RaisePropertyChanged(nameof(ConnectionString));
            }
        }

        public bool IsRunning { get => _timer.IsEnabled; }

        private DateTime? _lastUpdate;
        public DateTime? LastUpdate
        {
            get => _lastUpdate;
            set
            {
                _lastUpdate = value;
                RaisePropertyChanged(nameof(LastUpdate));
            }
        }

        public ObservableCollection<Sensor> Sensors { get; set; } = new ObservableCollection<Sensor>();

        public Device(string connectionString)
        {
            _logger = (ILogger)ServiceProvider.Container.GetService(typeof(ILogger<Device>));

            ConnectionString = connectionString;
            SetNameFromConnectionString();
            Interval = 5;
        }

        private void SetNameFromConnectionString(bool force = false)
        {
            if (string.IsNullOrEmpty(Name) || force)
            {
                Name = ConnectionString.Split(';')[1].Replace("DeviceId=", "");
            }
        }

        private int _interval;
        public int Interval
        {
            get { return _interval; }
            set
            {
                if (value < 0)
                    return;

                _interval = value;
                RaisePropertyChanged(nameof(Interval));
            }
        }

        public string MessageTemplate { get; set; }
        public bool IsMessageTemplateEnabled { get; set; } = false;

        public void Start()
        {
            if (IsRunning)
                return;
            if (_deviceClient == null)
            {
                _deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString);
            }
            _timer.Interval = TimeSpan.FromSeconds(Interval);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            RaisePropertyChanged(nameof(IsRunning));

            _logger.LogInformation($"Started device {Name}");
        }

        public void Stop()
        {
            if (!IsRunning)
                return;
            _timer.Stop();
            RaisePropertyChanged(nameof(IsRunning));
            _timer.Tick -= Timer_Tick;

            _logger.LogInformation($"Stopped device {Name}");
        }

        private async void Timer_Tick(object sender, object e)
        {
            await SendEventAsync();
        }

        private async Task SendEventAsync()
        {

            var msg = new Dictionary<string, object>
            {
                { "name", Name }
            };

            // Add sensor data to message body
            Sensors.Where(x => !x.IsApplicationProperty).ToList().ForEach(sensor =>
            {
                var reading = sensor.GetReading();
                msg.Add(reading.Key, reading.Value);
            });

            string dataBuffer;
            if (!IsMessageTemplateEnabled)
            {
                dataBuffer = JsonConvert.SerializeObject(msg);
            }
            else
            {
                dataBuffer = MessageTemplate;
                foreach(var m in msg)
                {
                    dataBuffer = dataBuffer.Replace($"[{m.Key}]", m.Value.ToString());
                }
            }

            using var eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer))
            {
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8.ToString(),
            };

            // Add sensor data to message application properties
            Sensors.Where(x => x.IsApplicationProperty).ToList().ForEach(sensor =>
            {
                var reading = sensor.GetReading();
                eventMessage.Properties.Add(reading.Key, reading.Value.ToString());
            });

            await _deviceClient.SendEventAsync(eventMessage);

            _logger.LogInformation($"Device {Name} sent data: {dataBuffer}");

            LastUpdate = DateTime.Now;
        }
    }
}
