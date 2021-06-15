using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Grpc.Models.Requests
{
    [DataContract]
    public class SendVerificationRequest : ISendMessageRequest
    {
        [DataMember(Order = 1)] public string Phone { get; set; }

        [DataMember(Order = 2)] public string Brand { get; set; }

        [DataMember(Order = 3)] public string Lang { get; set; }

        [DataMember(Order = 4)] public string Code { get; set; }
        
        public TemplateEnum Type { get; } = TemplateEnum.Verification;

    }
}