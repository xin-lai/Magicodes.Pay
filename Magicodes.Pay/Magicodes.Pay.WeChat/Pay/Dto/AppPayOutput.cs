using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.WeChat.Pay.Dto
{
    public class AppPayOutput
    {
        public string AppId { get; set; }

        public string MchId { get; set; }

        public string TimeStamp { get; set; }

        public string NonceStr { get; set; }

        public string PrepayId { get; set; }

        public string Package { get; set; }

        public string SignType { get; set; }
        public string PaySign { get; internal set; }
    }
}
