using Desktop.AI.App.Models;
using Microsoft.JSInterop;

namespace Desktop.AI.App.Interop.TextToSpeech
{
    public class TextToSpeech :  ITextToSpeech
    {
        private const string _modulePath = "./interop/TextToSpeech/TextToSpeech.js";
        private readonly IJSRuntime _jSRuntime;
        private IJSObjectReference _module = null!;
        private IJSObjectReference _textToSpeech = null!;

        public TextToSpeech(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task Initialise()
        {
            _module = await _jSRuntime!.InvokeAsync<IJSObjectReference>("import", _modulePath);
            _textToSpeech = await _module.InvokeAsync<IJSObjectReference>("createTextToSpeech");
        }

        public async Task Speak(string text, string voiceUri, CancellationToken canellationToken)
        {
            try
            {
                await _textToSpeech.InvokeAsync<object>("speak", canellationToken, text, voiceUri);
            }
            catch (TaskCanceledException) { }
        }

        public async Task Cancel()
        {
            await _textToSpeech.InvokeVoidAsync("cancel");
        }

        public async Task<List<Voice>> GetVoices()
        {
            var voices = await _textToSpeech.InvokeAsync<List<Voice>>("getVoices", CancellationToken.None);
            return voices;
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.DisposeAsync();
            }

            if (_textToSpeech != null)
            {
                await _textToSpeech.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
