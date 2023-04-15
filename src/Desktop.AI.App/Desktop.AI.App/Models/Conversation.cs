namespace Desktop.AI.App.Models
{
    public class Conversation
    {
        public List<ConversationItem> ConversationHistory { get; set; } = new List<ConversationItem>();

        public void AddItem(string user,string message)
        {
            ConversationHistory.Add(new ConversationItem(user,message));
        }

        public void AppendToCurrentItem(string message)
        {
            ConversationHistory[ConversationHistory.Count - 1].AppendToMessage(message);
        }

        public string GetCurrentItemMessage()
        {
            return ConversationHistory[ConversationHistory.Count - 1].Message;
        }
    }

    public class ConversationItem
    {
        public ConversationItem(string user, string message)
        {
            User = user;
            Message = message;
        }

        public string User { get; init; }
        public string Message { get; private set; }

        public void AppendToMessage(string message) 
        {
            this.Message += message;
        }
    }
}
