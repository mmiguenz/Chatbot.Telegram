using System.Threading.Tasks;

namespace ChatbotTelegram.Services
{
    public class ConversationStateService : IConversationStateService
    {
        public async Task GetCurrentMenuLabel(long chatId, string messageText)
        {
            throw new System.NotImplementedException();
        }
        public async Task SetCurrentMenuLabel(long chatId, string messageText)
        {
            throw new System.NotImplementedException();
        }

        public async Task SetCurrentMenuTransactionId(long chatId, object menuTransactionId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> GetCurrentMenuTransactionId(long chatId)
        {
            throw new System.NotImplementedException();
        }

        public async Task Reset(long chatId)
        {
            
        }
    }
}