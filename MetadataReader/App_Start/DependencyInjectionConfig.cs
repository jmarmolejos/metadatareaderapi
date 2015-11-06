using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
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

            builder.RegisterType<MetadataContext>().As<IMetadataContext>().InstancePerRequest();
            builder.RegisterType<BackgroundJobClient>().As<IBackgroundJobClient>();
            builder.RegisterType<MetadataRepository>().As<IMetadataRepository>();
            builder.RegisterType<PostNotificationSender>().As<IPostNotificationSender>();
            builder.RegisterType<HttpNotificationSender>().As<IHttpNotificationSender>();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}