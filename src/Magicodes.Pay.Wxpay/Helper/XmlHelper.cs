// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : XmlHelper.cs
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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Magicodes.Pay.Wxpay.Helper
{
    public static class XmlHelper
    {
        /// <summary>
        ///     XML序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                var xmlSerial = new XmlSerializer(obj.GetType());
                //序列化对象
                xmlSerial.Serialize(stream, obj);
                stream.Position = 0;
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        ///     序列化XML不带命名空间和定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObjectWithoutNamespace<T>(T obj)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = true,
            };
            using (var xmlWriter = XmlWriter.Create(sb, settings))
            {
                //去除默认命名空间xmlns:xsd和xmlns:xsi
                var xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(xmlWriter, obj, xmlSerializerNamespaces);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var str = reader.ReadToEnd();
                str = str.Replace("gb2312", "utf-8");
                var xmlSerial = new XmlSerializer(typeof(T));
                using (var rdr = new StringReader(str))
                {
                    return (T)xmlSerial.Deserialize(rdr);
                }
            }
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string input) where T : class
        {
            if (!input.StartsWith("<?xml"))
                input = @"<?xml version=""1.0"" encoding=""utf-8""?>" + input;
            using (var memoryStream = new MemoryStream(Encoding.Default.GetBytes(input)))
            {
                return DeserializeObject<T>(memoryStream);
                //using (var reader = XmlReader.Create(mem))
                //{
                //    var formatter = new XmlSerializer(typeof(T));
                //    return formatter.Deserialize(reader) as T;
                //}
            }
        }
    }
}