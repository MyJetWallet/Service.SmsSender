using System.Collections.Generic;
using System.Threading.Tasks;
using Service.SmsSender.Domain.Models;
using Service.SmsSender.Domain.Models.Enums;
using Service.SmsSender.Grpc.Models.Responses;
using Service.SmsSender.Postgres;

namespace Service.SmsSender.Services
{
    public interface ISmsProviderManager
    {
        string[] GetAllProviderNames();

        Task<SendSmsResponse> SendSmsAsync(string phone, string brand, string smsBody, TemplateEnum template, string retryId = null, int retryCount = 0);

        Task<List<SentHistoryRecord>> GetSentHistoryAsync(int count, int since);
    }
}