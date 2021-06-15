using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Grpc.Models.Requests
{
    [DataContract]
    public class SendTradeMadeRequest : ISendMessageRequest
    {
        [DataMember(Order = 1)] public string Phone { get; set; }

        [DataMember(Order = 2)] public string Brand { get; set; }

        [DataMember(Order = 3)] public string Lang { get; set; }

        [DataMember(Order = 4)] public string Symbol { get; set; }

        [DataMember(Order = 5)] public decimal Volume { get; set; }

        [DataMember(Order = 6)] public decimal Price { get; set; }
        
        public TemplateEnum Type { get; } = TemplateEnum.TradeMade;

    }
}