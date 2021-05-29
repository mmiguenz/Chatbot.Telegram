using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotTelegram.Model;

namespace ChatbotTelegram.Services
{
    public class OneMenuService : IOneMenuService
    {
        public async Task<string> InitMenuTransaction(string menuLabel)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Step> GetCurrentStep(object menuTransactionId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ProcessMessageResult> SaveResponse(string transactionId, string messageText)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Menu>> GetAllMenus()
        {
            throw new System.NotImplementedException();
        }
    }
}