using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatbotTelegram.Actions;
using ChatbotTelegram.Model;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Chatbot.Telegram.Core.Handlers
{
    public class UpdateHandler
    {
        private readonly ProcessMessage _processMessage;

        public UpdateHandler(ProcessMessage processMessage)
        {
            _processMessage = processMessage;
        }
        
        public async Task HandleUpdate(ITelegramBotClient botClient, Update update)
        {
            var  (chatId, messageData) = update.Type  switch
            {
                UpdateType.Message => (update.Message.Chat.Id, update.Message.Text),
                UpdateType.CallbackQuery => (update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Data),
                _ => (0, "/start")
            };
            
            try
            {
                await botClient.SendChatActionAsync( chatId: chatId, chatAction: ChatAction.Typing);
                
                var result = await _processMessage.Execute(chatId, messageData);

                if (result.HasErrors)
                {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: GetErrorResponse(result));    
                }
        
                await botClient.SendTextMessageAsync(chatId: chatId, text: GetTextResponse(result), replyMarkup: GetReplyMarkup(result));
            }
            catch (Exception exception)
            {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "en este momento no podemos procesar tu consulta \n /start");    
            }
        }
        
        private string GetErrorResponse(ProcessMessageResult result)
        {
            return string.Join("\n", result.ValidationErrors);
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
    }
}