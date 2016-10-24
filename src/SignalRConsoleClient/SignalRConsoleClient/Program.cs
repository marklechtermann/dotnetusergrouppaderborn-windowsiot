namespace DotNetUserGroupPaderborn
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Microsoft.AspNet.SignalR.Client;

    class Program
    {
        private static IHubProxy hubProxy;

        public static void LogMessage(string message)
        {
            Console.WriteLine($"Data from SignalR recieved: {message}");
        }

        public static void SendMessage(string message)
        {
            hubProxy.Invoke("send", message);
        }

        static void Main(string[] args)
        {
            var connection = new HubConnection("http://localhost:57624/") { TraceLevel = TraceLevels.All, TraceWriter = Console.Out };

            hubProxy = connection.CreateHubProxy("PiHub");
            hubProxy.On<string>("broadcastMessage", LogMessage);

            try
            {
                connection.Start().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Huch {e.Message}");
            }

            Console.WriteLine("Press Control + C to exit.");

            while (true)
            {
                string input = Console.ReadLine();
                if (input.StartsWith("send"))
                {
                    var fileName = input.Split(' ')[1];
                    SendPhoto(fileName);
                }
                else
                {
                    SendMessage(input);
                }
            }
        }

        public static void SendPhoto(string fileName)
        {
            var client = new HttpClient { BaseAddress = new Uri("http://localhost:57624/") };

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName} not found");
                return;
            }

            using (var stream = File.OpenRead(fileName))
            {
                var multipartFormDataContent = new MultipartFormDataContent();
                var content = new StreamContent(stream);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = "file.jpg" };
                multipartFormDataContent.Add(content);
                client.PostAsync("api/Image", multipartFormDataContent).Wait();
            }
        }
    }
}