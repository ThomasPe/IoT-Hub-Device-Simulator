using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTHubDeviceSimulator
{
    public sealed partial class DeviceEditDialog : ContentDialog
    {
        public string ConnectionString { get; set; }
        public int MyProperty { get; set; }
        public DeviceEditDialog()
        {
            InitializeComponent();
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
                new IoTDevice(ConnectionStringTb.Text);
                ConnectionString = ConnectionStringTb.Text;
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
    }
}
