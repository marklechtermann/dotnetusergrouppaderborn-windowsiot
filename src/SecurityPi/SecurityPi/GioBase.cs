namespace DotNetUserGroupPaderborn
{
    using Windows.Devices.Gpio;

    public abstract class GioBase
    {
        public GpioController controller;
       
        public int PinNumber { get; private set; }

        protected GioBase(int pinNumber)
        {
            this.controller = GpioController.GetDefault();

            this.PinNumber = pinNumber;
        }

        public abstract void Initialize();

        public bool IsAvialable => this.controller != null;
    }
}
