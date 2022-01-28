using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Domain.Models
{
    [DataContract]
    public class SmsDeliveryMessage
    {
        public const string TopicName = "sms-delivery-message";
        
        [DataMember(Order = 1)] public string ExternalMessageId { get; set; }
        [DataMember(Order = 2)] public DeliveryStatus Status { get; set; }
        [DataMember(Order = 3)] public string ErrorCode { get; set; }
        [DataMember(Order = 4)] public string ReceiverPhone { get; set; }
    }
}