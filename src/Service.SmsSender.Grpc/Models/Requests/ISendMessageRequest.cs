using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Grpc.Models.Requests
{
    public interface ISendMessageRequest
    {
        string Phone { get; set; }

        string Brand { get; set; }

        LangEnum Lang { get; set; }
    }
}
