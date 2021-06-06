using System.Threading.Tasks;
using Chatbot.Telegram.Core.Handlers;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Chatbot.Telegram.WebHook.Controllers
{
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly UpdateHandler _updateHandler;
        private readonly ITelegramBotClient _botClient;

        public UpdateController(UpdateHandler updateHandler, ITelegramBotClient botClient)
        {
            _updateHandler = updateHandler;
            _botClient = botClient;
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _updateHandler.HandleUpdate(_botClient, update);
            return Ok();
        }
    }
}
