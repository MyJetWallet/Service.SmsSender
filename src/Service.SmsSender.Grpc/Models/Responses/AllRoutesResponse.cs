using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models;

namespace Service.SmsSender.Grpc.Models.Responses
{
    [DataContract]
    public class AllRoutesResponse
    {
        [DataMember(Order = 1)]
        public ProviderRoute[] Routes { get; set; }
    }
}