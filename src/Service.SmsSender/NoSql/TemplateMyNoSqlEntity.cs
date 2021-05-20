using MyNoSqlServer.Abstractions;
using Service.SmsSender.Domain.Models;

namespace Service.SmsSender.NoSql
{
    public class TemplateMyNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-sms-template";

        public static string GeneratePartitionKey() => "Templates";

        public static string GenerateRowKey(string templateId) => templateId;

        public SmsTemplate Template { get; set; }

        public static TemplateMyNoSqlEntity Create(SmsTemplate template)
        {
            return new TemplateMyNoSqlEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(template.Id.ToString()),
                Template = template
            };
        }
    }
}
