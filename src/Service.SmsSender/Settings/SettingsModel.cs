using System.Collections.Generic;
using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.SmsSender.Settings
{
    public class SettingsModel
    {
        [YamlProperty("SmsSender.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }
        
        [YamlProperty("SmsSender.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("SmsSender.ZipkinUrl")]
        public string ZipkinUrl { get; set; }
        
        [YamlProperty("SmsSender.SmsProviders")]
        public Dictionary<string, SmsProvider> SmsProviders { get; set; }

        [YamlProperty("SmsSender.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
    }

    public class SmsProvider
    {
        [YamlProperty("GrpcUrl")]
        public string GrpcUrl { get; set; }
    }
}
