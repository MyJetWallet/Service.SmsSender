using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models;

namespace Service.SmsSender.Grpc.Models.Responses
{
    [DataContract]
    public class TemplateListResponse
    {
        [DataMember(Order = 1)]
        public SmsTemplate[] Templates { get; set; }
    }
}