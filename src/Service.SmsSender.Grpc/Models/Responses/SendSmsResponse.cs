using System.Runtime.Serialization;

namespace Service.SmsSender.Grpc.Models.Responses
{
    [DataContract]
    public class SendSmsResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public string ErrorMessage { get; set; }
    }
}