using Microsoft.AspNetCore.Components;

namespace Oscar.Blazor.Library.Shared
{
    public partial class SpinnerControl : ComponentBase
    {
        [Parameter]
        public SpinnerType SpinnerStyle { get; set; } = SpinnerType.Default;

        [Parameter]
        public string SpinnerText { get; set; } = "Please Wait...";
    }

    public enum SpinnerType
    {
        Bounce,
        Cube,
        ChasingDots,
        Default,
        FoldingCube,
        Rotate,
        ThreeDots,
        Wave
    }
}
