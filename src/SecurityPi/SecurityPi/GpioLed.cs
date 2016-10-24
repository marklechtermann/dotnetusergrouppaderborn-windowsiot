namespace DotNetUserGroupPaderborn
{
    using System;

    using Windows.Devices.Gpio;
    using Windows.UI.Xaml;

    public class GpioLed : GioBase
    {
        private bool blinkEnable;

        private int blinkSpeed = 100;

        private bool blinkTimerState = true;

        private GpioPin pin;

        private DispatcherTimer timer = new DispatcherTimer();

        public GpioLed(int pinNumber)
            : base(pinNumber)
        {
        }

        public bool BlinkEnable
        {
            get
            {
                return this.blinkEnable;
            }

            set
            {
                if (this.blinkEnable != value)
                {
                    this.blinkEnable = value;
                    this.UpdateTimer();
                }
            }
        }

        public int BlinkSpeed
        {
            get
            {
                return this.blinkSpeed;
            }

            set
            {
                this.blinkSpeed = value;

                if (this.timer != null)
                {
                    this.timer.Interval = TimeSpan.FromMilliseconds(this.BlinkSpeed);
                }
            }
        }

        public override void Initialize()
        {
            this.pin = this.controller.OpenPin(this.PinNumber);
            this.pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void Off()
        {
            if (this.IsAvialable)
            {
                this.pin.Write(GpioPinValue.High);
            }
        }

        public void On()
        {
            if (this.IsAvialable)
            {
                this.pin.Write(GpioPinValue.Low);
            }
        }

        private void UpdateTimer()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer = null;
            }

            if (!this.BlinkEnable)
            {
                return;
            }

            this.timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(this.BlinkSpeed) };

            this.timer.Tick += (sender, t) =>
                {
                    if (this.blinkTimerState)
                    {
                        this.On();
                    }
                    else
                    {
                        this.Off();
                    }

                    this.blinkTimerState = !this.blinkTimerState;
                };

            this.timer.Start();
        }
    }
}