using Newtonsoft.Json;

namespace Oscar.Infrastructure.Features.Common
{
    public static class CloneHelper
    {
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}