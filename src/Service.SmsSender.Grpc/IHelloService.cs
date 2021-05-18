using System.ServiceModel;
using System.Threading.Tasks;
using Service.SmsSender.Grpc.Models;

namespace Service.SmsSender.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}