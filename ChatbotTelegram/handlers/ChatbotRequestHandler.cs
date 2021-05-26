using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatbotTelegram.handlers
{
    public class ChatbotRequestHandler : IUpdateHandler
    {
        public UpdateType[]? AllowedUpdates => new List<UpdateType>() {UpdateType.Message}.ToArray();
        
        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            IChatBotRequestHandler handler = update.Type switch
            {
                UpdateType.Message => new ChatbotMessageHandler(botClient, update.Message),
                _ => new UnsupportedOperationHandler(botClient)
            };

            try
            {
                await handler.Handle();
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