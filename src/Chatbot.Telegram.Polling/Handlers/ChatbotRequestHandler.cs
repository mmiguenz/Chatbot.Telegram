using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatbot.Telegram.Core.Handlers;
using ChatbotTelegram.Actions;
using ChatbotTelegram.Model;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatbotTelegram.Handlers
{
    public class ChatbotRequestHandler : IUpdateHandler
    {
        private readonly UpdateHandler _updateHandler;

        public ChatbotRequestHandler(ServiceProvider provider)
        {
            _updateHandler = provider.GetService<UpdateHandler>();
        }
      

        public UpdateType[]? AllowedUpdates => new [] {UpdateType.Message, UpdateType.CallbackQuery};

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var  (chatId, messageData) = update.Type  switch
            {
                UpdateType.Message => (update.Message.Chat.Id, update.Message.Text),
                UpdateType.CallbackQuery => (update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Data),
                _ => (0, "/start")
            };
            
            try
            {
                await _updateHandler.HandleUpdate(botClient, update);
            }
            catch (Exception exception)
            {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "en este momento no podemos procesar tu consulta \n /start");    
            }
        }
        public async Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
        }
    }
}