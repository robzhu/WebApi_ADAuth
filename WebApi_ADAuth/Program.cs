using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace WebApi_ADAuth
{
    public class Startup
    {
        public void Configuration( IAppBuilder app )
        {
            HttpListener listener = (HttpListener)app.Properties[ "System.Net.HttpListener" ];
            listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;

            HttpConfiguration config = new HttpConfiguration();

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault( t => t.MediaType == "application/xml" );
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove( appXmlType );

            config.Routes.MapHttpRoute( "api", "{controller}" );
            app.UseWebApi( config );
        }
    }

    class Program
    {
        static void Main( string[] args )
        {
            var url = "http://*:9000";
            using( WebApp.Start( url ) )
            {
                Console.WriteLine( "Service started at {0}", url );
                Console.ReadLine();
            }
        }
    }

    [Authorize]
    public class HelloController : ApiController
    {
        public IHttpActionResult Get()
        {
            WindowsIdentity id = User.Identity as WindowsIdentity;
            if( id == null ) return Unauthorized();

            return Ok( "Hello, " + id.Name );
        }
    }
}
