using System;
using System.Threading.Tasks;
using Telegram.Bot;


namespace ChatbotTelegram.handlers
{
    public class UnsupportedOperationHandler : IChatBotRequestHandler
    {
        private readonly ITelegramBotClient _botClient;
        public UnsupportedOperationHandler(ITelegramBotClient botClient)
        {
            this._botClient = botClient;
        }
        public Task Handle()
        {
            throw new NotImplementedException();
        }
    }
}