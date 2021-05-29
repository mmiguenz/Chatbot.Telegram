using System.Threading.Tasks;

namespace ChatbotTelegram.Services
{
    public interface IConversationStateService
    {
        Task GetCurrentMenuLabel(long chatId, string messageText);
        Task SetCurrentMenuLabel(long chatId, string messageText);
        Task SetCurrentMenuTransactionId(long chatId, object menuTransactionId);
        Task<string> GetCurrentMenuTransactionId(long chatId);
        Task Reset(long chatId);
    }
}