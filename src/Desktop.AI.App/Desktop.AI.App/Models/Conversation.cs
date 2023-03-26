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
    }

    public class ConversationItem
    {
        public string User { get; set; }
        public string Message { get; set; }
    }
}
