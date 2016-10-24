namespace DotNetUserGroupPaderborn
{
    using System;

    using Microsoft.AspNet.SignalR.Client;

    using Windows.UI.Core;

    public class SignalRClient
    {
        private readonly HubConnection connection;

        private readonly IHubProxy hubProxy;

        public SignalRClient()
        {
            this.connection = new HubConnection(GlobalSettings.Server);
            this.hubProxy = this.connection.CreateHubProxy("PiHub");
            this.hubProxy.On<string>("broadcastMessage", this.MessageReceived);
            this.connection.Start().Wait();
        }

        public event EventHandler PhotoRequested;

        public async void MessageReceived(string message)
        {
            if (message == "takePhoto")
            {
                var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, this.SendPhotoRequested);
            }
        }

        public void Send(string message)
        {
            this.hubProxy.Invoke("send", message);
        }

        private void OnPhotoRequested(object sender, EventArgs e)
        {
            var handler = this.PhotoRequested;
            handler?.Invoke(sender, e);
        }

        private void SendPhotoRequested()
        {
            this.OnPhotoRequested(this, EventArgs.Empty);
        }
    }
}