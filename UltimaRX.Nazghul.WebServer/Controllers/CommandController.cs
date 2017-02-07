using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.SignalR;

namespace UltimaRX.Nazghul.WebServer.Controllers
{
    public class CommandController : ApiController
    {
        public HttpResponseMessage Get(string id)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NazghulHub>();
            context.Clients.All.Say("," + id);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}