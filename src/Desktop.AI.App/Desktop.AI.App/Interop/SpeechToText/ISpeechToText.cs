namespace Desktop.AI.App.Interop.SpeechToText
{
    public interface ISpeechToText : IAsyncDisposable
    {
        Task Initialise();
        void OnTextReceived(string text);
        Task Start(Action<string> onTextReceivedAction);
        Task Stop();
    }
}