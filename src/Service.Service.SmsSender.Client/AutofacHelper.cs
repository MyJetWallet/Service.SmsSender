using Autofac;
using Service.Service.SmsSender.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.Service.SmsSender.Client
{
    public static class AutofacHelper
    {
        public static void RegisterService.SmsSenderClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new Service.SmsSenderClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
