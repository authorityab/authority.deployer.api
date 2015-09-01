using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Routing;

namespace Deployer.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            // Web API routes
            config.MapHttpAttributeRoutes();


            //config.Routes.MapHttpRoute(
            //    name: "ApiWithAction",
            //    routeTemplate: "api/{controller}/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "DefaulApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


            

            config.Routes.MapHttpRoute(

                name: "ApiPOST",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, action = "Post" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            config.Routes.MapHttpRoute(
                name: "ApiGET",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );
        }
    }
}
