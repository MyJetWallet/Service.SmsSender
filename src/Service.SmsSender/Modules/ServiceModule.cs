using Autofac;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;
using Service.SmsSender.NoSql;
using Service.SmsSender.Services;

namespace Service.SmsSender.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<SmsProviderManager>()
                .As<ISmsProviderManager>()
                .AutoActivate()
                .SingleInstance();

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