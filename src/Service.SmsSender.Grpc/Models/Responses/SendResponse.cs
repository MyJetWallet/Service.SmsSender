using System.Runtime.Serialization;
using Service.SmsSender.Grpc.Enums;

namespace Service.SmsSender.Grpc.Models.Responses
{
    [DataContract]
    public class SendResponse
    {
        [DataMember(Order = 1)]
        public SmsSendResult Result { get; set; }

        [DataMember(Order = 1)]
        public string ErrorMessage { get; set; }
    }
}