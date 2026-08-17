using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Oscar.Blazor.Components
{
    public class EventConsoleComponent : ComponentBase
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, dynamic> Attributes { get; set; }

        [Parameter]
        public string Title { get; set; } = "Console";

        [Parameter] public bool Visible { get; set; } = true;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        protected class Message
        {
            public DateTime Date { get; set; }
            public string Text { get; set; }
        }

        protected IList<Message> Messages { get; set; } = new List<Message>();

        public bool HasMessages => Messages.Count > 0;

        protected void OnClearClick()
        {
            Clear();
        }

        public void Clear()
        {
            Messages.Clear();
            InvokeAsync(StateHasChanged);
        }

        public void Log(string message)
        {
            Messages.Add(new Message { Date = DateTime.Now, Text = message });
            InvokeAsync(StateHasChanged);
        }

        public void Log(string eventName, string message)
        {
           Log($"{eventName}: {message}");
        }
        public void LogError(string message)
        {
            Log($"[ERROR]: {message}");
        }
        public void LogException(Exception exception)
        {
            Log($"[ERROR]: {exception}");
        }
        public void Log(object value)
        {
            Log(JsonSerializer.Serialize(value, new JsonSerializerOptions() { WriteIndented = true }));
        }

    }
}
