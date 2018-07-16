using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Magicodes.Pay.WeChat.Pay.Dto
{

    public class MiniProgramPayOutput
    {
        public string AppId { get; set; }

        public string TimeStamp { get; set; }

        public string NonceStr { get; set; }

        public string Package { get; set; }

        public string SignType { get; set; }
        public string PaySign { get; internal set; }
    }
}
