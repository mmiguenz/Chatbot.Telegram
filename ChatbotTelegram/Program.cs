using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ChatbotTelegram.handlers;
using Telegram.Bot;
using Microsoft.Extensions.Configuration;

namespace ChatbotTelegram
{
    class Program
    {
        static ITelegramBotClient botClient;

        public static async Task Main()
        {
            var token = Environment.GetEnvironmentVariable("chatbotToken");
            botClient = new TelegramBotClient(token);

            var me = await botClient.GetMeAsync();
            Console.Title = me.Username;
            
            var cts = new CancellationTokenSource();
            
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            botClient.StartReceiving(
                new ChatbotRequestHandler(),
                cts.Token
            );

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }
    }
}