using System.ServiceModel;
using System.Threading.Tasks;
using Service.SmsSender.Grpc.Models.Requests;
using Service.SmsSender.Grpc.Models.Responses;

namespace Service.SmsSender.Grpc
{
    [ServiceContract]
    public interface ISmsTemplateService
    {
        [OperationContract]
        Task<TemplateListResponse> GetAllTemplatesAsync();

        [OperationContract]
        Task<SendResponse> EditTemplateAsync(EditTemplateRequest request);
    }
}
