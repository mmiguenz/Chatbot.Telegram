using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotTelegram.Model;

namespace ChatbotTelegram.Services
{
    public interface IOneMenuService
    {
        Task<string> InitMenuTransaction(string menuLabel);
        Task<Step> GetCurrentStep(object menuTransactionId);
        Task<ProcessMessageResult> SaveResponse(string transactionId, string messageText);
        Task<IEnumerable<Menu>> GetAllMenus();
    }
}