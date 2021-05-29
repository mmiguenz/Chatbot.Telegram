using System.Collections.Generic;

namespace ChatbotTelegram.Model
{
    public class Step
    {
        public string Text { get; set; }
        public IEnumerable<Option> Options { get; set; }
    }
}