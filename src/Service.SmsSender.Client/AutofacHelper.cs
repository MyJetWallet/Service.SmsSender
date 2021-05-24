using Autofac;
using Service.SmsSender.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.SmsSender.Client
{
    public static class AutofacHelper
    {
        public static void RegisterSmsSenderClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new SmsSenderClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetSmsService()).As<ISmsService>().SingleInstance();
        }

        public static void RegisterSmsSenderAdminClients(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new SmsSenderClientFactory(grpcServiceUrl);
            
            builder.RegisterInstance(factory.GetSmsProviderService()).As<ISmsProviderService>().SingleInstance();
            builder.RegisterInstance(factory.GetSmsTemplateService()).As<ISmsTemplateService>().SingleInstance();
        }
    }
}
