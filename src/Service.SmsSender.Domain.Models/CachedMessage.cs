using System;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Domain.Models
{
    public class CachedMessage
    {
        public string RetryId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Phone { get; set; }
        public string Brand { get; set; }
        public string SmsBody { get; set; }
        public TemplateEnum Template { get; set; }
    }
}