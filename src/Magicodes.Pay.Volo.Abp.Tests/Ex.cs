using Newtonsoft.Json;

namespace Magicodes.Pay.Volo.Abp.Tests
{
    public static class Ex
    {
        public static T FromJsonString<T>(this string str) => JsonConvert.DeserializeObject<T>(str);


        public static string ToJsonString<T>(this T t)
        {
            return JsonConvert.SerializeObject(value: t);
        }
    }
}
