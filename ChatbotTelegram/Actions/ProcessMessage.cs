using System;
using System.Threading.Tasks;
using ChatbotTelegram.Model;
using ChatbotTelegram.Services;

namespace ChatbotTelegram.Actions
{
    public class ProcessMessage
    {
        private readonly IOneMenuService _oneMenuService;
        private readonly IConversationStateService _conversationState;

        public ProcessMessage(IOneMenuService oneMenuService, IConversationStateService conversationState)
        {
            _oneMenuService = oneMenuService;
            _conversationState = conversationState;
        }

        public async Task<ProcessMessageResult> Execute(long chatId, string message)
        {
            if (IsCommandMenu(message))
            {
                _conversationState.Reset(chatId);
                var menuLabel = GetMenuLabel(message);

                var transactionId = await _oneMenuService.InitMenuTransaction(menuLabel);

                var step = await _oneMenuService.GetCurrentStep(transactionId);

                _conversationState.SetCurrentMenuTransactionId(chatId, transactionId);

                return new ProcessMessageResult()
                {
                    CurrentStep = step
                };
            }

            var menuTransactionId = _conversationState.GetCurrentMenuTransactionId(chatId);

            if (!string.IsNullOrEmpty(menuTransactionId))
            {
                var processMessageResult = await _oneMenuService.SaveResponse(menuTransactionId, message);

                if (processMessageResult.IsCompleted)
                    _conversationState.Reset(chatId);
                return processMessageResult;
            }
            
            return new ProcessMessageResult()
            {
                AvailableMenus = await _oneMenuService.GetAllMenus()
            };

        }

        private string GetMenuLabel(string message)
        {
            return message[1..];
        }

        private bool IsCommandMenu(string message)
        {
            return message.StartsWith("/");
        }
    }
}