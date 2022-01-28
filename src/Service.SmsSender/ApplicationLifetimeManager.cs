using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.SmsSender.Services;

namespace Service.SmsSender
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly SmsTemplateService _templateService;
        private readonly ServiceBusLifeTime _serviceBusLifeTime;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger, SmsTemplateService templateService, ServiceBusLifeTime serviceBusLifeTime)
            : base(appLifetime)
        {
            _logger = logger;
            _templateService = templateService;
            _serviceBusLifeTime = serviceBusLifeTime;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _templateService.CreateDefaultTemplatesAsync().GetAwaiter().GetResult();
            _serviceBusLifeTime.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _serviceBusLifeTime.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
