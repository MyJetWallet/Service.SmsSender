using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.SmsSender.Domain.Models
{
    [DataContract]
    public class BrandLangBody
    {
        [DataMember(Order = 1)]
        public string Brand { get; set; }
        
        [DataMember(Order = 2)]
        public Dictionary<string, string> LangBodies { get; set; }
    }
}
