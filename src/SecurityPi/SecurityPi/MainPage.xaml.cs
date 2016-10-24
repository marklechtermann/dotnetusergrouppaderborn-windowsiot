namespace DotNetUserGroupPaderborn
{
    using Windows.Devices.Gpio;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly CameraAccess cameraAccess;

        private readonly SignalRClient signalRClient;

        private GpioButton button0;

        private GpioButton button1;

        private GpioLed led;

        public MainPage()
        {
            this.InitializeComponent();

            this.cameraAccess = new CameraAccess();

            if (GpioController.GetDefault() != null)
            {
                this.textDeviceStatus.Text = "available";
                this.InitializeGpio();
            }
            else
            {
                this.textDeviceStatus.Text = "not available";
            }

            this.signalRClient = new SignalRClient();
            this.signalRClient.PhotoRequested += (sender, e) => { this.cameraAccess.TakePhoto(); };
        }

        public void InitializeGpio()
        {
            this.led = new GpioLed(5);
            this.led.Initialize();
            this.led.BlinkEnable = true;

            this.button0 = new GpioButton(26);
            this.button0.Initialize();
            this.button0.Pressed += (sender, e) => { this.signalRClient.Send("Hello from Pi!"); };

            this.button1 = new GpioButton(22);
            this.button1.Initialize();
            this.button1.Pressed += (sender, e) => { this.signalRClient.Send("takePhoto"); };
        }
    }
}