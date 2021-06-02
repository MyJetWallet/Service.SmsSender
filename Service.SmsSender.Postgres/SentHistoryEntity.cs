using System;
using Service.SmsSender.Domain.Models;

namespace Service.SmsSender.Postgres
{
    public class SentHistoryEntity : SentHistoryRecord
    {
        public int Id { get; set; }

        public SentHistoryEntity(SentHistoryRecord record) : this(record.MaskedPhone, record.Template, record.Provider, record.ProcDate, record.ProcError, record.ClientId)
        {
        }

        public SentHistoryEntity(string maskedPhone, string template, string provider, DateTime procDate, string? procError = null, string? clientId = null)
            : base(maskedPhone, template, provider, procDate, procError, clientId)
        {
        }
    }
}