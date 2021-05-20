using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Grpc.Models.Requests
{
    [DataContract]
    public class EditTemplateRequest
    {
        [DataMember(Order = 1)]
        public TemplateEnum TemplateId { get; set; }

        [DataMember(Order = 2)]
        public LangEnum Lang { get; set; }

        [DataMember(Order = 3)]
        public string TemplateBody { get; set; }
    }
}
