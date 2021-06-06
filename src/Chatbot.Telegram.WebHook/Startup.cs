using System;
using System.Net.Http;
using Chatbot.Telegram.Core.Handlers;
using ChatbotTelegram.Actions;
using ChatbotTelegram.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Telegram.Bot;

namespace Chatbot.Telegram.WebHook
{
    public class Startup
    {
        static ITelegramBotClient botClient;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void  ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Chatbot.Telegram.WebHook", Version = "v1"});
            });

            SetUpChatBot();

            services.AddMemoryCache();
            services.AddScoped<IConversationStateService, ConversationStateService>();
            services.AddScoped<IOneMenuService, OneMenuService>();
            services.AddScoped<ProcessMessage, ProcessMessage>();
            services.AddScoped<UpdateHandler, UpdateHandler>();
            services.Add(new ServiceDescriptor(typeof(HttpClient), (_) => GetClient(), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(ITelegramBotClient), (_) => GetChatbot(), ServiceLifetime.Scoped));
        }

        private ITelegramBotClient GetChatbot()
        {
            return botClient;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chatbot.Telegram.WebHook v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void SetUpChatBot()
        {
            var token = Environment.GetEnvironmentVariable("chatbotToken");
            botClient = new TelegramBotClient(token);
            
        }

        private static HttpClient GetClient()
        {
            var oneMenuServiceUrl = Environment.GetEnvironmentVariable("OneMenuServiceUrl") ?? string.Empty;

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };

            return new HttpClient(httpClientHandler) {BaseAddress = new Uri(oneMenuServiceUrl)};
        }
    }
}