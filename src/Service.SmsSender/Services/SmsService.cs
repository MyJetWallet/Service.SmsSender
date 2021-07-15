using System.Globalization;
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
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsProviderService> _logger;
        private readonly ISmsProviderManager _smsProviderManager;
        private readonly IMyNoSqlServerDataWriter<TemplateMyNoSqlEntity> _templateWriter;

        public SmsService(ILogger<SmsProviderService> logger,
            ISmsProviderManager smsProviderManager,
            IMyNoSqlServerDataWriter<TemplateMyNoSqlEntity> templateWriter)
        {
            _logger = logger;
            _smsProviderManager = smsProviderManager;
            _templateWriter = templateWriter;
        }

        public async Task<SendResponse> SendLogInSuccessAsync(SendLogInSuccessRequest request)
        {
            var templateBody = await SendMessageAsync(request);
            if (templateBody != null)
            {
                var smsBody = templateBody
                    .Replace("${IP}", request.Ip)
                    .Replace("${DATE}", request.Date.ToString(CultureInfo.InvariantCulture));

                await _smsProviderManager.SendSmsAsync(request.Phone, request.Brand, smsBody, TemplateEnum.LogInSuccess);

                return new SendResponse { Result = SmsSendResult.OK };
            }

            return new SendResponse
            {
                Result = SmsSendResult.TEMPLATE_NOT_FOUND,
                ErrorMessage = "Template doesn't exist."
            };
        }

        public async Task<SendResponse> SendTradeMadeAsync(SendTradeMadeRequest request)
        {
            var templateBody = await SendMessageAsync(request);
            if (templateBody != null)
            {
                var smsBody = templateBody
                    .Replace("${SYMBOL}", request.Symbol)
                    .Replace("${PRICE}", request.Price.ToString(CultureInfo.InvariantCulture))
                    .Replace("${VOLUME}", request.Volume.ToString(CultureInfo.InvariantCulture));

                await _smsProviderManager.SendSmsAsync(request.Phone, request.Brand, smsBody, TemplateEnum.TradeMade);

                return new SendResponse { Result = SmsSendResult.OK };
            }

            return new SendResponse
            {
                Result = SmsSendResult.TEMPLATE_NOT_FOUND,
                ErrorMessage = "Template doesn't exist."
            };
        }

        public async Task<SendResponse> SendVerificationAsync(SendVerificationRequest request)
        {
            var templateBody = await SendMessageAsync(request);
            if (templateBody != null)
            {
                var smsBody = templateBody
                    .Replace("${CODE}", request.Code);

                await _smsProviderManager.SendSmsAsync(request.Phone, request.Brand, smsBody, TemplateEnum.Verification);

                return new SendResponse { Result = SmsSendResult.OK };
            }

            return new SendResponse
            {
                Result = SmsSendResult.TEMPLATE_NOT_FOUND,
                ErrorMessage = "Template doesn't exist."
            };
        }

        public async Task<SentHistoryResponse> GetSentHistoryAsync(GetSentHistoryRequest request)
        {
            var maxCount = request.MaxCount > 0 ? request.MaxCount : 20;

            var records = await _smsProviderManager.GetSentHistoryAsync(maxCount, request.Since);

            return new SentHistoryResponse {SentHistoryRecords = records.Select(SentHistoryRecord.Create).ToList() };
        }

        #region Private methods

        private async Task<string> SendMessageAsync<T>(T request) where T : ISendMessageRequest
        {
            var partitionKey = TemplateMyNoSqlEntity.GeneratePartitionKey();
            var rowKey = TemplateMyNoSqlEntity.GenerateRowKey(request.Type.ToString());

            var templateEntity = await _templateWriter.GetAsync(partitionKey, rowKey);
            if (templateEntity == null)
            {
                _logger.LogError("Template with part.key {partitionKey} & row key {rowKey} doesn't exist.", partitionKey, rowKey);

                return null;
            }

            var brand = request.Brand;
            var brandLangBodies = templateEntity.Template.BrandLangBodies.FirstOrDefault(b => b.Brand == brand);
            if (brandLangBodies == null)
            {
                _logger.LogInformation("Template (ID: {templateId}) for brand {brand} doesn't exist, switch to the default brand ({defaultBrand}).",
                    templateEntity.Template.Id, request.Brand, templateEntity.Template.DefaultBrand);

                brand = templateEntity.Template.DefaultBrand;
                brandLangBodies = templateEntity.Template.BrandLangBodies.FirstOrDefault(b => b.Brand == brand);
                if (brandLangBodies == null)
                {
                    _logger.LogInformation("Template (ID: {templateId}) for default brand ({defaultBrand}) doesn't exist.",
                        templateEntity.Template.Id, request.Lang, brand);

                    return null;
                }
            }

            if (!brandLangBodies.LangBodies.TryGetValue(request.Lang, out var templateBody)) //set brand, set lang
            {
                _logger.LogInformation("Template (ID: {templateId}) for brand {brand} with lang {lang} doesn't exist, switch to the default lang ({defaultLang}).",
                    templateEntity.Template.Id, brand, request.Lang, templateEntity.Template.DefaultLang);

                if (!brandLangBodies.LangBodies.TryGetValue(templateEntity.Template.DefaultLang, out templateBody)) //set brand, default lang 
                {
                    _logger.LogInformation("Template (ID: {templateId}) for the default lang ({defaultLang}) doesn't exist. Switching to  default brand ({defaultBrand}) and default lang ({defaultLang})",
                        templateEntity.Template.Id, templateEntity.Template.DefaultBrand, templateEntity.Template.DefaultLang);

                    var defaultBrandLangBodies = templateEntity.Template.BrandLangBodies.FirstOrDefault(b => b.Brand == templateEntity.Template.DefaultBrand);
                    if(!defaultBrandLangBodies.LangBodies.TryGetValue(request.Lang, out templateBody)) //default brand, set lang
                    {
                        _logger.LogInformation("Template (ID: {templateId}) for defaultBrand {defaultBrand} with lang {lang} doesn't exist, switch to the default lang ({defaultLang}).",
                            templateEntity.Template.Id, templateEntity.Template.DefaultBrand, request.Lang, templateEntity.Template.DefaultLang);
                        
                        if (!defaultBrandLangBodies.LangBodies.TryGetValue(templateEntity.Template.DefaultLang,
                            out templateBody)) //default brand, default lang
                        {
                            _logger.LogError(
                                "Template (ID: {templateId}) for the default brand ({defaultBrand}) and default lang ({defaultLang}) doesn't exist.",
                                templateEntity.Template.Id, templateEntity.Template.DefaultBrand,
                                templateEntity.Template.DefaultLang);
                            return null;
                        }
                    }
                }
            }

            return templateBody;
        }

        #endregion
    }
}