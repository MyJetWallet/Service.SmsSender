using System.ServiceModel;
using System.Threading.Tasks;
using Service.SmsSender.Grpc.Models.Requests;
using Service.SmsSender.Grpc.Models.Responses;

namespace Service.SmsSender.Grpc
{
    [ServiceContract]
    public interface ISmsDeliveryService
    {
        [OperationContract]
        Task<SendSmsResponse> SendSmsAsync(SendSmsRequest request);
    }
}
