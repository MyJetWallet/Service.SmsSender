using System.ServiceModel;
using System.Threading.Tasks;
using Service.SmsSender.Grpc.Models.Requests;
using Service.SmsSender.Grpc.Models.Responses;

namespace Service.SmsSender.Grpc
{
    [ServiceContract]
    public interface ISmsService
    {
        [OperationContract]
        Task<SendResponse> SendLogInSuccessAsync(SendLogInSuccessRequest request);
        
        [OperationContract]
        Task<SendResponse> SendTradeMadeAsync(SendTradeMadeRequest request);
    }
}