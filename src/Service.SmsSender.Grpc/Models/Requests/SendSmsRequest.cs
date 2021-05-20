using System.Runtime.Serialization;

namespace Service.SmsSender.Grpc.Models.Requests
{
    [DataContract]
    public class SendSmsRequest
    {
        [DataMember(Order = 1)]
        public string Phone { get; set; }

        [DataMember(Order = 2)]
        public string Body { get; set; }
    }
}