using PN.SmartLib.Helper;
using RestSharp;
using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.Config;
using SAPCore.Helper;
using SAPCore.SAP;
using SAPCore.SAP.DIAPI;
using STD.DataReader;
using STDApp.Common;
using STDApp.Models;
using System;
using System.Text.Json;
using System.Web.Script.Serialization;

namespace STDApp.Payoo
{
    [FormAttribute("STDApp.Payoo.frmBatch", "Payoo/frmBatch.b1f")]
    class frmBatch : UserFormBase
    {
        public frmBatch()
        {
        }


        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblBank = ((SAPbouiCOM.StaticText)(this.GetItem("lblBank").Specific));
            this.cbbBank = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBank").Specific));
            this.cbbBank.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbBank_ComboSelectAfter);
            this.lblTrDa = ((SAPbouiCOM.StaticText)(this.GetItem("lblTrDa").Specific));
            this.cbbBat = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBat").Specific));
            this.lblBat = ((SAPbouiCOM.StaticText)(this.GetItem("lblBat").Specific));
            this.lblPag = ((SAPbouiCOM.StaticText)(this.GetItem("lblPag").Specific));
            this.txtTrDa = ((SAPbouiCOM.EditText)(this.GetItem("txtTrDa").Specific));
            this.txtPag = ((SAPbouiCOM.EditText)(this.GetItem("txtPag").Specific));
            this.btnLoad = ((SAPbouiCOM.Button)(this.GetItem("btnLoad").Specific));
            this.btnLoad.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnLoad_ClickBefore);
            this.grHdr = ((SAPbouiCOM.Grid)(this.GetItem("grHdr").Specific));
            this.grDt = ((SAPbouiCOM.Grid)(this.GetItem("grDt").Specific));
            this.btnSave = ((SAPbouiCOM.Button)(this.GetItem("btnSave").Specific));
            this.btnSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSave_ClickBefore);
            this.btnReload = ((SAPbouiCOM.Button)(this.GetItem("btnRel").Specific));
            this.btnReload.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnReload_ClickBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseAfter += new SAPbouiCOM.Framework.FormBase.CloseAfterHandler(this.Form_CloseAfter);
            this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);

        }

        private SAPbouiCOM.StaticText lblBank;

        private void OnCustomInitialize()
        {

        }

        private SAPbouiCOM.ComboBox cbbBank;
        private SAPbouiCOM.StaticText lblTrDa;
        private SAPbouiCOM.ComboBox cbbBat;
        private SAPbouiCOM.StaticText lblBat;
        private SAPbouiCOM.StaticText lblPag;
        private SAPbouiCOM.EditText txtTrDa;
        private SAPbouiCOM.EditText txtPag;
        private SAPbouiCOM.Button btnLoad;
        private SAPbouiCOM.Grid grHdr;

        public static bool IsFormOpen = false;
        private static frmBatch instance;

        private string TransDate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtTrDa);
            }
        }

        private int Page
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(txtPag.Value))
                        return 1;
                    var ret = 1;
                    int.TryParse(txtPag.Value, out ret);
                    return ret;
                }
                catch (Exception ex)
                {

                }
                return 1;
                // return UIHelper.GetTextboxValue(txtPag);
            }
        }

        private string Bank
        {
            get
            {
                return UIHelper.GetComboValue(cbbBank);
            }
        }
        private int BatchNo
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(cbbBat.Value))
                        return 1;
                    var ret = 1;
                    int.TryParse(cbbBat.Value, out ret);
                    return ret;
                }
                catch (Exception ex)
                {

                }
                return 1;
            }
        }
        public static void ShowForm()
        {
            try
            {
                if (instance == null)
                {
                    instance = new frmBatch();
                }
                instance.InitControl();
                instance.Show();
                IsFormOpen = true;
            }
            catch (Exception ex)
            {

            }
        }
        private void InitControl()
        {
            SetLocation();

            UIHelper.ComboboxSelectDefault(cbbBank);
            //this.btnSave.Item.Enabled = false;
            //cbbBank.Item.Enabled = false;
        }

        private void SetLocation()
        {
            var max = this.UIAPIRawForm.ClientHeight;
            var maxw = this.UIAPIRawForm.ClientWidth;

            this.lblBank.Item.Top = 15;
            this.lblBank.Item.Left = CoreSetting.UF_HorMargin;

            this.cbbBank.Item.Top = this.lblBank.Item.Top;
            this.cbbBank.Item.Left = this.lblBank.Item.Left + this.lblBank.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblTrDa.Item.Top = this.lblBank.Item.Top;
            this.lblTrDa.Item.Left = this.cbbBank.Item.Left + this.cbbBank.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.txtTrDa.Item.Top = this.lblBank.Item.Top;
            this.txtTrDa.Item.Left = this.lblTrDa.Item.Left + this.lblTrDa.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblBat.Item.Top = this.lblBank.Item.Top;
            this.lblBat.Item.Left = this.txtTrDa.Item.Left + this.txtTrDa.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.cbbBat.Item.Top = this.lblBank.Item.Top;
            this.cbbBat.Item.Left = this.lblBat.Item.Left + this.lblBat.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblPag.Item.Top = this.lblBank.Item.Top;
            this.lblPag.Item.Left = this.cbbBat.Item.Left + this.cbbBat.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.txtPag.Item.Top = this.lblBank.Item.Top;
            this.txtPag.Item.Left = this.lblPag.Item.Left + this.lblPag.Item.Width + CoreSetting.UF_HorizontallySpaced;

            var labBottom = this.lblBank.Item.Top + this.lblBank.Item.Height;
            var bttTop = labBottom - this.btnLoad.Item.Height;
            this.btnLoad.Item.Top = bttTop;
            this.btnLoad.Item.Left = this.cbbBat.Item.Left + this.cbbBat.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.btnSave.Item.Top = bttTop;
            this.btnSave.Item.Left = this.btnLoad.Item.Left + this.btnLoad.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.btnReload.Item.Top = bttTop;
            this.btnReload.Item.Left = this.btnSave.Item.Left + this.btnSave.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.grHdr.Item.Left = this.lblBank.Item.Left;
            this.grHdr.Item.Width = maxw - grHdr.Item.Left - 40;
            this.grHdr.Item.Top = lblBank.Item.Top + lblBank.Item.Height + 20;
            var bodyHeight = max - grHdr.Item.Top - 20;
            var headeHeight = (bodyHeight - 10) / 2;
            this.grHdr.Item.Height = headeHeight;

            this.grDt.Item.Left = this.lblBank.Item.Left;
            this.grDt.Item.Width = maxw - grDt.Item.Left - 40;
            this.grDt.Item.Top = this.grHdr.Item.Top + this.grHdr.Item.Height + 10;
            this.grDt.Item.Height = max - grDt.Item.Top - 20;
        }

        private SAPbouiCOM.Grid grDt;

        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            instance = null;
            IsFormOpen = false;
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);
            SetLocation();
            this.Freeze(false);
        }

        private void btnLoad_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            if (string.IsNullOrEmpty(TransDate))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_TransDateSelectNull, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }

            var batchno = $"SP{TransDate.Substring(2, 6)}0{BatchNo}";
            var query = "SELECT COUNT(*) AS \"Exists\" FROM \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Payoo_BatchHeader\" WHERE \"BatchNo\" = '" + batchno + "'";
            var dataHash = dbProvider.QuerySingle(query);
            if(dataHash != null)
            {
                var exist = dataHash["Exists"].ToString();
                if(exist != "0")
                {
                    UIHelper.LogMessage($"Batch {batchno} đã tồn tại", UIHelper.MsgType.Msgbox, true);
                    this.Freeze(false);
                    return;
                }
            }
            this.Clear();

            if (Bank == Banks.ViettinBank.GetDescription())
            {
                this.CallPayooAPI();
            }
            //else
            //{
            //    this.CallBIDVAPI();
            //}
            this.Freeze(false);
        }

        //private string GetToken()
        //{
        //    try
        //    {
        //        var query = string.Format(QueryString.GetAPICode);
        //        var data = DataProvider.QuerySingle(CoreSetting.DataConnection, query);
        //        if (data == null)
        //        {
        //            return string.Empty;
        //        }
        //        var code = data["Code"].ToString();
        //        if (string.IsNullOrEmpty(code))
        //        {
        //            return string.Empty;
        //        }

        //        var options = new RestClientOptions(ConfigurationManager.AppSettings["LinkBIDVAPI"])
        //        {
        //            MaxTimeout = -1,
        //        };
        //        var client = new RestClient(options);
        //        var request = new RestRequest(ConfigurationManager.AppSettings["AuthenBIDV"], Method.Post);

        //        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        //        request.AddParameter("grant_type", "authorization_code");
        //        request.AddParameter("client_id", ConfigurationManager.AppSettings["ClientIDBIDV"]);
        //        request.AddParameter("client_secret", ConfigurationManager.AppSettings["ClientSecretBIDV"]);
        //        request.AddParameter("code", code);
        //        request.AddParameter("redirect_uri", ConfigurationManager.AppSettings["redirect_uri"]);

        //        var response = client.Execute(request);
        //        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //        {
        //            var error = JsonSerializer.Deserialize<BIDVAccesstokenResponseErrro>(response.Content);
        //            if (error.error_description.Contains("code expired"))
        //            {
        //                UIHelper.LogMessage($"Code để lấy token đã bị hết hạn, vui lòng lấy lại code mới và thử lại", UIHelper.MsgType.StatusBar, true);
        //            }
        //            else
        //            {
        //                UIHelper.LogMessage($"Lỗi {error.error_description}", UIHelper.MsgType.StatusBar, true);
        //            }
        //        }
        //        else
        //        {
        //            var result = JsonSerializer.Deserialize<BIDVAccesstokenResponse>(response.Content);
        //            if (result != null)
        //                return result.access_token;
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //    return string.Empty;
        //}
        //private string Base64UrlEncode(byte[] input)
        //{
        //    return Convert.ToBase64String(input)
        //        .TrimEnd('=')
        //        .Replace('+', '-')
        //        .Replace('/', '_');
        //}
        ////private static RSAParameters ConvertToRSAParameters(RsaPrivateCrtKeyParameters rsaKey)
        //{
        //    RSAParameters result = default(RSAParameters);
        //    result.Modulus = rsaKey.Modulus.ToByteArrayUnsigned();
        //    result.Exponent = rsaKey.PublicExponent.ToByteArrayUnsigned();
        //    result.D = rsaKey.Exponent.ToByteArrayUnsigned();
        //    result.P = rsaKey.P.ToByteArrayUnsigned();
        //    result.Q = rsaKey.Q.ToByteArrayUnsigned();
        //    result.DP = rsaKey.DP.ToByteArrayUnsigned();
        //    result.DQ = rsaKey.DQ.ToByteArrayUnsigned();
        //    result.InverseQ = rsaKey.QInv.ToByteArrayUnsigned();
        //    return result;
        //}
        //private RSA LoadPrivateKey(string filePath)
        //{
        //    try
        //    {
        //        string text = File.ReadAllText(filePath);
        //        text = text.Replace("-----BEGIN PRIVATE KEY-----", "").Replace("-----END PRIVATE KEY-----", "").Replace("\n", "")
        //            .Replace("\r", "")
        //            .Trim();
        //        var obj = Convert.FromBase64String(text);
        //        var instance = PrivateKeyInfo.GetInstance(obj);
        //        var asymmetricKeyParameter = PrivateKeyFactory.CreateKey(instance);
        //        if (asymmetricKeyParameter is RsaPrivateCrtKeyParameters rsaKey)
        //        {
        //            RSA rSA = RSA.Create();
        //            rSA.ImportParameters(ConvertToRSAParameters(rsaKey));
        //            return rSA;
        //        }

        //        throw new ArgumentException("Key is not an RSA private key.");
        //    }
        //    catch (Exception innerException)
        //    {
        //        throw new Exception("Error loading private key", innerException);
        //    }
        //}
        //static byte[] SignData(RSA rsa, string data)
        //{
        //    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        //    return rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        //}
        //private string GenSignature()
        //{

        //    // Header (base64 encoded JSON)
        //    var header = new
        //    {
        //        alg = "RS256", // RSA with SHA-256
        //        typ = "JWT"
        //    };
        //    var headerJson = JsonSerializer.Serialize(header);
        //    var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        //    var detachedPayload = string.Empty;

        //    var signingInput = $"{headerBase64}.{detachedPayload}";
        //    var certPath = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";

        //    var rsa = LoadPrivateKey(certPath);
        //    var signature = SignData(rsa, signingInput);

        //    var signatureBase64 = Base64UrlEncode(signature);
        //    var jws = $"{headerBase64}.{detachedPayload}.{signatureBase64}";
        //    return jws;
        //    // Console.WriteLine("Detached JWS:");
        //    //Console.WriteLine(jws);

        //    // Example: Verification
        //    //bool isValid = VerifySignature(rsa, signingInput, signature);
        //    //Console.WriteLine("Signature valid: " + isValid);
        //}
     
        private byte[] HexToByteArray(string hex)
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

        private void CallPayooAPI()
        {

            try
            {
                UIHelper.LogMessage($"Bắt đầu gửi thông tin qua hệ thống Payoo để lấy thanh toán", UIHelper.MsgType.StatusBar, false);

                var options = new RestClientOptions(APIPayooConstant.APILink)
                {
                    MaxTimeout = -1,
                };

                var client = new RestClient(options);
                var request = new RestRequest(APIPayooConstant.SettlementTransactionsLink, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("APIUsername", APIPayooConstant.APIUsername);
                request.AddHeader("APIPassword", APIPayooConstant.APIPassword);
                request.AddHeader("APISignature", APIPayooConstant.APISignature);
                request.AddHeader("Accept", "application/json");
                var transDate = DateTime.ParseExact(TransDate, "yyyyMMdd", null).ToString("yyyyMMdd");

                var obj = new PayooGetSettlement
                {
                    BatchNumber = BatchNo,
                    PageNumber = Page,
                    SettlementDate = transDate
                };
                var objJson = new JavaScriptSerializer();
                var objBody = objJson.Serialize(obj);

                var oRequest = new PayoRequest();
                oRequest.RequestData = objBody;
                // sign data
                oRequest.SecureHash = APIUtil.EncryptSHA512(APIPayooConstant.ChecksumKey + oRequest.RequestData);
                var objJsonFinal = new JavaScriptSerializer();
                string temp = objJsonFinal.Serialize(oRequest);

                request.AddParameter("application/json", temp, ParameterType.RequestBody);

                //var requestData = new
                //{
                //    SettlementDate = transDate,
                //    BatchNumber = BatchNo,
                //    PageNumber = Page
                //};
                //var requestDataJson = JsonSerializer.Serialize(requestData);
                //var requestDataFull = new
                //{
                //    RequestData = requestDataJson,
                //    SecureHash = APIUtil.EncryptSHA512(APIPayooConstant.ChecksumKey + requestDataJson)//  "35b8fd5ca7e314b134126add44ad7be804462e17f0e28956df3bd0d4c673339e856bbf3b83635ada03ee5efd54941a62e8c370931cf3bdd8d9f555e75e5accc9"
                //};
                //var finalRequestJson = JsonSerializer.Serialize(requestDataFull);
                //finalRequestJson = @"{
                //                ""RequestData"": " + @"""" + requestDataJson + @"""
                //                ""SecureHash"": " + @"""" + APIUtil.EncryptSHA512(APIPayooConstant.ChecksumKey + requestDataJson) + @"""
                //            }
                //";
                //request.AddStringBody(finalRequestJson, DataFormat.Json);

                var response = client.Execute(request);

                var result = response.Content;

                var resp = JsonSerializer.Deserialize<PayooResponse>(result);
                if (resp == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var respDataStr = resp.ResponseData.Replace(@"\", "");
                var respDataObj = JsonSerializer.Deserialize<PayooResponseData>(respDataStr);
                if (respDataObj == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }

                PayooResponseCode rpcode = (PayooResponseCode)CoreExtensions.GetEnumValue<PayooResponseCode>(respDataObj.ResponseCode);
                if (rpcode != PayooResponseCode.Res0)
                {
                    UIHelper.LogMessage(rpcode.GetDescription(), UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var rpDataTransaction = JsonSerializer.Deserialize<PayooResponseDataExt>(respDataStr);

                // var inquiry = JsonSerializer.Deserialize<InquiryHeader>(result);

                if (rpDataTransaction != null)
                {
                    if (this.grHdr != null && this.grHdr.DataTable != null)
                    {
                        this.grHdr.DataTable.Rows.Clear();
                        this.grHdr.DataTable.Rows.Add();
                        var index = grHdr.DataTable.Rows.Count - 1;
                        this.grHdr.DataTable.SetValue("BatchNo", index, rpDataTransaction.BatchNo);
                        this.grHdr.DataTable.SetValue("TotalAmount", index, rpDataTransaction.TotalSettlementAmount);
                        this.grHdr.DataTable.SetValue("RowCount", index, rpDataTransaction.TotalSettlementRowCount);
                        this.grHdr.DataTable.SetValue("PageSize", index, rpDataTransaction.PageSize);

                        this.grHdr.Columns.Item("BatchNo").TitleObject.Caption = "Số Batch";
                        this.grHdr.Columns.Item("BatchNo").Editable = false;
                        this.grHdr.Columns.Item("TotalAmount").TitleObject.Caption = "Tổng tiền giao dịch thanh toán";
                        this.grHdr.Columns.Item("TotalAmount").Editable = false;
                        this.grHdr.Columns.Item("RowCount").TitleObject.Caption = "Tổng dòng giao dịch thanh toán";
                        this.grHdr.Columns.Item("RowCount").Editable = false;
                        this.grHdr.Columns.Item("PageSize").TitleObject.Caption = "Số lượng giao dịch của một trang";
                        this.grHdr.Columns.Item("PageSize").Editable = false;
                        this.grHdr.AutoResizeColumns();
                    }

                    if (this.grDt != null && this.grDt.DataTable != null)
                    {
                        this.grDt.DataTable.Rows.Clear();
                        var status = "N";
                        var refBank =string.Empty;
                        var query = string.Format(QueryString.GetStatusPayoo, rpDataTransaction.BatchNo);
                        var data = dbProvider.QuerySingle(query);
                        if(data != null)
                        {
                            status = data["BankRecStatus"].ToString();
                            refBank = data["BankRefNo"].ToString();
                        }
                        foreach (var item in rpDataTransaction.TransactionList)
                        {
                            this.grDt.DataTable.Rows.Add();
                            var index = grDt.DataTable.Rows.Count - 1;
                            this.grDt.DataTable.SetValue("OrderNo", index, item.OrderNo);
                            this.grDt.DataTable.SetValue("ShopId", index, item.ShopId);
                            this.grDt.DataTable.SetValue("SellerName", index, item.SellerName);
                            this.grDt.DataTable.SetValue("TransferAmount", index, item.MoneyAmount);
                            this.grDt.DataTable.SetValue("InvoiceDate", index, item.PurchaseDate);
                            this.grDt.DataTable.SetValue("Status", index, item.Status);
                            this.grDt.DataTable.SetValue("IntDate", index, DateTime.Now.ToString("yyyyMMdd"));
                            this.grDt.DataTable.SetValue("IntTime", index, DateTime.Now.ToString("HHmmss"));
                            this.grDt.DataTable.SetValue("BankRecStatus", index, status);
                            this.grDt.DataTable.SetValue("BankRefNo", index, refBank);
                            
                        }
                        this.grDt.Columns.Item("OrderNo").TitleObject.Caption = "Mã đơn hàng";
                        this.grDt.Columns.Item("OrderNo").Editable = false;

                        this.grDt.Columns.Item("ShopId").TitleObject.Caption = "Mã cửa hàng";
                        this.grDt.Columns.Item("ShopId").Editable = false;

                        this.grDt.Columns.Item("SellerName").TitleObject.Caption = "Tài khoản Đối tác";
                        this.grDt.Columns.Item("SellerName").Editable = false;

                        this.grDt.Columns.Item("TransferAmount").TitleObject.Caption = "Số tiền giao dịch";
                        this.grDt.Columns.Item("TransferAmount").Editable = false;

                        this.grDt.Columns.Item("InvoiceDate").TitleObject.Caption = "Ngày mua hàng";
                        this.grDt.Columns.Item("InvoiceDate").Editable = false;

                        this.grDt.Columns.Item("Status").TitleObject.Caption = "Trạng thái";
                        this.grDt.Columns.Item("Status").Editable = false;

                        this.grDt.Columns.Item("IntDate").TitleObject.Caption = "Ngày tích hợp";
                        this.grDt.Columns.Item("IntDate").Editable = false;

                        this.grDt.Columns.Item("IntTime").TitleObject.Caption = "Giờ tích hợp";
                        this.grDt.Columns.Item("IntTime").Editable = false;

                        this.grDt.Columns.Item("BankRefNo").Visible = false;
                        this.grDt.Columns.Item("PaymnetID").Visible = false;

                        this.grDt.Columns.Item("BankRecStatus").TitleObject.Caption = "Đã Clear";
                        this.grDt.Columns.Item("BankRecStatus").Editable = false;
                        this.grDt.Columns.Item("PaymentID").TitleObject.Caption = "Mã thanh toán";
                        this.grDt.Columns.Item("PaymentID").Editable = false;
                        this.grDt.Columns.Item("Message").TitleObject.Caption = "Thông điệp";
                        this.grDt.Columns.Item("Message").Editable = false;
                        this.grDt.AutoResizeColumns();
                    }

                    this.btnSave.Item.Enabled = true;
                }
                // }
            }
            catch (Exception ex)
            { }
        }

        private SAPbouiCOM.Button btnSave;

        private void btnSave_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            try
            {
                

                if (grHdr != null && grHdr.DataTable != null && grHdr.DataTable.Rows.Count > 0)
                {
                    var sqlCheckExist = string.Format(QueryString.CheckBatchExists, this.grHdr.GetValueCustom("BatchNo", 0));
                    var data = dbProvider.QuerySingle(sqlCheckExist);
                    if (data != null && data["Existed"].ToString() == "Existed")
                    {
                        UIHelper.LogMessage($"Batch này đã được lưu", UIHelper.MsgType.StatusBar, false);

                        this.Freeze(false);
                        return;
                    }

                    var insertHd = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Payoo_BatchHeader\" VALUES ( ";
                    insertHd += $"'{this.grHdr.GetValueCustom("BatchNo", 0)}',";
                    insertHd += $"{this.grHdr.GetValueCustom("TotalAmount", 0)},";
                    insertHd += $"{this.grHdr.GetValueCustom("RowCount", 0)},";
                    insertHd += $"{this.grHdr.GetValueCustom("PageSize", 0)}";
                    insertHd += ")";
                    var retHeader = dbProvider.ExecuteNonQuery(insertHd);

                    for (var index = 0; index < this.grDt.DataTable.Rows.Count; index++)
                    {
                        var insertdt = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Payoo_BatchDetail\" VALUES ( ";
                        insertdt += $"'{this.grHdr.GetValueCustom("BatchNo", 0)}',";
                        insertdt += $"'{this.grDt.GetValueCustom("OrderNo", index)}',";
                        insertdt += $"'{this.grDt.GetValueCustom("ShopId", index)}',";
                        insertdt += $"'{this.grDt.GetValueCustom("SellerName", index)}',";
                        insertdt += $"{this.grDt.GetValueCustom("TransferAmount", index)},";
                        insertdt += $"'{this.grDt.GetValueCustom("InvoiceDate", index)}',";
                        insertdt += $"'{this.grDt.GetValueCustom("Status", index)}',";
                        insertdt += $"'{this.grDt.GetValueCustom("IntDate", index)}',";
                        insertdt += $"'{this.grDt.GetValueCustom("IntTime", index)}',";
                        insertdt += $"'{this.grDt.GetValueCustom("BankRecStatus", index)}',";
                        insertdt += $"'{this.grDt.GetValueCustom("BankRefNo", index)}', ";
                        insertdt += $"'', '', ''";
                        insertdt += ")";
                        var retDetail = dbProvider.ExecuteNonQuery(insertdt);
                    }
                }
                UIHelper.LogMessage($"Lưu hoàn tất", UIHelper.MsgType.StatusBar, false);

            }
            catch (Exception ex)
            {

            }
            //this.Clear();
            this.Freeze(false);
        }
        private void Clear()
        {
            if (this.grHdr != null && this.grHdr.DataTable != null)
            {
                this.grHdr.DataTable.Rows.Clear();
            }
            if (this.grDt != null && this.grDt.DataTable != null)
            {
                this.grDt.DataTable.Rows.Clear();
            }
            this.btnSave.Item.Enabled = false;
        }

        private void cbbBank_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            //if(this.grHdr == null || this.grDt == null)
            //{
            //    return;
            //}
            //this.Freeze(true);


            //this.Freeze(false);
        }

        private SAPbouiCOM.Button btnReload;

        private void btnReload_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            if (string.IsNullOrEmpty(TransDate))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_TransDateSelectNull, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }

            var batchno = $"SP{TransDate.Substring(2, 6)}0{BatchNo}";
            var query = "SELECT COUNT(*) AS \"Exists\" FROM \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Payoo_BatchHeader\" WHERE \"BatchNo\" = '" + batchno + "'";
            var dataHash = dbProvider.QuerySingle(query);
            if (dataHash != null)
            {
                var exist = dataHash["Exists"].ToString();
                if (exist == "0")
                {
                    UIHelper.LogMessage($"Batch {batchno} chưa tồn tại", UIHelper.MsgType.Msgbox, true);
                    this.Freeze(false);
                    return;
                }
            }
            this.Clear();

            var queryHeader = "SELECT * FROM \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Payoo_BatchHeader\" WHERE \"BatchNo\" = '" + batchno + "'";

            if (this.grHdr != null && this.grHdr.DataTable != null)
            {

                this.grHdr.DataTable.Rows.Clear();
                this.grHdr.DataTable.ExecuteQuery(queryHeader);

                this.grHdr.Columns.Item("BatchNo").TitleObject.Caption = "Số Batch";
                this.grHdr.Columns.Item("BatchNo").Editable = false;
                this.grHdr.Columns.Item("TotalAmount").TitleObject.Caption = "Tổng tiền giao dịch thanh toán";
                this.grHdr.Columns.Item("TotalAmount").Editable = false;
                this.grHdr.Columns.Item("RowCount").TitleObject.Caption = "Tổng dòng giao dịch thanh toán";
                this.grHdr.Columns.Item("RowCount").Editable = false;
                this.grHdr.Columns.Item("PageSize").TitleObject.Caption = "Số lượng giao dịch của một trang";
                this.grHdr.Columns.Item("PageSize").Editable = false;

                this.grHdr.AutoResizeColumns();
            }

            var queryDetail = "SELECT * FROM \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Payoo_BatchDetail\" WHERE \"BatchNo\" = '" + batchno + "'";
            if (this.grDt != null && this.grDt.DataTable != null)
            {
                this.grDt.DataTable.Rows.Clear();
                this.grDt.DataTable.ExecuteQuery(queryDetail);

                this.grDt.Columns.Item("OrderNo").TitleObject.Caption = "Mã đơn hàng";
                this.grDt.Columns.Item("OrderNo").Editable = false;

                this.grDt.Columns.Item("ShopId").TitleObject.Caption = "Mã cửa hàng";
                this.grDt.Columns.Item("ShopId").Editable = false;

                this.grDt.Columns.Item("SellerName").TitleObject.Caption = "Tài khoản Đối tác";
                this.grDt.Columns.Item("SellerName").Editable = false;

                this.grDt.Columns.Item("TransferAmount").TitleObject.Caption = "Số tiền giao dịch";
                this.grDt.Columns.Item("TransferAmount").Editable = false;

                this.grDt.Columns.Item("InvoiceDate").TitleObject.Caption = "Ngày mua hàng";
                this.grDt.Columns.Item("InvoiceDate").Editable = false;

                this.grDt.Columns.Item("Status").TitleObject.Caption = "Trạng thái";
                this.grDt.Columns.Item("Status").Editable = false;

                this.grDt.Columns.Item("IntDate").TitleObject.Caption = "Ngày tích hợp";
                this.grDt.Columns.Item("IntDate").Editable = false;

                this.grDt.Columns.Item("IntTime").TitleObject.Caption = "Giờ tích hợp";
                this.grDt.Columns.Item("IntTime").Editable = false;

                this.grDt.Columns.Item("BankRefNo").Visible = false;
                this.grDt.Columns.Item("PaymnetID").Visible = false;

                this.grDt.Columns.Item("BankRecStatus").TitleObject.Caption = "Đã Clear";
                this.grDt.Columns.Item("BankRecStatus").Editable = false;
                this.grDt.Columns.Item("PaymentID").TitleObject.Caption = "Mã thanh toán";
                this.grDt.Columns.Item("PaymentID").Editable = false;
                this.grDt.Columns.Item("Message").TitleObject.Caption = "Thông điệp";
                this.grDt.Columns.Item("Message").Editable = false;

                SAPbouiCOM.EditTextColumn oCol1 = null;
                oCol1 = (SAPbouiCOM.EditTextColumn)this.grDt.Columns.Item("PaymentID");
                oCol1.LinkedObjectType = SAPObjectType.oIncomingPayments;

                this.grDt.AutoResizeColumns();

            }
            this.Freeze(false);
        }
    }
}
