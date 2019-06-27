using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Allinpay
{
    public class HttpRequestUtil
    {
        public static async Task<string> PostAsync(string url, Dictionary<string, string> data)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(data);
            var respMsg = await client.PostAsync("url", content);
            var msgBody = await respMsg.Content.ReadAsStringAsync();
            return msgBody;
        }
    }
}
