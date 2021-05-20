using System.Threading.Tasks;
using Service.SmsSender.Grpc.Models.Requests;
using Service.SmsSender.Grpc.Models.Responses;

namespace Service.SmsSender.Grpc
{
    public interface ISmsDeliveryService
    {
        Task<SendSmsResponse> SendSmsAsync(SendSmsRequest request);
    }
}
