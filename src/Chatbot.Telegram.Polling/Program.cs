using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Chatbot.Telegram.Core.Handlers;
using ChatbotTelegram.Actions;
using ChatbotTelegram.Handlers;
using ChatbotTelegram.Services;
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Formatting.Compact;
using Telegram.Bot.Extensions.Polling;

namespace ChatbotTelegram
{
    class Program
    {
        static ITelegramBotClient botClient;

        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMemoryCache();
            services.AddScoped<IConversationStateService, ConversationStateService>();
            services.AddScoped<IOneMenuService, OneMenuService>();
            services.AddScoped<ProcessMessage, ProcessMessage>();
            services.AddScoped<UpdateHandler, UpdateHandler>();
            services.Add(new ServiceDescriptor(typeof(HttpClient), (_) => GetClient(), ServiceLifetime.Scoped));
            
            await SetUpChatBot(services.BuildServiceProvider());
        }

        private static HttpClient GetClient()
        {
            var oneMenuServiceUrl = Environment.GetEnvironmentVariable("OneMenuServiceUrl") ?? string.Empty;
            
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };
            
           return  new HttpClient(httpClientHandler) { BaseAddress = new Uri(oneMenuServiceUrl) };
        }

        private static async Task SetUpChatBot(ServiceProvider provider)
        {
            var token = Environment.GetEnvironmentVariable("chatbotToken");
            botClient = new TelegramBotClient(token);

            var me = await botClient.GetMeAsync();
            Console.Title = me.Username;
            
            var cts = new CancellationTokenSource();
            var handler = new ChatbotRequestHandler(provider);
            
            botClient.StartReceiving(
                new DefaultUpdateHandler(handler.HandleUpdate, handler.HandleError),
                cts.Token
            );
            
            
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
       /*     botClient.StartReceiving(
                handler,
                cts.Token
            );
         */   
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            
            cts.Cancel();
        }
    }
}