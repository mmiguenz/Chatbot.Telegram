using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace ChatbotTelegram.Services
{
    public class ConversationStateService : IConversationStateService
    {
        private readonly IMemoryCache _memoryCache;

        public ConversationStateService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public string GetCurrentMenuLabel(long chatId)
        {
            var key = BuildKey("menuLabel", chatId.ToString());

            return GetCachedValue(key);
        }
        public  void SetCurrentMenuLabel(long chatId, string menuLabel)
        {
            var key = BuildKey("menuLabel", chatId.ToString());

            SetValue(key, menuLabel);
        }

        public void SetCurrentMenuTransactionId(long chatId, string menuTransactionId)
        {
            var key = BuildKey("menuTransactionId", chatId.ToString());

            SetValue(key, menuTransactionId);
        }

        public string GetCurrentMenuTransactionId(long chatId)
        {
            var key = BuildKey("menuTransactionId", chatId.ToString());

            return GetCachedValue(key);
        }

        public void Reset(long chatId)
        {
            var keyTransaction = BuildKey("menuTransactionId", chatId.ToString());
            var keyMenuLabel = BuildKey("menuTransactionId", chatId.ToString());
            
            _memoryCache.Remove(keyTransaction);
            _memoryCache.Remove(keyMenuLabel);
        }

        private string BuildKey(string spaceKey, string valueKey)
        {
            return $"${spaceKey}_chatId_${valueKey}";
        }
        private string GetCachedValue(string key)
        {
            _memoryCache.TryGetValue(key, out string entry);

            return entry;
        }
        
        private void SetValue(string key, string value)
        {
            _memoryCache.Set(key, value);
        }
    }
}