using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ProcessMessage _processMessage;

        public ChatbotRequestHandler(ServiceProvider provider)
        {
            _processMessage = provider.GetService<ProcessMessage>();
        }

        public UpdateType[]? AllowedUpdates => new [] {UpdateType.Message, UpdateType.CallbackQuery};

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
           
            try
            {
                var  (chatId, messageData) = update.Type  switch
                {
                    UpdateType.Message => (update.Message.Chat.Id, update.Message.Text),
                    UpdateType.CallbackQuery => (update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Data),
                    _ => (0, "/start")
                };
                
                await botClient.SendChatActionAsync( chatId: chatId, chatAction: ChatAction.Typing, cancellationToken: cancellationToken);
                
                var result = await _processMessage.Execute(chatId, messageData);

                if (result.HasErrors)
                {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: GetErrorResponse(result));    
                }
        
                await botClient.SendTextMessageAsync(chatId: chatId, text: GetTextResponse(result), replyMarkup: GetReplyMarkup(result));
            }
            catch (Exception exception)
            {
                await HandleError(botClient, exception, cancellationToken);
            }
        }

        private string GetErrorResponse(ProcessMessageResult result)
        {
            return string.Join("\n", result.Errors);
        }

        private InlineKeyboardMarkup GetReplyMarkup(ProcessMessageResult result)
        {
            if (result.CurrentStep?.Options != null)
            {
                return  new InlineKeyboardMarkup( 
                    result.CurrentStep?.Options
                          .Select(o =>  
                              new [] {InlineKeyboardButton.WithCallbackData(o.DisplayText, o.Value)})
                          .ToArray());
            }

            return null;
        }

        private string GetTextResponse(ProcessMessageResult result)
        {
            if (result.IsCompleted)
            {
                return result.CompletionMsg;
            }
            
            if (result.AvailableMenus != null)
            {
                return BuildGlobalMenu(result.AvailableMenus);
            }

            if (result.CurrentStep != null)
            {
                return result.CurrentStep.Text;
                
            }

            return "Error procesando el mensaje";
        }

        private string BuildGlobalMenu(IEnumerable<Menu> resultAvailableMenus)
        {
            return "Bienvenido! \n Opciones Disponibles: \n" + string.Join("\n", resultAvailableMenus.Select(m => $"/{m.Label}  {m.Title}"));
        }

        public async Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
        }
    }
}