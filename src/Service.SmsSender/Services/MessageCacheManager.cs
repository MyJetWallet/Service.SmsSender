using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.SmsSender.Domain.Models;

namespace Service.SmsSender.Services
{
    public static class MessageCacheManager
    {
        private static List<CachedMessage> _messages = new List<CachedMessage>();
        private const int MaxCacheSize = 1000;

        public static void AddMessage(CachedMessage message)
        {
            var existingMessage = GetMessage(message.RetryId);
            if (existingMessage != null)
                _messages.Remove(message);
            
            _messages.Add(message);
            if (_messages.Count > MaxCacheSize)
            {
                var newList = _messages.OrderByDescending(t => t.Timestamp).Take(MaxCacheSize).ToList();
                _messages.Clear();
                _messages.AddRange(newList);
            }
        }

        public static CachedMessage GetMessage(string retryId)
        {
            return _messages.FirstOrDefault(t => t.RetryId == retryId);
        }

        public static void DeleteMessage(string retryId)
        {
            var message = GetMessage(retryId);
            _messages.Remove(message);
        }
    }
}