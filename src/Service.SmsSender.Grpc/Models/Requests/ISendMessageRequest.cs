using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Grpc.Models.Requests
{
    public interface ISendMessageRequest
    {
        string Phone { get; set; }

        string Brand { get; set; }

        string Lang { get; set; }
        
        TemplateEnum Type { get; }
    }
}
