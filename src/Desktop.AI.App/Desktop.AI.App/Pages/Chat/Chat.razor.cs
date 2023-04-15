using Desktop.AI.App.Interop.SpeechToText;
using Desktop.AI.App.Interop.TextToSpeech;
using Desktop.AI.App.Models;
using Desktop.AI.App.Pages.Chat.Components;
using Microsoft.AspNetCore.Components;
using OpenAI.Net;

namespace Desktop.AI.App.Pages.Chat
{
    public partial class Chat : ComponentBase
    {
        [Inject]
        private IOpenAIService? OpenAIService { get; init; } 

        [Inject]
        private ISpeechToText? SpeechToText { get; init; }

        [Inject]
        private ITextToSpeech? TextToSpeech { get; init; }


        private bool _isConverstionContextVisible = true;
        private bool _isBusy = false;
        private string _statusText = string.Empty;
        private string? _errorText = string.Empty;
        readonly SearchModel _searchModel = new()
        {
            SearchText = "write me a simple class that adds 2 numbers together and write a unit text using xunit to test it",
            SystemContext = "You are a code generator, when generating code markdown ensure to include the language e.g c# should start with ```csharp. dont give any explanations or description code only",
            AssistantContext = ""
        };
        readonly Conversation _conversation = new();
        private bool _isRecording = false;
        private string _voiceRecordingPartial = string.Empty;
        //private List<Voice> _voices = new List<Voice>();
        private CancellationTokenSource? _textToSpeechCancellationTokenSource;
        private bool _isTextToSpeechEnabled { get; set; } = true;
        private bool _isSpeaking { get; set; } = true;
        private string _selectedVoiceUri = "";
        private VoiceSelectionModal? _voiceSelectionModal;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
               InitialiseSpeechToText();
               InitialiseTextToSpeech();
            }
        }

        private async void InitialiseSpeechToText()
        {
              await SpeechToText!.Initialise();
        }

        private async void InitialiseTextToSpeech()
        {
            
            await TextToSpeech!.Initialise();
            //this._voices = await TextToSpeech.GetVoices();
            StateHasChanged();
        }

        private async Task Search()
        {
            _conversation.AddItem("User", _searchModel.SearchText);

            var messagesRequest = new List<Message>
            {
                Message.Create(ChatRoleType.System, _searchModel.SystemContext),
                Message.Create(ChatRoleType.Assistant, _searchModel.AssistantContext),
                Message.Create(ChatRoleType.User, _searchModel.SearchText)
            };

            _conversation.AddItem("AI", "");

            SetIsBusy(true, "Generating text..");

            try
            {
                _searchModel.SearchText = string.Empty;

                await foreach (var result in OpenAIService!.Chat.GetStream(messagesRequest, o =>
                {
                    o.N = 1;
                    o.MaxTokens = 2048;
                }))
                {
                    _conversation.AppendToCurrentItem(result.Result!.Choices[0].Delta.Content);
                   
                    StateHasChanged();
                }

                _searchModel.AssistantContext += _conversation.GetCurrentItemMessage();
            }
            catch
            {
                _errorText = "An error has occurred";
            }
            finally
            {
                SetIsBusy(false);
                Speak(_conversation.GetCurrentItemMessage());
            }
        }


        private async Task GenerateImage()
        {
            SetIsBusy(true, "Generating Image");

            var imageResponse = await OpenAIService!.Images.Generate(_searchModel.SearchText, 1);
            if (imageResponse.IsSuccess)
            {
                _conversation.AddItem("AI", $@"<img src=""{imageResponse?.Result?.Data[0].Url}"" alt=""drawing"" width=""400"" />");
            }
            else
            {
                _errorText = imageResponse?.ErrorResponse?.Error?.Message;
            }
            SetIsBusy(false);
        }

        private async void Speak(string text)
        {
            if (this._isTextToSpeechEnabled)
            {
                _textToSpeechCancellationTokenSource = new CancellationTokenSource();
                this._isSpeaking = true;
                await TextToSpeech.Speak(text, this._selectedVoiceUri, _textToSpeechCancellationTokenSource.Token);
                this._isSpeaking = false;
                StateHasChanged();
            }
        }

        private void OnSelectVoiceClicked()
        {
            _voiceSelectionModal?.ShowAsync();
        }

        private void OnVoiceSelected(Voice voice)
        {
            this._selectedVoiceUri = voice.VoiceURI;
        }

        private void OnTextToSpeechToggleClicked()
        {
            _isTextToSpeechEnabled = !_isTextToSpeechEnabled;
        }

        private async void OnStopSpeakingClicked()
        {
            if (this._isTextToSpeechEnabled)
            {
                await TextToSpeech!.Cancel();
                this._isSpeaking = false;
                _textToSpeechCancellationTokenSource?.Cancel();
                StateHasChanged();
            }
        }


        private async Task OnRecordClicked()
        {
            _isRecording = true;
            await SpeechToText!.Start((text) =>
            {
                OnVoiceTextReceived(text);
            });
        }

        private async Task OnStopRecordingClicked()
        {
            _voiceRecordingPartial = string.Empty;
            _isRecording = false;
            await SpeechToText!.Stop();
        }

        private void OnVoiceTextReceived(string text)
        {
            if (!_isRecording)
            {
                _searchModel.SearchText = text;
            }
            else
            {
                _voiceRecordingPartial = text;
            }

            StateHasChanged();
        }

        private void SetIsBusy(bool isBusy, string statusText = "")
        {
            _isBusy = isBusy;
            _statusText = statusText;
        }

        private void OnHideSidePanelClicked()
        {
            _isConverstionContextVisible = !_isConverstionContextVisible;
        }
    }
}
