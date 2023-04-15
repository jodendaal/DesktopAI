using Microsoft.JSInterop;

namespace Desktop.AI.App.Interop.SpeechToText
{
    public class SpeechToText : IAsyncDisposable
    {
        private const string _modulePath = "./interop/SpeechToText.js";
        private readonly IJSRuntime _jSRuntime;
        private IJSObjectReference _module;
        private IJSObjectReference _speechToText;
        private Action<string> _onTextReceivedAction;
        DotNetObjectReference<SpeechToText> _dotNetObjectReference
        {
            get; set;
        }
        public SpeechToText(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
            _dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        public async Task Initialise()
        {
            _module = await _jSRuntime!.InvokeAsync<IJSObjectReference>("import", _modulePath);
            _speechToText = await _module.InvokeAsync<IJSObjectReference>("createSpeechToText");
        }

        public async Task Start(Action<string> onTextReceivedAction)
        {
            _onTextReceivedAction = onTextReceivedAction;
            await _speechToText.InvokeAsync<object>("startProxy", _dotNetObjectReference, nameof(OnTextReceived));
        }

        public async Task Stop()
        {
            await _speechToText.InvokeVoidAsync("stop");
        }

        [JSInvokable]
        public void OnTextReceived(string text)
        {
            if (_onTextReceivedAction != null)
            {
                _onTextReceivedAction(text);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.DisposeAsync();
            }

            if (_speechToText != null)
            {
                await _speechToText.DisposeAsync();
            }

            if (_dotNetObjectReference != null)
            {
                _dotNetObjectReference.Dispose();
            }
        }
    }
}
