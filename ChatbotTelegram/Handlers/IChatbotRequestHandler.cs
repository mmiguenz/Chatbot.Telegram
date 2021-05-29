using System.Threading.Tasks;

namespace ChatbotTelegram.Handlers
{
    public interface IChatBotRequestHandler
    {
        Task Handle();
    }
}