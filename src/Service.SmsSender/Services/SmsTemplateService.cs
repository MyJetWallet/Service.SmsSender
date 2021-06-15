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
        private const string _defaultBrand = "DefaultBrand";
        private const string _defaultLang = "En";
        private readonly IDictionary<TemplateEnum, string> _defaultBrandLangBodies = new Dictionary<TemplateEnum, string>
        {
            { TemplateEnum.LogInSuccess, "Successful log in account from IP ${IP} (${DATE})" },
            { TemplateEnum.TradeMade, "Trade made: ${SYMBOL}, price ${PRICE}, volume ${VOLUME}" },
            { TemplateEnum.Verification, "Your verification code: ${CODE}"}
        };
        private readonly IDictionary<TemplateEnum, List<string>> _templateBodyParams = new Dictionary<TemplateEnum, List<string>>
        {
            { TemplateEnum.LogInSuccess, new List<string> { "${IP}", "${DATE}" } },
            { TemplateEnum.TradeMade, new List<string> { "${SYMBOL}", "${PRICE}", "${VOLUME}" } },
            { TemplateEnum.Verification, new List<string> { "${CODE}" } }
        };

        public SmsTemplateService(ILogger<SmsProviderService> logger, IMyNoSqlServerDataWriter<TemplateMyNoSqlEntity> templateWriter)
        {
            _logger = logger;
            _templateWriter = templateWriter;
        }

        public async Task CreateDefaultTemplatesAsync()
        {
            var templateEntities = (await _templateWriter.GetAsync())?.ToArray();
            var templateIds = Enum.GetValues(typeof(TemplateEnum)).Cast<TemplateEnum>();
            foreach (var templateId in templateIds)
            {
                var template = templateEntities?.FirstOrDefault(e => e.Template.Id == templateId)?.Template;
                if (template == null)
                {
                    template = new SmsTemplate
                    {
                        Id = templateId,
                        DefaultLang = _defaultLang,
                        DefaultBrand = _defaultBrand,
                        BrandLangBodies = GetTemplateLangBodies(templateId),
                        Params = GetTemplateBodyParams(templateId)
                    };

                    var newTemplateEntity = TemplateMyNoSqlEntity.Create(template);
                    await _templateWriter.InsertAsync(newTemplateEntity);

                    _logger.LogInformation("Template (ID: {templateId}) doesn't exist, creating the new one.", templateId);
                }
            }
        }
        public async Task<TemplateListResponse> GetAllTemplatesAsync()
        {
            var templateEntities = (await _templateWriter.GetAsync())?.ToArray();
            var templateIds = Enum.GetValues(typeof(TemplateEnum)).Cast<TemplateEnum>();

            return new TemplateListResponse
            {
                Templates = templateIds.Select(templateId => templateEntities?.FirstOrDefault(e => e.Template.Id == templateId)?.Template).ToArray()
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

            if (string.IsNullOrEmpty(request.TemplateBody))
            {
                var blb = templateEntity.Template.BrandLangBodies.FirstOrDefault(b => b.Brand == request.Brand);
                if (blb != null)
                {
                    var lng = request.Lang.ToString();
                    if (blb.LangBodies.ContainsKey(lng))
                    {
                        blb.LangBodies.Remove(lng);

                        _logger.LogInformation("Lang body with lang {lang} has been removed from brand ({brand}).", request.Lang, request.Brand);

                        if (!blb.LangBodies.Any())
                        {
                            templateEntity.Template.BrandLangBodies = templateEntity.Template.BrandLangBodies
                                .Where(b => b.Brand != blb.Brand).ToArray();

                            _logger.LogInformation("Brand ({brand}) removed because it no longer has lang bodies.", request.Brand);
                        }

                        return new SendResponse { Result = SmsSendResult.OK };
                    }
                }
            }

            var lang = request.Lang.ToString();
            var brandLangBodies = templateEntity.Template.BrandLangBodies.FirstOrDefault(b => b.Brand == request.Brand);
            if (brandLangBodies == null)
            {
                _logger.LogInformation("Template (ID: {templateId}) for required brand {brand} doesn't exist, creating new one.",
                    templateEntity.Template.Id, request.Brand);

                templateEntity.Template.BrandLangBodies = templateEntity.Template.BrandLangBodies.Append(new BrandLangBody
                {
                    Brand = request.Brand,
                    LangBodies = new Dictionary<string, string>
                    {
                        { lang, request.TemplateBody }
                    }
                }).ToArray();
            }
            else
            {
                if (!brandLangBodies.LangBodies.ContainsKey(lang))
                {
                    _logger.LogInformation("Template (ID: {templateId}) for brand {brand} with required lang {lang} doesn't exist, creating new one.",
                        templateEntity.Template.Id, request.Brand, request.Lang);

                    brandLangBodies.LangBodies.Add(lang, request.TemplateBody);
                }
            }
            
            if (!string.IsNullOrEmpty(request.DefaultBrand))
            {
                templateEntity.Template.DefaultBrand = request.DefaultBrand;
            }

            templateEntity.Template.DefaultLang = request.DefaultLang;

            await _templateWriter.InsertOrReplaceAsync(templateEntity);

            return new SendResponse { Result = SmsSendResult.OK };
        }

        private BrandLangBody[] GetTemplateLangBodies(TemplateEnum templateId)
        {
            return new[] {
                new BrandLangBody
                {
                    Brand = _defaultBrand,
                    LangBodies = new Dictionary<string, string>()
                    {
                        {_defaultLang, _defaultBrandLangBodies[templateId]}   
                    }
                }
            };
        }

        private List<string> GetTemplateBodyParams(TemplateEnum templateId) => _templateBodyParams[templateId];
    }
}