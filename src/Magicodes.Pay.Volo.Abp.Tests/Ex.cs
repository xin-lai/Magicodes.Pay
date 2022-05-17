using Newtonsoft.Json;

namespace Magicodes.Pay.Volo.Abp.Tests
{
    public static class Ex
    {
        public static T FromJsonString<T>(this string str) => JsonConvert.DeserializeObject<T>(str);
    }
}
