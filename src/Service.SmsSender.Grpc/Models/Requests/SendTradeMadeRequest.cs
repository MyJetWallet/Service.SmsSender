using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Grpc.Models.Requests
{
    public class SendTradeMadeRequest
    {
        [DataMember(Order = 1)] public string Phone { get; set; }

        [DataMember(Order = 2)] public LangEnum Lang { get; set; }

        [DataMember(Order = 3)] public string Symbol { get; set; }

        [DataMember(Order = 4)] public decimal Volume { get; set; }

        [DataMember(Order = 5)] public decimal Price { get; set; }
    }
}