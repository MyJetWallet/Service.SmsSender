using System.Collections.Generic;
using System.Threading.Tasks;
using Service.SmsSender.Domain.Models.Enums;
using Service.SmsSender.Grpc.Models.Responses;
using Service.SmsSender.Postgres;

namespace Service.SmsSender.Services
{
    public interface ISmsProviderManager
    {
        string[] GetAllProviderNames();

        Task<SendSmsResponse> SendSmsAsync(string phone, string brand, string smsBody, TemplateEnum template);

        Task<IEnumerable<SentHistoryEntity>> GetSentHistoryAsync(int count, int since);
    }
}