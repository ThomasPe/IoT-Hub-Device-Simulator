using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTHubDeviceSimulator.IoTDevice
{
    public sealed partial class DeviceEditDialog : ContentDialog
    {

        public Device Device { get; set; }
        public bool IsEditMode { get; set; } = false;

        public DeviceEditDialog()
        {
            InitializeComponent();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsEditMode)
            {
                SecondaryButtonText = string.Empty;
                PrimaryButtonText = "Update";
                Title = "Update IoT Device";
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ErrorTb.Visibility = Visibility.Collapsed;
            if (string.IsNullOrEmpty(ConnectionStringTb.Text))
            {
                args.Cancel = true;
                ErrorTb.Visibility = Visibility.Visible;
            }
            try
            {
                Device = new Device(ConnectionStringTb.Text);
            }
            catch
            {
                args.Cancel = true;
                ErrorTb.Visibility = Visibility.Visible;
            }

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void AddSensorBtn_Click(object sender, RoutedEventArgs e)
        {
            var s = new Sensor()
            {
                Name = $"Sensor {Device.Sensors.Count + 1}",
                Value = "Test",
                Type = "String"
            };
            Device.Sensors.Add(s);
        }

        private void RemoveSensorBtn_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var sensor = b.DataContext as Sensor;
            Device.Sensors.Remove(sensor);
        }

        private void IntervalTb_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }
    }
}
