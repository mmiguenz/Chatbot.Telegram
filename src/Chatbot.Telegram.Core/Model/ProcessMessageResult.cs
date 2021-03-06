using System.Collections;
using System.Collections.Generic;

namespace ChatbotTelegram.Model
{
    public class ProcessMessageResult
    {
        public bool HasErrors { get; set; }
        public Step CurrentStep { get; set; }
        public string CompletionMsg { get; set; }
        public bool IsCompleted { get; set; }
        public IEnumerable<string> ValidationErrors { get; set; }
        public IEnumerable<Menu> AvailableMenus;
    }
}