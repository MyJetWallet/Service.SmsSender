using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.GrpcMetrics;
using ProtoBuf.Grpc.Client;
using Service.SmsSender.Grpc;

namespace Service.SmsSender.Client
{
    [UsedImplicitly]
    public class SmsSenderClientFactory
    {
        private readonly CallInvoker _channel;

        public SmsSenderClientFactory(string assetsDictionaryGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(assetsDictionaryGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public ISmsService GetSmsService() => _channel.CreateGrpcService<ISmsService>();

        public ISmsDeliveryService GetSmsDeliveryService() => _channel.CreateGrpcService<ISmsDeliveryService>();

        public ISmsProviderService GetSmsProviderService() => _channel.CreateGrpcService<ISmsProviderService>();

        public ISmsTemplateService GetSmsTemplateService() => _channel.CreateGrpcService<ISmsTemplateService>();
    }
}
