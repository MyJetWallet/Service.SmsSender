using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.SmsSender.Domain.Models;
using Service.SmsSender.Domain.Models.Enums;
using Service.SmsSender.Grpc;
using Service.SmsSender.Grpc.Enums;
using Service.SmsSender.Grpc.Models.Requests;
using Service.SmsSender.Grpc.Models.Responses;
using Service.SmsSender.NoSql;

namespace Service.SmsSender.Services
{
    public class SmsTemplateService : ISmsTemplateService
    {
        private readonly ILogger<SmsProviderService> _logger;
        private readonly IMyNoSqlServerDataWriter<TemplateMyNoSqlEntity> _templateWriter;
        private readonly IDictionary<TemplateEnum, string> _defaultBrandLangBodies = new Dictionary<TemplateEnum, string>
        {
            { TemplateEnum.LogInSuccess, "Successful log in account from IP ${IP} (${DATE})" },
            { TemplateEnum.TradeMade, "Trade made: ${SYMBOL}, price ${PRICE}, volume ${VOLUME}" }
        };
        private readonly IDictionary<TemplateEnum, List<string>> _templateBodyParams = new Dictionary<TemplateEnum, List<string>>
        {
            { TemplateEnum.LogInSuccess, new List<string> { "${IP}", "${DATE}" } },
            { TemplateEnum.TradeMade, new List<string> { "${SYMBOL}", "${PRICE}", "${VOLUME}" } }
        };

        public SmsTemplateService(ILogger<SmsProviderService> logger, IMyNoSqlServerDataWriter<TemplateMyNoSqlEntity> templateWriter)
        {
            _logger = logger;
            _templateWriter = templateWriter;
        }

        public async Task<TemplateListResponse> GetAllTemplatesAsync()
        {
            var templateEntities = (await _templateWriter.GetAsync())?.ToArray();
            var templateIds = Enum.GetValues(typeof(TemplateEnum)).Cast<TemplateEnum>();
            var templates = new List<SmsTemplate>();
            
            foreach (var templateId in templateIds)
            {
                var template = templateEntities?.FirstOrDefault(e => e.Template.Id == templateId)?.Template;
                if (template == null)
                {
                    template = new SmsTemplate
                    {
                        Id = templateId,
                        DefaultLang = LangEnum.En,
                        BrandLangBodies = GetTemplateLangBodies(templateId),
                        Params = GetTemplateBodyParams(templateId)
                    };

                    var newTemplateEntity = TemplateMyNoSqlEntity.Create(template);
                    await _templateWriter.InsertAsync(newTemplateEntity);

                    _logger.LogInformation("Template (ID: {templateId}) doesn't exist, creating the new one.", templateId);
                }

                templates.Add(template);
            }

            return new TemplateListResponse
            {
                Templates = templates.ToArray()
            };
        }

        public async Task<SendResponse> EditTemplateAsync(EditTemplateRequest request)
        {
            var partitionKey = TemplateMyNoSqlEntity.GeneratePartitionKey();
            var rowKey = TemplateMyNoSqlEntity.GenerateRowKey(request.TemplateId.ToString());

            var templateEntity = await _templateWriter.GetAsync(partitionKey, rowKey);
            if (templateEntity == null)
            {
                _logger.LogError("Template with part.key {partitionKey} & row key {rowKey} doesn't exist.", partitionKey, rowKey);

                return new SendResponse
                {
                    Result = SmsSendResult.TEMPLATE_NOT_FOUND,
                    ErrorMessage = "Template doesn't exist."
                };
            }
            
            if (!templateEntity.Template.BrandLangBodies.ContainsKey(request.Brand))
            {
                _logger.LogInformation("Template (ID: {templateId}) for required brand {brand} doesn't exist.",
                    templateEntity.Template.Id, request.Brand);

                return new SendResponse
                {
                    Result = SmsSendResult.TEMPLATE_NOT_FOUND,
                    ErrorMessage = "Template doesn't exist for required brand."
                };
            }

            var brandLangBodies = templateEntity.Template.BrandLangBodies[request.Brand];
            var lang = request.Lang.ToString();

            if (!brandLangBodies.ContainsKey(lang))
            {
                _logger.LogInformation("Template (ID: {templateId}) for required lang {lang} doesn't exist.",
                    templateEntity.Template.Id, request.Lang);

                return new SendResponse
                {
                    Result = SmsSendResult.TEMPLATE_NOT_FOUND,
                    ErrorMessage = "Template doesn't exist for required lang."
                };
            }

            brandLangBodies[lang] = request.TemplateBody;

            await _templateWriter.InsertOrReplaceAsync(templateEntity);

            return new SendResponse { Result = SmsSendResult.OK };
        }

        private Dictionary<string, Dictionary<string, string>> GetTemplateLangBodies(TemplateEnum templateId)
        {
            var langs = Enum.GetValues(typeof(LangEnum)).Cast<LangEnum>();
            return new Dictionary<string, Dictionary<string, string>>
            {
                { "DefaultBrand", langs.ToDictionary(lang => lang.ToString(), lang => _defaultBrandLangBodies[templateId])}
            };
        }

        private List<string> GetTemplateBodyParams(TemplateEnum templateId) => _templateBodyParams[templateId];
    }
}