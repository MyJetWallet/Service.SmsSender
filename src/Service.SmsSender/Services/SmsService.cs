using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
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
            var partitionKey = TemplateMyNoSqlEntity.GeneratePartitionKey();
            var rowKey = TemplateMyNoSqlEntity.GenerateRowKey(TemplateEnum.LogInSuccess.ToString());

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

            if (!templateEntity.Template.LangBodies.TryGetValue(request.Lang, out var templateBody))
            {
                _logger.LogInformation("Template (ID: {templateId}) for lang {lang} doesn't exist, switch to the default lang ({defaultLang}).",
                    templateEntity.Template.Id, request.Lang, templateEntity.Template.DefaultLang);

                if (!templateEntity.Template.LangBodies.TryGetValue(templateEntity.Template.DefaultLang, out templateBody))
                {
                    _logger.LogError("Template (ID: {templateId}) for the default lang ({defaultLang}) doesn't exist.",
                        templateEntity.Template.Id, templateEntity.Template.DefaultLang);

                    return new SendResponse
                    {
                        Result = SmsSendResult.TEMPLATE_NOT_FOUND,
                        ErrorMessage = "Template doesn't exist for any reasonable langs."
                    };
                }
            }

            var smsBody = templateBody
                .Replace("${IP}", request.Ip)
                .Replace("${DATE}", request.Date.ToString(CultureInfo.InvariantCulture));

            await _smsProviderManager.SendSmsAsync(request.Phone, smsBody);

            return new SendResponse { Result = SmsSendResult.OK };
        }

        public async Task<SendResponse> SendTradeMadeAsync(SendTradeMadeRequest request)
        {
            var partitionKey = TemplateMyNoSqlEntity.GeneratePartitionKey();
            var rowKey = TemplateMyNoSqlEntity.GenerateRowKey(TemplateEnum.LogInSuccess.ToString());

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

            if (!templateEntity.Template.LangBodies.TryGetValue(request.Lang, out var templateBody))
            {
                _logger.LogInformation("Template (ID: {templateId}) for lang {lang} doesn't exist, switch to the default lang ({defaultLang}).",
                    templateEntity.Template.Id, request.Lang, templateEntity.Template.DefaultLang);

                if (!templateEntity.Template.LangBodies.TryGetValue(templateEntity.Template.DefaultLang, out templateBody))
                {
                    _logger.LogError("Template (ID: {templateId}) for the default lang ({defaultLang}) doesn't exist.",
                        templateEntity.Template.Id, templateEntity.Template.DefaultLang);

                    return new SendResponse
                    {
                        Result = SmsSendResult.TEMPLATE_NOT_FOUND,
                        ErrorMessage = "Template doesn't exist for any reasonable langs."
                    };
                }
            }

            var smsBody = templateBody
                .Replace("${SYMBOL}", request.Symbol)
                .Replace("${PRICE}", request.Price.ToString(CultureInfo.InvariantCulture))
                .Replace("${VOLUME}", request.Volume.ToString(CultureInfo.InvariantCulture));

            await _smsProviderManager.SendSmsAsync(request.Phone, smsBody);

            return new SendResponse { Result = SmsSendResult.OK };
        }
    }
}