using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Service.SmsSender.Domain.Models;
using Service.SmsSender.Grpc;
using Service.SmsSender.Grpc.Models.Responses;
using Service.SmsSender.NoSql;

namespace Service.SmsSender.Services
{
    public class SmsProviderService : ISmsProviderService
    {
        private readonly ILogger<SmsProviderService> _logger;
        private readonly ISmsProviderManager _smsProviderManager;
        private readonly IMyNoSqlServerDataWriter<ProviderRouteMyNoSqlEntity> _routeWriter;

        public SmsProviderService(ILogger<SmsProviderService> logger, ISmsProviderManager smsProviderManager, IMyNoSqlServerDataWriter<ProviderRouteMyNoSqlEntity> routeWriter)
        {
            _logger = logger;
            _smsProviderManager = smsProviderManager;
            _routeWriter = routeWriter;
        }

        public Task<AllProvidersResponse> GetAllProvidersAsync()
        {
            var response = new AllProvidersResponse
            {
                Providers = _smsProviderManager.GetAllProviderNames()
            };

            return Task.FromResult(response);
        }

        public async Task<AllRoutesResponse> GetAllRoutesAsync()
        {
            var routes = await _routeWriter.GetAsync();
            var response = new AllRoutesResponse
            {
                Routes = routes.Select(r => r.Route).ToArray()
            };

            return response;
        }

        public async Task AddOrUpdateRouteAsync(ProviderRoute newRoute)
        {
            if (string.IsNullOrEmpty(newRoute.Id))
            {
                newRoute.Id = Guid.NewGuid().ToString("N");
            }

            var entity = ProviderRouteMyNoSqlEntity.Create(newRoute);

            await _routeWriter.InsertOrReplaceAsync(entity);

            _logger.LogInformation("Added/updated provider route {routeJson}", JsonConvert.SerializeObject(newRoute));
        }

        public async Task DeleteRouteAsync(ProviderRoute newRoute)
        {
            var partitionKey = ProviderRouteMyNoSqlEntity.GeneratePartitionKey();
            var rowKey = ProviderRouteMyNoSqlEntity.GenerateRowKey(newRoute.Id);

            await _routeWriter.DeleteAsync(partitionKey, rowKey);

            _logger.LogInformation("Deleted provider route {routeJson}", JsonConvert.SerializeObject(newRoute));
        }
    }
}