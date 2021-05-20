using System.ServiceModel;
using System.Threading.Tasks;
using Service.SmsSender.Domain.Models;
using Service.SmsSender.Grpc.Models.Responses;

namespace Service.SmsSender.Grpc
{
    [ServiceContract]
    public interface ISmsProviderService
    {
        [OperationContract]
        Task<AllProvidersResponse> GetAllProvidersAsync();
        
        [OperationContract]
        Task<AllRoutesResponse> GetAllRoutesAsync();
        
        [OperationContract]
        Task AddOrUpdateRouteAsync(ProviderRoute newRoute);

        [OperationContract]
        Task DeleteRouteAsync(ProviderRoute newRoute);
    }
}