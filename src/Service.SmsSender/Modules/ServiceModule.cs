using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;
using MyServiceBus.Abstractions;
using Service.SmsSender.Domain.Models;
using Service.SmsSender.Jobs;
using Service.SmsSender.NoSql;
using Service.SmsSender.Services;

namespace Service.SmsSender.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            
            var serviceBus = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(t => t.SpotServiceBusHostPort),
                Program.LogFactory);
            var queue = $"SmsSender";
            
            builder.RegisterMyServiceBusSubscriberSingle<SmsDeliveryMessage>(serviceBus,
                SmsDeliveryMessage.TopicName, queue, TopicQueueType.PermanentWithSingleConnection);
            
            
            builder
                .RegisterType<SmsProviderManager>()
                .As<ISmsProviderManager>()
                .AutoActivate()
                .SingleInstance();

            builder
                .RegisterType<SmsTemplateService>()
                .AsSelf()
                .SingleInstance();
            
            builder
                .RegisterType<MessageDeliveryJob>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();

            var providerRouterWriter = new MyNoSqlServerDataWriter<ProviderRouteMyNoSqlEntity>(Program.ReloadedSettings(s => s.MyNoSqlWriterUrl), ProviderRouteMyNoSqlEntity.TableName, true);
            builder
                .RegisterInstance(providerRouterWriter)
                .As<IMyNoSqlServerDataWriter<ProviderRouteMyNoSqlEntity>>()
                .SingleInstance();

            var templateWriter = new MyNoSqlServerDataWriter<TemplateMyNoSqlEntity>(Program.ReloadedSettings(s => s.MyNoSqlWriterUrl), TemplateMyNoSqlEntity.TableName, true);
            builder
                .RegisterInstance(templateWriter)
                .As<IMyNoSqlServerDataWriter<TemplateMyNoSqlEntity>>()
                .SingleInstance();
        }
    }
}