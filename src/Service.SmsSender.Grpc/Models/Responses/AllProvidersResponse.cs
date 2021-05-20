using System.Runtime.Serialization;

namespace Service.SmsSender.Grpc.Models.Responses
{
    [DataContract]
    public class AllProvidersResponse
    {
        [DataMember(Order = 1)]
        public string[] Providers { get; set; }
    }
}
