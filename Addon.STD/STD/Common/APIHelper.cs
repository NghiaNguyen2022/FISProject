using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using PN.SmartLib.Helper;
using RestSharp;
using SAPCore;
using SAPCore.Config;
using STD.DataReader;
using STDApp.Models;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace STDApp.Common
{
    public class APIHelper
    {
        public static string GetToken(ref string message)
        {
            try
            {
                var query = string.Format(QueryString.GetAPICode);
                var data = DataProvider.QuerySingle(CoreSetting.DataConnection, query);
                if (data == null)
                {
                    message = "Không thể load được code của API";
                    return string.Empty;
                }
                var code = data["Code"].ToString();
                if (string.IsNullOrEmpty(code))
                {
                    message = "Không thể load được code của API";
                    return string.Empty;
                }

                var options = new RestClientOptions(APIBIDVConstrant.APILink)// ConfigurationManager.AppSettings["LinkBIDVAPI"])
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest(APIBIDVConstrant.AuthenBIDV// ConfigurationManager.AppSettings["AuthenBIDV"]
                                            , Method.Post);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("client_id", APIBIDVConstrant.ClientID);// ConfigurationManager.AppSettings["ClientIDBIDV"]);
                //request.AddParameter("client_secret", APIBIDVConstrant.ClientSecret);//ConfigurationManager.AppSettings["ClientSecretBIDV"]);
                request.AddParameter("code", code);
                //request.AddParameter("redirect_uri", APIBIDVConstrant.URL_Redirect);// ConfigurationManager.AppSettings["redirect_uri"]);

                var response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var error = JsonSerializer.Deserialize<BIDVAccesstokenResponseErrro>(response.Content);
                    if (error.error_description.Contains("code expired"))
                    {
                        message = $"Code để lấy token đã bị hết hạn, vui lòng lấy lại code mới và thử lại";//, UIHelper.MsgType.StatusBar, true);
                        return string.Empty;
                    }
                    else
                    {
                        message = $"Lỗi {error.error_description}";
                        return string.Empty;
                    }
                }
                else
                {
                    var result = JsonSerializer.Deserialize<BIDVAccesstokenResponse>(response.Content);
                    if (result != null)
                        return result.access_token;
                }
            }
            catch (Exception ex)
            {
                message = $"Lỗi {ex.Message}";
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
