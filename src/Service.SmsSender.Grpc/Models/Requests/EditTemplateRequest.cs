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
        public string Brand { get; set; }

        [DataMember(Order = 3)]
        public string Lang { get; set; }

        [DataMember(Order = 4)]
        public string DefaultBrand { get; set; }

        [DataMember(Order = 5)]
        public string DefaultLang { get; set; }

        [DataMember(Order = 6)]
        public string TemplateBody { get; set; }
    }
}
