using MyNoSqlServer.Abstractions;
using Service.SmsSender.Domain.Models;

namespace Service.SmsSender.NoSql
{
    public class ProviderRouteMyNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-sms-provider-route";

        public static string GeneratePartitionKey() => "Routes";

        public static string GenerateRowKey(string routeId) => routeId;

        public ProviderRoute Route { get; set; }

        public static ProviderRouteMyNoSqlEntity Create(ProviderRoute route)
        {
            return new ProviderRouteMyNoSqlEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(route.Id),
                Route = route
            };
        }
    }
}
