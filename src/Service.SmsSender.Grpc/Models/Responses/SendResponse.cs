using System.Runtime.Serialization;
using Service.SmsSender.Grpc.Enums;

namespace Service.SmsSender.Grpc.Models.Responses
{
    [DataContract]
    public class SendResponse
    {
        [DataMember]
        public SmsSendResult Result { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}