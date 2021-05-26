using System.Threading.Tasks;

namespace ChatbotTelegram.handlers
{
    public interface IChatBotRequestHandler
    {
        Task Handle();
    }
}