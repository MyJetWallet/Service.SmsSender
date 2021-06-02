using System;
using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Domain.Models
{
    [DataContract]
    public class SentHistoryRecord
    {
        public SentHistoryRecord(string phone, string template, string provider, DateTime procDate, string? procError = null, string? clientId = null)
        {
            MaskedPhone = phone.Substring(0, Convert.ToInt32(Math.Ceiling(phone.Length / 2m)));
            Template = template;
            Provider = provider;
            ProcDate = procDate;
            ProcError = procError;
            ClientId = clientId;
        }

        public SentHistoryRecord(string phone, TemplateEnum template, string provider, DateTime procDate, string? procError = null, string? clientId = null)
            : this(phone, template.ToString(), provider, procDate, procError, clientId)
        {
        }

        [DataMember(Order = 1)]
        public string MaskedPhone { get; set; }

        [DataMember(Order = 2)]
        public string Template { get; set; }

        [DataMember(Order = 3)]
        public string Provider { get; set; }

        [DataMember(Order = 4)]
        public DateTime ProcDate { get; set; }

        [DataMember(Order = 5)]
        public string? ProcError { get; set; }

        [DataMember(Order = 6)]
        public string? ClientId { get; set; }
    }
}