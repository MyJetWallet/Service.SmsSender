using System.Runtime.Serialization;

namespace Service.SmsSender.Grpc.Models.Responses
{
    [DataContract]
    public class SendSmsResponse
    {
        [DataMember]
        public bool Status { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}