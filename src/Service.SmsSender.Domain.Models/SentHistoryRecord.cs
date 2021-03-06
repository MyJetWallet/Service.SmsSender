using System;
using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Domain.Models
{
    [DataContract]
    public class SentHistoryRecord
    {
        public SentHistoryRecord(string phone, string brand, string template, string provider, DateTime procDate, string retryId, MessageStatus status, int retryCount, string? procError = null, string? clientId = null)
        {
            MaskedPhone = phone.Substring(0, Convert.ToInt32(Math.Ceiling(phone.Length / 2m)));
            Brand = brand;
            Template = template;
            Provider = provider;
            ProcDate = procDate;
            ProcError = procError;
            ClientId = clientId;
            RetryId = retryId;
            Status = status;
            RetryCount = retryCount;
        }

        public SentHistoryRecord(string phone, string brand, TemplateEnum template, string provider, DateTime procDate, string retryId, MessageStatus status, int retryCount, string? procError = null, string? clientId = null)
            : this(phone, brand, template.ToString(), provider, procDate, retryId, status, retryCount, procError, clientId)
        {
        }
        
        public SentHistoryRecord(string phone, string brand, string template, string provider, DateTime procDate, string retryId, MessageStatus status, int retryCount, string externalMessageId,string? procError = null, string? clientId = null)
            : this(phone, brand, template, provider, procDate, retryId, status, retryCount, procError, clientId)
        {
            ExternalMessageId = externalMessageId;
        }

        public SentHistoryRecord()
        {
        }

        [DataMember(Order = 1)]
        public string MaskedPhone { get; set; }

        [DataMember(Order = 3)]
        public string Brand { get; set; }

        [DataMember(Order = 4)]
        public string Template { get; set; }

        [DataMember(Order = 5)]
        public string Provider { get; set; }

        [DataMember(Order = 6)]
        public DateTime ProcDate { get; set; }

        [DataMember(Order = 7)]
        public string? ProcError { get; set; }

        [DataMember(Order = 8)]
        public string? ClientId { get; set; }

        [DataMember(Order = 9)]
        public long Id { get; set; }
        
        [DataMember(Order = 10)]
        public string? ExternalMessageId { get; set; }
        [DataMember(Order = 11)]
        public string RetryId { get; set; }
        [DataMember(Order = 12)]
        public int RetryCount { get; set; }
        [DataMember(Order = 13)]
        public MessageStatus Status { get; set; }
        public static SentHistoryRecord Create(SentHistoryRecord record)
        {
            return new SentHistoryRecord(record.MaskedPhone, record.Brand, record.Template, record.Provider,
                record.ProcDate, record.RetryId, record.Status, record.RetryCount, record.ExternalMessageId ?? String.Empty, record.ProcError, record.ClientId)
            {
                MaskedPhone = record.MaskedPhone,
                Id = record.Id
            };
        }
    }
}