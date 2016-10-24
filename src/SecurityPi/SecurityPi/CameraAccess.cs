namespace DotNetUserGroupPaderborn
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.Storage;
    using Windows.UI.Xaml.Media.Imaging;

    public class CameraAccess
    {
        private readonly Uri webServiceUrl = new Uri(GlobalSettings.Server);
        private readonly Random random = new Random();
        private readonly HttpClient client;
        private MediaCapture mediaCapture;
        private bool blocked = false;
        private uint width = 600;
        private uint height = 480;

        public CameraAccess()
        {
            this.client = new HttpClient { BaseAddress = this.webServiceUrl };
        }

        public void TakePhoto()
        {
            if (!this.blocked)
            {
                this.blocked = true;
                this.TakePhotoAsync().GetAwaiter().OnCompleted(() => { this.blocked = false; });
            }
        }

        private async Task TakePhotoAsync()
        {
            this.mediaCapture = new MediaCapture();
            await this.mediaCapture.InitializeAsync();

            var imgFormat = ImageEncodingProperties.CreateJpeg();
            imgFormat.Height = this.height;
            imgFormat.Width = this.width;

            var files = ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (var filex in await files.AsTask())
            {
                filex.DeleteAsync().GetAwaiter();
            }

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"{this.random.Next().ToString()}picture.jpg", CreationCollisionOption.GenerateUniqueName);
            await this.mediaCapture.CapturePhotoToStorageFileAsync(imgFormat, file);
            var bmpImage = new BitmapImage(new Uri(file.Path));
            await this.UploadImage(file);
        }

        private async Task<HttpResponseMessage> UploadImage(StorageFile file)
        {
            var stream = await file.OpenStreamForReadAsync();
            var multipartFormDataContent = new MultipartFormDataContent();
            var content = new StreamContent(stream);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = "file.jpg" };
            multipartFormDataContent.Add(content);
            return await this.client.PostAsync("api/Image", multipartFormDataContent);
        }
    }
}
