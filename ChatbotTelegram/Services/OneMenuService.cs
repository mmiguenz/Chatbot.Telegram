using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ChatbotTelegram.Model;
using Newtonsoft.Json;

namespace ChatbotTelegram.Services
{
    public class OneMenuService : IOneMenuService
    {
        private readonly HttpClient _httpClient;

        public OneMenuService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> InitMenuTransaction(string menuLabel)
        {
            var uri = $"/menutransaction/{menuLabel}";
            var response =  await _httpClient.PostAsync(uri, null);

            var result = await response.Content.ReadFromJsonAsync<MenuTransactionResult>();
            return  result?.MenuTransactionId ?? string.Empty;
        }

        public async Task<Step> GetCurrentStep(object menuTransactionId)
        {
            var uri = $"/menuTransaction/{menuTransactionId}/step";
            var response =  await _httpClient.GetAsync(uri);

            var result = await response.Content.ReadFromJsonAsync<Step>();

            return result;
        }

        public async Task<ProcessMessageResult> SaveResponse(string transactionId, string messageText)
        {
            var uri = $"/menuTransaction/{transactionId}/response";
            var data = new { response = messageText};
            var json = JsonConvert.SerializeObject(data);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response =  await _httpClient.PostAsync(uri, body);

            var result = await response.Content.ReadFromJsonAsync<ProcessMessageResult>();

            return result;
        }

        public async Task<IEnumerable<Menu>> GetAllMenus()
        {
            try
            {
                var uri = $"/menu/all";
                var response = await _httpClient.GetAsync(uri);

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<Menu>>();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        class MenuTransactionResult
        {
            public string MenuTransactionId { get; set; }
        }
    }
}