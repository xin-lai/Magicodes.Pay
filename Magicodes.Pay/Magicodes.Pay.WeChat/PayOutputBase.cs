// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : PayOutputBase.cs
//           description :
//   
//           created by 雪雁 at  2018-07-16 15:46
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Xml.Serialization;

namespace Magicodes.Pay.WeChat.Pay.Dto
{
    [XmlRoot("xml")]
    [Serializable]
    public abstract class PayOutputBase
    {
        /// <summary>
        ///     业务结果  SUCCESS/FAIL
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        ///     错误代码
        /// </summary>
        [XmlElement("err_code")]
        public string ErrCode { get; set; }

        /// <summary>
        ///     错误代码描述
        /// </summary>
        [XmlElement("err_code_des")]
        public string ErrCodeDes { get; set; }


        /// <summary>
        ///     返回状态码
        ///     SUCCESS/FAIL，此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        [XmlElement("return_code")]
        public string PayReturnCode { get; set; }

        /// <summary>
        ///     详细结果
        /// </summary>
        [XmlIgnore]
        public string DetailResult { get; set; }

        /// <summary>
        ///     返回信息，返回信息，如非空，为错误原因，签名失败，参数格式校验错误
        /// </summary>
        [XmlElement("return_msg")]
        public string Message { get; set; }

        /// <summary>
        ///     是否支付成功
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSuccess()
        {
            return PayReturnCode == "SUCCESS" && ResultCode == "SUCCESS";
        }

        /// <summary>
        ///     获取错误友好提示
        /// </summary>
        /// <returns></returns>
        public string GetFriendlyMessage()
        {
            return $"{ErrCode ?? ""}：{ErrCodeDes ?? Message}".TrimStart('：');
        }
    }
}