using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Domain.Models
{
    [DataContract]
    public class SmsTemplate
    {
        [DataMember(Order = 1)]
        public TemplateEnum Id { get; set; }

        [DataMember(Order = 2)]
        public string Brand { get; set; }

        [DataMember(Order = 3)]
        public LangEnum DefaultLang { get; set; }

        [DataMember(Order = 4)]
        public Dictionary<string, string> LangBodies { get; set; }

        [DataMember(Order = 5)]
        public List<string> Params { get; set; }
    }
}