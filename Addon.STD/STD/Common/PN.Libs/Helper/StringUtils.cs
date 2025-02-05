using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PN.SmartLib.Helper
{
    public class StringUtils
    {
        private static string Key = "2s5u8x/A?D(G+KbPeShVmYq3t6w9y$B&";

        /// <summary>
        ///  Encode password
        /// </summary>
        /// <param name="plainText">pass or string to encode</param>
        /// <returns></returns>
        public static string EncryptString(string plainText)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(Key);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }

                return Convert.ToBase64String(array);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Decode password
        /// </summary>
        /// <param name="cipherText">hash string to decode</param>
        /// <returns></returns>
        public static string DecryptString(string cipherText)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(Key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static T Deserialize<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringReader reader = new StringReader(xml))
            {
                var xmlTextReader = new System.Xml.XmlTextReader(reader);
                xmlTextReader.Namespaces = false;
               // reader.Namespaces = false;
                return (T)serializer.Deserialize(xmlTextReader);
            }
        }

        public static string Serialize<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true, // You can set this to true if you want indented XML
                Encoding = Encoding.UTF8 // Set the desired encoding
            };
            using (StringWriter writer = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
                {
                    serializer.Serialize(xmlWriter, obj);
                    return writer.ToString();
                }
                //serializer.Serialize(writer, obj);
                ////return writer.ToString().Replace("<xml version=\"1.0\">", "").Replace("</xml>", "");
                //return writer.ToString();
            }
        }
        public static bool CheckFromDateEarlyToDate(string fromDateStr, string toDateStr)
        {
            DateTime fromDate, toDate;
            if (DateTime.TryParseExact(fromDateStr, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out fromDate) &&
                DateTime.TryParseExact(toDateStr, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out toDate))
            {
                if (toDate >= fromDate)
                    return true;
            }
            return false;
        }
    }
}
