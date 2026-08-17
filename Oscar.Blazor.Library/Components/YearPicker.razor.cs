using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Oscar.Blazor.Library.Components
{
    public partial class YearPicker
    {
        [Parameter]
        public Expression<Func<int?>> For { get; set; }

        [Parameter]
        public int? Value { get; set; }

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<int?> ValueChanged { get; set; }

        private async Task OnSelectChange(int? newValue)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

        private IEnumerable<int?> Years
        {
            get
            {
                for (var i = DateTime.Now.Year; i >= DateTime.Now.Year - 100; i--)
                {
                    yield return i;
                }
            }
        }
    }
}
