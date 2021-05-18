using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Service.SmsSender.Settings
{
    public class SettingsModel
    {
        [YamlProperty("SmsSender.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("SmsSender.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("SmsSender.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
    }
}
