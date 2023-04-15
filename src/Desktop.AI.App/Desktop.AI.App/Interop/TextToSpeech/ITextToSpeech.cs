using Desktop.AI.App.Models;

namespace Desktop.AI.App.Interop.TextToSpeech
{
    public interface ITextToSpeech : IAsyncDisposable
    {
        Task Cancel();
        Task<List<Voice>> GetVoices();
        Task Initialise();
        Task Speak(string text, string voiceUri, CancellationToken canellationToken);
    }
}