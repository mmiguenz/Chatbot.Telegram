using System;
using System.Threading.Tasks;
using Telegram.Bot;

using Telegram.Bot.Types;


namespace ChatbotTelegram.handlers
{
    public class ChatbotMessageHandler : IChatBotRequestHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly Message _message;

        public ChatbotMessageHandler(ITelegramBotClient botClient, Message message)
        {
            this._botClient = botClient;
            this._message = message;
        }
        public async Task Handle()
        {
            Console.WriteLine($"Message received from: {_message.Chat.FirstName}");
            await _botClient.SendTextMessageAsync(
                chatId: _message.Chat.Id,
                text: $"echo {_message.Text}"
            );
        }
    }
}