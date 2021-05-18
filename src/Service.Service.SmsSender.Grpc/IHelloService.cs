using System.ServiceModel;
using System.Threading.Tasks;
using Service.Service.SmsSender.Grpc.Models;

namespace Service.Service.SmsSender.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}