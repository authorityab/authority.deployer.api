using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Routing;

namespace DeployerServices
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            


            config.Routes.MapHttpRoute(
                name: "ApiGET",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, action = "Get", httpMethod = new HttpMethodConstraint("GET")}
            );

            config.Routes.MapHttpRoute(

                name: "ApiPOST",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, action = "Post", httpMethod = new HttpMethodConstraint("POST") }
            );
        }
    }
}
