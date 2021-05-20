using System.Collections.Generic;
using Service.SmsSender.Domain.Models.Enums;

namespace Service.SmsSender.Domain.Models
{
    public class SmsTemplate
    {
        public TemplateEnum Id { get; set; }

        public LangEnum DefaultLang { get; set; }

        public Dictionary<LangEnum, string> LangBodies { get; set; }

        public List<string> Params { get; set; }
    }
}