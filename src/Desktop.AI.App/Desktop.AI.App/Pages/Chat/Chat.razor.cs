using Desktop.AI.App.Models;
using Microsoft.AspNetCore.Components;
using OpenAI.Net;

namespace Desktop.AI.App.Pages.Chat
{
    public partial class Chat : ComponentBase
    {
        [Inject]
        private IOpenAIService? OpenAIService { get; init; }


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
            catch (Exception ex)
            {
                _errorText = "An error has occurred";
            }
            finally
            {
                SetIsBusy(false);
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
