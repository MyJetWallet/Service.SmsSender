using System;
using Service.SmsSender.Domain.Models;

namespace Service.SmsSender.Postgres
{
    public class SentHistoryEntity : SentHistoryRecord
    {
        public int Id { get; set; }

        public SentHistoryEntity(SentHistoryRecord record) : this(record.MaskedPhone, record.Brand, record.Template, record.Provider, record.ProcDate, record.ProcError, record.ClientId)
        {
        }

        public SentHistoryEntity(string maskedPhone, string brand, string template, string provider, DateTime procDate, string? procError = null, string? clientId = null)
            : base(maskedPhone, brand, template, provider, procDate, procError, clientId)
        {
        }
    }
}