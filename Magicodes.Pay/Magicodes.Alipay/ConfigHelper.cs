using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Alipay
{
    public class ConfigHelper
    {
        ///// <summary>
        ///// 获取支付配置
        ///// </summary>
        ///// <param name="configuration"></param>
        ///// <returns></returns>
        //public static AlipaySettings GetAlipaySettings(IConfigurationRoot configuration)
        //{
        //    var alipaySettings = new AlipaySettings();
        //    if (configuration["alipay:GlobalSetting:IsEnabled"] != null && Convert.ToBoolean(configuration["alipay:GlobalSetting:IsEnabled"]))
        //    {
        //        alipaySettings.AlipayPublicKey = configuration["alipay:GlobalSetting:AlipayPublicKey"];
        //        alipaySettings.AppId = configuration["alipay:GlobalSetting:AppId"];
        //        if (!configuration["alipay:GlobalSetting:CharSet"].IsNullOrWhiteSpace())
        //            alipaySettings.CharSet = configuration["alipay:GlobalSetting:CharSet"];

        //        if (!configuration["alipay:GlobalSetting:Notify"].IsNullOrWhiteSpace())
        //            alipaySettings.Notify = configuration["alipay:GlobalSetting:Notify"];

        //        if (!configuration["alipay:GlobalSetting:Gatewayurl"].IsNullOrWhiteSpace())
        //            alipaySettings.Gatewayurl = configuration["alipay:GlobalSetting:Gatewayurl"];

        //        //alipaySettings.IsKeyFromFile = configuration["alipay:GlobalSetting:IsKeyFromFile"] == null ? false : Convert.ToBoolean(configuration["alipay:GlobalSetting:IsKeyFromFile"]);

        //        alipaySettings.PrivateKey = configuration["alipay:GlobalSetting:PrivateKey"];

        //        if (!configuration["alipay:GlobalSetting:SignType"].IsNullOrWhiteSpace())
        //            alipaySettings.SignType = configuration["alipay:GlobalSetting:SignType"];
        //        alipaySettings.Uid = configuration["alipay:GlobalSetting:Uid"];
        //    }
        //    else
        //    {
        //        //TODO:从系统设置中获取
        //    }
        //    return alipaySettings;
        //}
    }
}
