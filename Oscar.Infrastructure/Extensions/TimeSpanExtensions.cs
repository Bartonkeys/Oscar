using System.Text;

namespace Oscar.Infrastructure.Extensions;

public static class TimeSpanExtensions
{
    public static string ToDuration(this TimeSpan elapsed)
    {
        StringBuilder duration = new StringBuilder();

        if (elapsed.Hours > 0)
        {
            duration.AppendFormat("{0} hour{1}", elapsed.Hours, elapsed.Hours > 1 ? "s" : "");
        }
        if (elapsed.Minutes > 0)
        {
            duration.AppendFormat(" {0} min{1}", elapsed.Minutes, elapsed.Minutes > 1 ? "s" : "");
        }
        if (elapsed.Seconds > 0)
        {
            duration.AppendFormat(" {0} sec{1}", elapsed.Seconds, elapsed.Seconds > 1 ? "s" : "");
        }
        if (duration.ToString().Length == 0)
        {
            duration.AppendFormat(" {0} ms", Math.Abs(elapsed.Milliseconds));
        }
        return duration.ToString().Trim();
    }
}