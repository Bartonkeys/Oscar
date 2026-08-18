using MudBlazor;

namespace Oscar.Blazor.Theme
{
    /// <summary>
    /// The shared MudBlazor theme for Oscar: a slate-and-indigo palette with softened
    /// elevation and rounded surfaces, tuned separately for light and dark mode.
    /// </summary>
    public static class OscarTheme
    {
        private static readonly string[] FontStack =
        {
            "Inter", "-apple-system", "BlinkMacSystemFont", "Segoe UI",
            "Roboto", "Helvetica Neue", "Arial", "sans-serif"
        };

        public static MudTheme Build() => new()
        {
            PaletteLight = Light,
            PaletteDark = Dark,
            LayoutProperties = Layout,
            Typography = Fonts
        };

        private static PaletteLight Light => new()
        {
            Primary = "#4F46E5",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#0D9488",
            SecondaryContrastText = "#FFFFFF",
            Tertiary = "#7C3AED",
            Info = "#2563EB",
            Success = "#15A34A",
            Warning = "#D97706",
            Error = "#DC2626",

            Background = "#F6F8FB",
            BackgroundGray = "#EEF2F7",
            Surface = "#FFFFFF",

            AppbarBackground = "#FFFFFF",
            AppbarText = "#0F172A",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#334155",
            DrawerIcon = "#64748B",

            TextPrimary = "#0F172A",
            TextSecondary = "#5A6B84",
            TextDisabled = "#94A3B8",

            ActionDefault = "#64748B",
            ActionDisabled = "#CBD5E1",
            ActionDisabledBackground = "#F1F5F9",

            Divider = "#E3E8F0",
            DividerLight = "#EEF2F7",
            TableLines = "#E3E8F0",
            LinesDefault = "#E3E8F0",
            LinesInputs = "#CBD5E1",

            HoverOpacity = 0.05
        };

        private static PaletteDark Dark => new()
        {
            Primary = "#818CF8",
            PrimaryContrastText = "#0B1120",
            Secondary = "#2DD4BF",
            SecondaryContrastText = "#0B1120",
            Tertiary = "#A78BFA",
            Info = "#60A5FA",
            Success = "#4ADE80",
            Warning = "#FBBF24",
            Error = "#F87171",

            Background = "#0B1120",
            BackgroundGray = "#0F1729",
            Surface = "#141D31",

            AppbarBackground = "#141D31",
            AppbarText = "#E8EDF6",
            DrawerBackground = "#141D31",
            DrawerText = "#C7D2E1",
            DrawerIcon = "#94A3B8",

            TextPrimary = "#E8EDF6",
            TextSecondary = "#9AABC2",
            TextDisabled = "#64748B",

            ActionDefault = "#94A3B8",
            ActionDisabled = "#3A4459",
            ActionDisabledBackground = "#1B2438",

            Divider = "#24304A",
            DividerLight = "#1B2438",
            TableLines = "#24304A",
            LinesDefault = "#24304A",
            LinesInputs = "#38445F",

            HoverOpacity = 0.08
        };

        private static LayoutProperties Layout => new()
        {
            DefaultBorderRadius = "10px",
            AppbarHeight = "60px",
            DrawerWidthLeft = "264px",
            DrawerMiniWidthLeft = "72px"
        };

        private static Typography Fonts => new()
        {
            Default = new DefaultTypography
            {
                FontFamily = FontStack,
                FontSize = "0.875rem",
                FontWeight = "400",
                LineHeight = "1.5",
                LetterSpacing = "normal"
            }
        };
    }
}
