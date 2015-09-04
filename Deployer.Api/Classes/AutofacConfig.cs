using System;
using System.Web.Http;
using System.Web.Mvc;
using Authority.Deployer.Api.Services;
using Authority.Deployer.Api.Services.Contracts;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;

namespace Authority.Deployer.Api.Classes
{
    public class AutofacConfig
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
            builder.RegisterType<NodeService>().As<INodeService>().InstancePerRequest();

            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}