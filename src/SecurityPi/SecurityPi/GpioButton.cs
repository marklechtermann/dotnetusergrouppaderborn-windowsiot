namespace DotNetUserGroupPaderborn
{
    using System;

    using Windows.Devices.Gpio;
    using Windows.UI.Core;

    public class GpioButton : GioBase
    {
        private GpioPin pin;
        private GpioPinValue ledPinValue = GpioPinValue.High;

        public GpioButton(int pinNumber) : base (pinNumber)
        {
         
        }

        public override void Initialize()
        {
            this.pin = this.controller.OpenPin(this.PinNumber);

            if (this.pin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            {
                this.pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            }
            else
            {
                this.pin.SetDriveMode(GpioPinDriveMode.Input);
            }

            this.pin.ValueChanged += this.Pin_ValueChanged;
        }

        private async void Pin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(this.OnButtonPressed));
            }
        }

        private void OnButtonPressed()
        {
            var handler = this.Pressed;
            if (handler != null)
            {
                this.Pressed(this, EventArgs.Empty);
            }
        }

        public event EventHandler Pressed;
    }
}
