using System.Runtime.Serialization;

namespace Service.SmsSender.Grpc.Models.Requests
{
    [DataContract]
    public class GetSentHistoryRequest
    {
        [DataMember(Order = 1)]
        public int MaxCount { get; set; }
    }
}
