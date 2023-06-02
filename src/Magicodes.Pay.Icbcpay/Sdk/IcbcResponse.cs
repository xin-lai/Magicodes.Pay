using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Magicodes.Pay.Icbcpay
{
    [Serializable]
    [DataContract]
    public class IcbcResponse
    {
        [DataMember]
        private int return_code = -1;
        [DataMember]
        private String return_msg = "unexpected error.";
        [DataMember]
        private String msg_id;

        public String getMsgId()
        {
            return msg_id;
        }
        public void setMsgId(String value)
        {
            msg_id = value;
        }

        public int getReturnCode()
        {
            return return_code;
        }
        public void setReturnCode(int value)
        {
            return_code = value;
        }

        public String getReturnMsg()
        {
            return return_msg;
        }
        public void setReturnMsg(String value)
        {
            return_msg = value;
        }

        /**
	     * @return 技术上是否成功，如接口约定业务标志，则需进一步判断
	     */
        public Boolean isSuccess()
        {
            return return_code == 0;
        }
    }
}
