using OpenAI.Net.Models.Responses;

namespace Desktop.AI.App.Models
{
    public class Conversation
    {
        public List<ConversationItem> ConversationHistory { get; set; } = new List<ConversationItem>();

        public void AddItem(string user,string message)
        {
            ConversationHistory.Add(new ConversationItem()
            {
                User = user,
                Message = message
            }
            );
        }

        public void AppendToCurrentItem(string message)
        {
            this.ConversationHistory[this.ConversationHistory.Count - 1].Message += message;
        }

        public string GetCurrentItemMessage()
        {
            return this.ConversationHistory[this.ConversationHistory.Count - 1].Message;
        }
    }

    public class ConversationItem
    {
        public string User { get; set; }
        public string Message { get; set; }
    }
}
