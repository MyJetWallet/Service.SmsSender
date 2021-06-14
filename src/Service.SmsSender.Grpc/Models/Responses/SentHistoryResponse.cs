using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.SmsSender.Domain.Models;

namespace Service.SmsSender.Grpc.Models.Responses
{
    [DataContract]
    public class SentHistoryResponse
    {
        [DataMember(Order = 1)]
        public List<SentHistoryRecord> SentHistoryRecords { get; set; }
    }
}
