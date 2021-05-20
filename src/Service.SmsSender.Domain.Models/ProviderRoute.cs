using System.Runtime.Serialization;

namespace Service.SmsSender.Domain.Models
{
    [DataContract]
    public class ProviderRoute
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }
        
        [DataMember(Order = 3)]
        public int Order { get; set; }
        
        [DataMember(Order = 4)]
        public string Pattern { get; set; }
        
        [DataMember(Order = 5)]
        public string ProviderId { get; set; }
    }
}