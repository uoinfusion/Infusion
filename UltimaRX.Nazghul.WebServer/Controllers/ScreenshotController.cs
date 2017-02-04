using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.SignalR;

namespace UltimaRX.Nazghul.WebServer.Controllers
{
    public class ScreenshotController : ApiController
    {
        private static readonly Dictionary<string, byte[]> screenshots = new Dictionary<string, byte[]>();
        private static byte[] image;

        public HttpResponseMessage Get()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new ByteArrayContent(image);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            return result;
        }

        public HttpResponseMessage Post()
        {
            HttpResponseMessage result = null;

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null)
                    {
                        var content = ReadFully(postedFile.InputStream);
                        var screenshotId = Guid.NewGuid().ToString();
                        screenshots[screenshotId] = content;
                        image = content;

                        docfiles.Add(screenshotId);

                        var context = GlobalHost.ConnectionManager.GetHubContext<NazghulHub>();
                        context.Clients.All.ScreenshotReady();
                    }
                }

                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}