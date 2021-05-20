using System.Runtime.Serialization;

namespace Service.SmsSender.Grpc.Models.Requests
{
    [DataContract]
    public class SendSmsRequest
    {
        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Body { get; set; }
    }
}