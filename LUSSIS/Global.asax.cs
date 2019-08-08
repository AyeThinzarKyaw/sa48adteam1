using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LUSSIS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings
            //    .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; //By Charles
            //GlobalConfiguration.Configuration.Formatters
            //    .Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter); //By Charles
            //var jsonSerializerSettings = new JsonSerializerSettings
            //{
            //    PreserveReferencesHandling = PreserveReferencesHandling.Objects
            //};
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
            //    = ReferenceLoopHandling.Serialize;
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling
            //    = PreserveReferencesHandling.Objects;
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
