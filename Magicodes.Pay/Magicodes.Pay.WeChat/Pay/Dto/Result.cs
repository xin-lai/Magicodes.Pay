using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.WeChat.Pay.Dto
{

    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// 获取普通现金红包(裂变红包)发送接口的结果
    /// </summary>
    [XmlRoot("xml")]
    [Serializable]
    public class Result : PayOutputBase
    {
        /// <summary>
        /// Gets or sets the Sign
        /// 签名
        /// </summary>
        [XmlAttribute("sign")]
        public string Sign { get; set; }

        /// <summary>
        /// Gets or sets the ResultCode
        /// SUCCESS/FAIL
        /// </summary>
        [XmlAttribute("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// Gets or sets the ErrCode
        /// </summary>
        [XmlAttribute("err_code")]
        public string ErrCode { get; set; }

        /// <summary>
        /// Gets or sets the ErrCodeDes
        /// </summary>
        [XmlAttribute("err_code_des")]
        public string ErrCodeDes { get; set; }
    }
}
