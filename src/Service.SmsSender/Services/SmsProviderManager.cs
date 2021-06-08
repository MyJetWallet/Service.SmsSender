using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.SmsSender.Client;
using Service.SmsSender.Domain.Models;
using Service.SmsSender.Domain.Models.Enums;
using Service.SmsSender.Grpc;
using Service.SmsSender.Grpc.Models.Requests;
using Service.SmsSender.Grpc.Models.Responses;
using Service.SmsSender.NoSql;
using Service.SmsSender.Postgres;
using Service.SmsSender.Settings;

namespace Service.SmsSender.Services
{
    public class SmsProviderManager : ISmsProviderManager
    {
        private readonly ILogger<SmsProviderManager> _logger;
        private readonly IMyNoSqlServerDataWriter<ProviderRouteMyNoSqlEntity> _writer;
        private readonly DbContextOptionsBuilder<SmsSenderDbContext> _dbContextOptionsBuilder;

        private readonly Dictionary<string, ISmsDeliveryService> _serviceImplementations =
            new Dictionary<string, ISmsDeliveryService>();

        public SmsProviderManager(ILogger<SmsProviderManager> logger,
            SettingsModel settingsModel,
            IMyNoSqlServerDataWriter<ProviderRouteMyNoSqlEntity> writer,
            DbContextOptionsBuilder<SmsSenderDbContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _writer = writer;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;

            foreach (var (providerName, provider) in settingsModel.SmsProviders)
            {
                var clientFactory = new SmsSenderClientFactory(provider.GrpcUrl);
                _serviceImplementations[providerName] = clientFactory.GetSmsDeliveryService();
            }
        }

        public string[] GetAllProviderNames() => Program.Settings.SmsProviders.Select(p => p.Key).ToArray();

        public async Task<SendSmsResponse> SendSmsAsync(string phone, string brand, string smsBody, TemplateEnum template)
        {
            var providerName = await SelectProviderByPhone(phone);
            var provider = GetProviderByName(providerName);
            var response = new SendSmsResponse();

            if (provider != null)
            {
                var request = new SendSmsRequest
                {
                    Phone = phone,
                    Body = smsBody
                };

                var record = new SentHistoryRecord(phone, brand, template, providerName, DateTime.Now);

                response = await provider.SendSmsAsync(request);

                if (!response.Status)
                {
                    record.ProcError = response.ErrorMessage;
                }
                else if (!string.IsNullOrEmpty(response.ReturnedId)) // threat ID returned from provider as ClientID
                {
                    record.ClientId = response.ReturnedId;
                }

                response.ReturnedId = await SaveSentHistoryRecord(record); // fill ReturnedID field with stored record ID value
            }
            else
            {
                response.Status = false;
                response.ErrorMessage = "Provider not found";
            }

            return response;
        }

        public async Task<IEnumerable<SentHistoryEntity>> GetSentHistoryAsync(int count)
        {
            try
            {
                await using var context = new SmsSenderDbContext(_dbContextOptionsBuilder.Options);
                return context.SentHistory.AsEnumerable().TakeLast(count);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot retrieve sms history records from DB");
                throw;
            }
        }

        #region Private methods

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

        private async Task<string> SaveSentHistoryRecord(SentHistoryRecord record)
        {
            try
            {
                await using var context = new SmsSenderDbContext(_dbContextOptionsBuilder.Options);
                var dbResult = await context.SentHistory.AddAsync(new SentHistoryEntity(record));
                await context.SaveChangesAsync();
                return dbResult.Entity.Id.ToString();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot store sms history record for {maskedPhone} (provider: {provider})", record.MaskedPhone, record.Provider);
                throw;
            }
        }

        #endregion
    }
}