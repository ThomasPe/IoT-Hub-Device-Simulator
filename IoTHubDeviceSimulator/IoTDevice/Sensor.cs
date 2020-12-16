using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Devices.Sensors;

namespace IoTHubDeviceSimulator.IoTDevice
{
    public class Sensor : INotifyPropertyChanged
    {
        private static readonly Random _randomGenerator = new Random();

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }
        private string _type;
        public string Type
        {
            get => _type;
            set
            {
                if(_type != value)
                {
                    _type = value;
                    RaisePropertyChanged(nameof(Type));
                }
            }
        } 

        public double Min { get; set; }
        public double Max { get; set; }
        public string Value { get; set; }
        public bool IsApplicationProperty { get; set; } = false;


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public KeyValuePair<string, object> GetReading()
        {
            try
            {
                switch (Type)
                {
                    case "String":
                        return new KeyValuePair<string, object>(Name, Value);
                    case "Double":
                        var x = _randomGenerator.NextDouble() * (Max - Min) + Min;
                        return new KeyValuePair<string, object>(Name, x);
                    case "Integer":
                        var y = _randomGenerator.NextDouble() * (Max - Min) + Min;
                        return new KeyValuePair<string, object>(Name, Convert.ToInt32(y));
                    default:
                        return new KeyValuePair<string, object>(Name, "error");
                }
            }
            catch
            {
                return new KeyValuePair<string, object>(Name, "Sensor Error");
            }
        }
    }


}

