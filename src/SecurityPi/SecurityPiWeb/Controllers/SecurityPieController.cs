namespace SecurityPiWeb.Controllers
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    using Microsoft.AspNet.SignalR;

    using SecurityPiWeb.Hubs;

    [RoutePrefix("api/Image")]
    public class SecurityPiController : ApiController
    {
        public SecurityPiController()
        {
            var app = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            if (!Directory.Exists(app))
            {
                Directory.CreateDirectory(app);
            }
        }

        [Route("")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var file = this.FileName;
            HttpResponseMessage result;
            if (File.Exists(file))
            {
                using (var img = Image.FromFile(file))
                {
                    using (var ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(ms.ToArray()) };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    }
                }
            }
            else
            {
                result = new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            return result;
        }

        [Route("")]
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData fileData in provider.FileData)
                {
                    if (File.Exists(this.FileName))
                    {
                        File.Delete(this.FileName);
                    }

                    File.Move(fileData.LocalFileName, this.FileName);
                }

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<PiHub>();
                hubContext.Clients.All.broadcastMessage("newPhoto");

                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private string FileName => Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "image.jpg");
    }
}