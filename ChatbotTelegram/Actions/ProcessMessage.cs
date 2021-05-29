using System;
using System.Threading.Tasks;
using ChatbotTelegram.Model;

namespace ChatbotTelegram.Actions
{
    public class ProcessMessage
    {
        public async Task<ProcessMessageResult> Execute(long chatId, string message)
        {
            return new ProcessMessageResult();
        }
    }
}