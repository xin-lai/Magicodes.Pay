using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Magicodes.Pay.Icbcpay
{
    class IcbcSignature
    {

        public static String sign(String content, String signType, String privateKey, String charset)
        {
            return sign(content, signType, privateKey, charset, null);
        }
        public static String sign(String content, String signType, String privateKey, String charset, String password)
        {
            Encoding encode = Encoding.GetEncoding(charset);
            if (signType.Equals(IcbcConstants.SIGN_TYPE_CA))
            {
             }
            else if (signType.Equals(IcbcConstants.SIGN_TYPE_RSA))
            {
                return RSAClass.RSAFromPkcs8.sign(content, privateKey, "UTF-8");

            }
            else if (signType.Equals(IcbcConstants.SIGN_TYPE_RSA2))
            {
                return RSAClass.RSAFromPkcs8.signWithRSASHA256(content, privateKey, "UTF-8");

            }
            return null;
        }

        public static Boolean verify(String content, String signType, String publicKey, String charset, String sign)
        {
            try
            {
                byte[] contentBytes = Encoding.GetEncoding(charset).GetBytes(content);
                if (signType.Equals(IcbcConstants.SIGN_TYPE_CA))
                {
                    //return RSAClass.RSAFromPkcs8.verifySHA1(content, sign, publicKey, charset);
                    //int ret = icbcCA.verifySign_all(1,content, publicKey, sign);//CA证书时，publicKey字段传cer文件的路径
                    //if (ret == 0)
                    //    return true;
                    //else
                    //    return false;

                }
                else if (signType.Equals(IcbcConstants.SIGN_TYPE_RSA))
                {
                    return RSAClass.RSAFromPkcs8.verifySHA1(content, sign, publicKey, charset);

                }
                else if (signType.Equals(IcbcConstants.SIGN_TYPE_RSA2))
                {
                    return RSAClass.RSAFromPkcs8.verifySHA256(content, sign, publicKey, charset);

                }
                throw new Exception("not support signType.");
            }
            catch (Exception e)
            {
                throw new Exception("get content charset exception. content: " + content + " charset: " + charset,
                        e);
            }

        }

        //public static string RSAPrivateKeyJava2DotNet(String privateKey)
        //{
        //    RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
        //    return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
        //    Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
        //    Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
        //    Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
        //    Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
        //    Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
        //    Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
        //    Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
        //    Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        //}
        //AES内容解密
        public static String DecryptContent(String content, String encryptType, String encryptKey, String charset)
        {
            if (encryptType.Equals(IcbcConstants.ENCRYPT_TYPE_AES))
                return AesDecrypt(content, encryptKey);
            else
                throw new Exception("当前不支持该算法类型：encrypeType=" + encryptType);
        }
        //AES内容加密
        public static String EncryptContent(String content, String encryptType, String encryptKey, String charset)
        {
            if (encryptType.Equals(IcbcConstants.ENCRYPT_TYPE_AES))
                return AesEncrypt(content, encryptKey);
            else
                throw new Exception("当前不支持该算法类型：encrypeType=" + encryptType);
        }
        #region AES加解密
        /// <summary>
        ///AES加密（加密步骤）
        ///1，加密字符串得到2进制数组；
        ///2，将2禁止数组转为16进制；
        ///3，进行base64编码
        /// </summary>
        /// <param name="toEncrypt">要加密的字符串</param>
        /// <param name="key">密钥</param>
        public static String AesEncrypt(String toEncrypt, String key)
        {
            Byte[] _Key = Convert.FromBase64String(key);
            Byte[] _Source = Encoding.UTF8.GetBytes(toEncrypt);
            Aes aes = Aes.Create("AES");
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _Key;
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            ICryptoTransform cTransform = aes.CreateEncryptor();
            Byte[] cryptData = cTransform.TransformFinalBlock(_Source, 0, _Source.Length);
            String CryptString = Convert.ToBase64String(cryptData);
            return CryptString;
        }
        /// <summary>
        /// AES解密（解密步骤）
        /// 1，将BASE64字符串转为16进制数组
        /// 2，将16进制数组转为字符串
        /// 3，将字符串转为2进制数据
        /// 4，用AES解密数据
        /// </summary>
        /// <param name="encryptedSource">已加密的内容</param>
        /// <param name="key">密钥</param>
        public static String AesDecrypt(string encryptedSource, string key)
        {
            Byte[] _Key = Convert.FromBase64String(key);
            Aes aes = Aes.Create("AES");
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _Key;
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            ICryptoTransform cTransform = aes.CreateDecryptor();
            Byte[] encryptedData = Convert.FromBase64String(encryptedSource);
            Byte[] originalSrouceData = cTransform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            String originalString = Encoding.UTF8.GetString(originalSrouceData);
            return originalString;
        }
        /// <summary>
        /// 2进制转16进制
        /// </summary>
        static String Hex_2To16(Byte[] bytes)
        {
            String hexString = String.Empty;
            Int32 iLength = 65535;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();


                if (bytes.Length < iLength)
                {
                    iLength = bytes.Length;
                }


                for (int i = 0; i < iLength; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }


        /// <summary>
        /// 16进制转2进制
        /// </summary>
        static Byte[] Hex_16To2(String hexString)
        {
            if ((hexString.Length % 2) != 0)
            {
                hexString += " ";
            }
            Byte[] returnBytes = new Byte[hexString.Length / 2];
            for (Int32 i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        #endregion

    }
}
