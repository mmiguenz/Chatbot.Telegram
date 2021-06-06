using System.Threading.Tasks;

namespace ChatbotTelegram.Services
{
    public interface IConversationStateService
    {
        string GetCurrentMenuLabel(long chatId);
        void SetCurrentMenuLabel(long chatId, string menuLabel);
        void SetCurrentMenuTransactionId(long chatId, string menuTransactionId);
        string GetCurrentMenuTransactionId(long chatId);
        void Reset(long chatId);
    }
}