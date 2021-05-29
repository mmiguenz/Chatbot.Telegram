using System;
using System.Threading.Tasks;
using ChatbotTelegram.Services;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace ChatbotTelegram.Handlers
{
    public class ChatbotMessageHandler : IChatBotRequestHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly Message _message;
        private readonly ConversationStateService _conversationState;
        private readonly OneMenuService _oneMenuService;

        public ChatbotMessageHandler(ITelegramBotClient botClient, Message message)
        {
            _botClient = botClient;
            _message = message;
            _conversationState = new ConversationStateService();
            _oneMenuService = new OneMenuService();
        }

        public async Task Handle()
        {
            var chatId = _message.Chat.Id;
            await RespondChatWithMenus(_botClient);
            return;
            
            /*if (IsMenuMsg(_message))
            {
                var menuLabel = _message.Text.Replace("/", "");

                await _conversationState.SetCurrentMenu(chatId, _message.Text);

                var menuTransactionId = await _oneMenuService.InitMenuTransaction(menuLabel);

                await _conversationState.SetMenuTransactionId(chatId, menuTransactionId);

                var firstStep = await _oneMenuService.GetCurrentStep(menuTransactionId);

                await RespondChat(_botClient, firstStep);
            }

            string transactionId = await _conversationState.GetCurrentMenuTransactionId(chatId);

            if (string.IsNullOrWhiteSpace(transactionId))
            {
                await RespondChatWithMenus(_botClient);
            }

            var result = await _oneMenuService.SaveResponse(transactionId, _message.Text);

            await RespondChat(_botClient, result);*/
        }

        private async Task RespondChatWithMenus(ITelegramBotClient botClient)
        {
          await _botClient.SendTextMessageAsync(_message.Chat.Id, "/menu_test  this is the first menu");
        }

        private async Task RespondChat(ITelegramBotClient botClient, object firstStep)
        {
            throw new NotImplementedException();
        }

        private bool IsMenuMsg(Message message)
        {
            return message.Text.StartsWith("/");
        }
    }
}