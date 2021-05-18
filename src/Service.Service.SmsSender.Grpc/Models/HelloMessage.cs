using System.Runtime.Serialization;
using Service.Service.SmsSender.Domain.Models;

namespace Service.Service.SmsSender.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}