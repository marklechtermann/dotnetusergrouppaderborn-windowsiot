using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Owin;

[assembly: OwinStartup(typeof(SecurityPiWeb.Startup))]

namespace SecurityPiWeb
{
    using Microsoft.Owin;
    using Owin;
  
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}