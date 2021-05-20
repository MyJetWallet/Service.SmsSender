using System.Threading.Tasks;
using Service.SmsSender.Grpc.Models.Responses;

namespace Service.SmsSender.Services
{
    public interface ISmsProviderManager
    {
        string[] GetAllProviderNames();

        Task<SendSmsResponse> SendSmsAsync(string phone, string smsBody);
    }
}