using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Desktop.AI.App.Interop.SpeechToText
{
    public class SpeechToText : ISpeechToText
    {
        private readonly IJSRuntime _jsRuntime;

        private const string _modulePath = "./interop/SpeechToText/SpeechToText.js";

        private IJSObjectReference _module = null!;
        private IJSObjectReference _speechToText = null!;
        private Action<string> _onTextReceivedAction = null!;
        DotNetObjectReference<SpeechToText> _dotNetObjectReference
        {
            get; set;
        } = null!;

        public SpeechToText(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task Initialise()
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
            _module = await _jsRuntime!.InvokeAsync<IJSObjectReference>("import", _modulePath);
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
            _onTextReceivedAction?.Invoke(text);
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

            _dotNetObjectReference?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
