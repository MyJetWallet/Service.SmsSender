using System;
using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Grpc.Models.Requests
{
    [DataContract]
    public class SendLogInSuccessRequest
    {
        [DataMember(Order = 1)] public string Phone { get; set; }

        [DataMember(Order = 2)] public LangEnum Lang { get; set; }

        [DataMember(Order = 3)] public string Ip { get; set; }

        [DataMember(Order = 4)] public DateTime Date { get; set; }
    }
}