using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Service.SmsSender.Domain.Models;
using Service.SmsSender.Domain.Models.Enums;
using Service.SmsSender.Postgres;
using Service.SmsSender.Services;

namespace Service.SmsSender.Jobs
{
    public class MessageDeliveryJob
    {
        private readonly DbContextOptionsBuilder<SmsSenderDbContext> _dbContextOptionsBuilder;
        private readonly ISmsProviderManager _smsProviderManager;

        public MessageDeliveryJob(ISubscriber<SmsDeliveryMessage> subscriber, DbContextOptionsBuilder<SmsSenderDbContext> dbContextOptionsBuilder, ISmsProviderManager smsProviderManager)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _smsProviderManager = smsProviderManager;

            subscriber.Subscribe(HandleEvents);
        }

        private async ValueTask HandleEvents(SmsDeliveryMessage message)
        {
            await using var context = new SmsSenderDbContext(_dbContextOptionsBuilder.Options);
            var record = await  context.SentHistory.FirstOrDefaultAsync(t => t.ExternalMessageId == message.ExternalMessageId);
            if(record == null)
                return;

            switch (message.Status)
            {
                case DeliveryStatus.Delivered:
                    record.Status = MessageStatus.Delivered;
                    break;
                case DeliveryStatus.Failed:
                    record.Status = MessageStatus.Failed;
                    record.RetryCount++;
                    //
                    // if (record.RetryCount <= Program.Settings.RetryCount)
                    // {
                    //     //TODO: retry sms
                    // }
                    break;
            }
         
            await context.UpsetAsync(new[] { record });
        }
    }
}