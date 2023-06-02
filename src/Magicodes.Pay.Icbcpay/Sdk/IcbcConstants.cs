using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.Icbcpay
{
	public class IcbcConstants
	{
		public static String SIGN_TYPE = "sign_type";

		public static String SIGN_TYPE_RSA = "RSA";

		public static String SIGN_TYPE_RSA2 = "RSA2";

		public static String SIGN_TYPE_SM2 = "SM2";

		public static String SIGN_TYPE_CA = "CA";

		public static String SIGN_SHA1RSA_ALGORITHMS = "SHA1WithRSA";

		public static String SIGN_SHA256RSA_ALGORITHMS = "SHA256WithRSA";

		public static String ENCRYPT_TYPE_AES = "AES";

		public static String APP_ID = "app_id";

		public static String FORMAT = "format";

		public static String TIMESTAMP = "timestamp";

		public static String SIGN = "sign";

		public static String APP_AUTH_TOKEN = "app_auth_token";

		public static String CHARSET = "charset";

		public static String NOTIFY_URL = "notify_url";

		public static String RETURN_URL = "return_url";

		public static String ENCRYPT_TYPE = "encrypt_type";

		// -----===-------///

		public static String BIZ_CONTENT_KEY = "biz_content";

		/** 默认时间格式 **/
		public static String DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

		/** Date默认时区 **/
		public static String DATE_TIMEZONE = "GMT+8";

		/** UTF-8字符集 **/
		public static String CHARSET_UTF8 = "UTF-8";

		/** GBK字符集 **/
		public static String CHARSET_GBK = "GBK";

		/** JSON 应格式 */
		public static String FORMAT_JSON = "json";

		/** XML 应格式 */
		public static String FORMAT_XML = "xml";

		public static String CA = "ca";

		public static String PASSWORD = "password";

		public static String RESPONSE_BIZ_CONTENT = "response_biz_content";

		/** 消息唯一编号 **/
		public static String MSG_ID = "msg_id";

		/** sdk版本号在header中的key */
		public static String VERSION_HEADER_NAME = "APIGW-VERSION";

	}
}
