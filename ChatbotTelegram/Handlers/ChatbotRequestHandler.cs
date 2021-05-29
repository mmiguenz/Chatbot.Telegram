using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ChatbotTelegram.Actions;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatbotTelegram.Handlers
{
    public class ChatbotRequestHandler : IUpdateHandler
    {
        private readonly ProcessMessage _processMessage = new ();
        
        public UpdateType[]? AllowedUpdates => null;

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message.Chat.Id;
            await botClient.SendChatActionAsync( chatId: chatId, chatAction: ChatAction.Typing, cancellationToken: cancellationToken);
            try
            {
                string messageData = update.Message.Text;
                var result = await _processMessage.Execute(chatId, messageData);

                await botClient.SendTextMessageAsync(chatId: chatId, text: messageData, cancellationToken: cancellationToken);
            }
            catch (Exception exception)
            {
                await HandleError(botClient, exception, cancellationToken);
            }
        }
        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}