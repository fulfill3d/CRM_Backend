using Microsoft.Extensions.Primitives;

namespace CRM.API.Client.Service.Data.Extensions
{
    public static class QueryExtensions
    {
        public static int TryGetIntValueOrDefault(this IDictionary<string, StringValues> keyValues, string key, int defaultValue)
        {
            if (keyValues.TryGetValue(key, out var value) && int.TryParse(value, out var intValue))
            {
                return intValue;
            }

            return defaultValue;
        }
    }
}