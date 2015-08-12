using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.WebApi;
using DeployerServices.Services;

namespace DeployerServices.Classes
{
    public class AutofacConfiguration
    {
        private static void InitializeIoc()
        {
            var builder = new ContainerBuilder();
            RegisterTypes(builder);
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Register controllers in this Assembly
            builder.RegisterApiControllers(assemblies);

            // Register Umbracos internal controllers
            //builder.RegisterApiControllers(typeof(UmbracoApplication).Assembly);

            // Register DbContext and Unit of Work
            builder.RegisterType<CacheManager>().As<ICacheManager>().SingleInstance();


            // ServiceIds
            builder.RegisterType<TeamCityService>().As<ITeamCityService>().InstancePerRequest();
            builder.RegisterType<OctopusService>().As<IOctopusService>().InstancePerRequest();
          

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacWebApiDependencyResolver(container));
        }
    }
}