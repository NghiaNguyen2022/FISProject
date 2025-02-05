using Jose;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using STDApp.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace STDApp.Common
{
    public class APIUtil
    {
        public static string EncryptSHA512(string input)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (SHA512 hash = System.Security.Cryptography.SHA512.Create())
            {
                byte[] hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                StringBuilder hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (byte b in hashedInputBytes)
                {
                    hashedInputStringBuilder.Append(b.ToString("x2"));
                }
                return hashedInputStringBuilder.ToString();
            }
        }

        private static byte[] hexStringToBytes(String s)
        {
            byte[] ans = new byte[s.Length / 2];

            for (int i = 0; i < ans.Length; i++)
            {
                int index = i * 2;
                int val = Convert.ToInt32(s.Substring(index, 2), 16);
                ans[i] = (byte)val;
            }

            return ans;
        }
        public static byte[] HexToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length.");

            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                result[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return result;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string doEncryptJWE(string payload, string symmetricKey)
        {
            byte[] key = hexStringToBytes(symmetricKey);
            // Encrypt the payload using JWE
            string jwe = Jose.JWT.Encode(payload, key, JweAlgorithm.A256KW, JweEncryption.A128GCM);

            string[] parts = jwe.Split('.');

            Recipient recipient = new Recipient();
            recipient.header = new Header();
            recipient.encrypted_key = parts[1];

            Recipient[] recipients = new Recipient[1];
            recipients[0] = recipient;

            var jwePayload = new Req_000_JWEBIDVInquiryRequest();
            jwePayload.recipients = recipients;
            jwePayload.protectedField = parts[0];
            jwePayload.ciphertext = parts[3];
            jwePayload.iv = parts[2];
            jwePayload.tag = parts[4];

            var payloadRequest = JsonConvert.SerializeObject(jwePayload);          
            return payloadRequest;

        }
        //public static String doSignatureJWS(String payloadRequest)
        //{
        //    //--lay thong tin truyen vao header JWS
        //    RSA rsaAlg = RSA.Create();
        //    byte[] privateKeyByte = getRsaPrivateKeyEncodedFromPem(loadRsaPrivateKeyPem());
        //    int _out;
        //    object p = rsaAlg.ImportPkcs8PrivateKey(privateKeyByte, out _out);
        //    string TXT_DATA = Jose.JWT.Encode(payloadRequest, rsaAlg, Jose.JwsAlgorithm.RS256);
        //    char[] delimiterChars = { '.' };
        //    string[] words = TXT_DATA.Split(delimiterChars);
        //    string X_JWS_SIGNATURE = words[0] + ".." + words[2];
        //    Console.WriteLine(X_JWS_SIGNATURE);
        //    return X_JWS_SIGNATURE;
        //}
        public static string doSignatureJWS(string jwePayload)
        {
            // Load RSA private key
            RSA rsaKey = ImportPrivateKey(GetData());

            // Create JWS
            string jws = Jose.JWT.Encode(jwePayload, rsaKey, JwsAlgorithm.RS256);
            string[] parts = jws.Split('.');
            string X_JWS_SIGNATURE = parts[0] + ".." + parts[2];
            return X_JWS_SIGNATURE;
        }
        private static RSA ImportPrivateKey(string privateKeyPem)
        {
            using (var reader = new StringReader(loadRsaPrivateKeyPem()))
            {
                PemReader pemReader = new PemReader(reader);
                RsaPrivateCrtKeyParameters keyPair = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();
                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(keyPair);
                RSA rsa = RSA.Create();
                rsa.ImportParameters(rsaParams);
                return rsa;
            }
        }
        public static string DoSignatureJWS(string payloadRequest)
        {
            var privateKeyPem = GetData();
            AsymmetricKeyParameter keyParameter;
            byte[] data = getRsaPrivateKeyEncodedFromPem(privateKeyPem);
            using (var reader = new System.IO.StringReader(privateKeyPem))
            {
                var pemReader = new PemReader(reader);
                keyParameter = pemReader.ReadObject() as AsymmetricKeyParameter;
                if (keyParameter == null)
                {
                    throw new InvalidOperationException("Invalid private key format.");
                }
            }
            
            var rsa = DotNetUtilities.ToRSA((Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)keyParameter);
            string jwsToken = Jose.JWT.Encode(payloadRequest, rsa, Jose.JwsAlgorithm.RS256);
            char[] delimiterChars = { '.' };
            string[] words = jwsToken.Split(delimiterChars);
            string xJwsSignature = words[0] + ".." + words[2];

            //Console.WriteLine(xJwsSignature);
            return xJwsSignature;
        }


        static string Base64Encoding(byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        static byte[] Base64Decoding(String input)
        {
            return Convert.FromBase64String(input);
        }

        private static byte[] getRsaPrivateKeyEncodedFromPem(string rsaPrivateKeyPem)
        {
            string rsaPrivateKeyHeaderPem = "-----BEGIN PRIVATE KEY-----";
            string rsaPrivateKeyFooterPem = "-----END PRIVATE KEY-----";
            string rsaPrivateKeyDataPem = rsaPrivateKeyPem.Replace(rsaPrivateKeyHeaderPem, "").Replace(rsaPrivateKeyFooterPem, "").Replace("\n", "").Replace("\r", "").Trim();
            return Base64Decoding(rsaPrivateKeyDataPem);
        }
        public static string GetData()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "certificate.cer";

            var certificate = string.Empty;
            certificate = File.ReadAllText(path);
            var rsaPrivateKeyHeaderPem = "-----BEGIN CERTIFICATE-----";
            var rsaPrivateKeyFooterPem = "-----END CERTIFICATE-----";
            certificate = certificate.Replace(rsaPrivateKeyHeaderPem, "").Replace(rsaPrivateKeyFooterPem, "").Replace("\n\r", "").Replace("\r\n", "");
            return certificate;
        }

        private static string loadRsaPrivateKeyPem()
        {
            var certPath = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";
            var text = File.ReadAllText(certPath);
            return text;
           
        }
    }
}
