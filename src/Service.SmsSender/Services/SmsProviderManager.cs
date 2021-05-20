using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.SmsSender.Client;
using Service.SmsSender.Grpc;
using Service.SmsSender.Grpc.Models.Requests;
using Service.SmsSender.Grpc.Models.Responses;
using Service.SmsSender.NoSql;
using Service.SmsSender.Settings;

namespace Service.SmsSender.Services
{
    public class SmsProviderManager : ISmsProviderManager
    {
        private readonly IMyNoSqlServerDataWriter<ProviderRouteMyNoSqlEntity> _writer;

        private readonly Dictionary<string, ISmsDeliveryService> _serviceImplementations =
            new Dictionary<string, ISmsDeliveryService>();

        public SmsProviderManager(IMyNoSqlServerDataWriter<ProviderRouteMyNoSqlEntity> writer, SettingsModel settingsModel)
        {
            _writer = writer;

            foreach (var (providerName, provider) in settingsModel.SmsProviders)
            {
                var clientFactory = new SmsSenderClientFactory(provider.GrpcUrl);
                _serviceImplementations[providerName] = clientFactory.GetSmsDeliveryService();
            }
        }

        public string[] GetAllProviderNames() => Program.Settings.SmsProviders.Select(p => p.Key).ToArray();

        public async Task<SendSmsResponse> SendSmsAsync(string phone, string smsBody)
        {
            var providerName = await SelectProviderByPhone(phone);

            var provider = GetProviderByName(providerName);

            if (provider == null)
            {
                return new SendSmsResponse
                {
                    Status = false,
                    ErrorMessage = "Provider not found"
                };
            }

            var request = new SendSmsRequest
            {
                Phone = phone,
                Body = smsBody
            };

            var result = await provider.SendSmsAsync(request);

            return result;
        }

        private async Task<string> SelectProviderByPhone(string phone)
        {
            var routes = await _writer.GetAsync();
            phone = phone
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "");

            if (!phone.StartsWith('+'))
            {
                phone = $"+{phone}";
            }

            foreach (var route in routes
                .Where(r => !string.IsNullOrEmpty(r.Route.Pattern))
                .OrderBy(r => r.Route.Order))
            {
                var prefixes = route.Route.Pattern.Split(';');

                if (prefixes.Contains("*"))
                {
                    return route.Route.ProviderId;
                }

                if (prefixes.Any(phone.StartsWith))
                {
                    return route.Route.ProviderId;
                }
            }

            return string.Empty;
        }

        private ISmsDeliveryService GetProviderByName(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                return null;
            }

            if (!_serviceImplementations.TryGetValue(providerName, out var provider))
            {
                return null;
            }

            return provider;
        }
    }
}