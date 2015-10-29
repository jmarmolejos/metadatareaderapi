using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using Hangfire;
using MetadataReader.Models;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace MetadataReader
{
    public class DependencyInjectionConfig
    {
        public static void RegisterContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<MetadataContext>().As<IMetadataContext>();
            builder.RegisterType<BackgroundJobClient>().As<IBackgroundJobClient>();

            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
        }
    }
}