using System;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Deployer.Services.Services;

namespace Deployer.Services.Classes
{
    public class AutofacConfiguration
    {
        public static void InitializeIoc()
        {
            var builder = new ContainerBuilder();
            RegisterTypes(builder);
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterApiControllers(assemblies);

            builder.RegisterType<CacheManager>().As<ICacheManager>().SingleInstance();
            builder.RegisterType<TeamCityService>().As<ITeamCityService>().InstancePerRequest();
            builder.RegisterType<OctopusService>().As<IOctopusService>().InstancePerRequest();

            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}