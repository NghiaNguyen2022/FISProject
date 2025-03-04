using APIHandler;
using ERPService.Common;
using ERPService.DataReader;
using ERPService.Models;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text.Json;
using System.Timers;

namespace ERPService.BackJob
{
    public partial class Test : ServiceBase
    {
        Timer timer = new Timer();

        public Test()
        {
            InitializeComponent();
		}

        protected override void OnStop()
        {
            Utils.WriteToFile($"On Stop Test");
        }

		protected override void OnStart(string[] args)
		{
            try
            {
                Utils.WriteToFile($"On Start Test");
                run();
            }
            catch (FormatException ex)
            {
                Utils.WriteToFile($"Error: {ex.Message}");
                return;
            }
        }

		private void run()
		{
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Start();
        }

        public void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                var docentry = 1893;
                Utils.WriteToFile($"Start Cancle {docentry}");

                //if(DISe)
                var message = string.Empty;
                APIJobHandler.Test(docentry, ref message);
                Utils.WriteToFile($"{message}");

            }
            catch (Exception ex)
            {
                Utils.WriteToFile($"Error catching runTime and generate fromDate toDate: {ex.Message}");
                return;
            }
        }
        
  //      private void Process(DateTime fromDate, DateTime toDate)
  //      {
		//	Utils.WriteToFile($"[Process] START from - to: {fromDate} - {toDate}");
		//	InquiryHeader inquiryHeader = CallAPIViettin(fromDate, toDate);
		//	Utils.WriteToFile($"[Process] CallAPIViettin success");
		//	if (inquiryHeader == null)
		//	{
		//		return;
		//	};
		//	HashSet<string> transactionNumbers = inquiryHeader.transactions.Select(transaction => transaction.transactionNumber).ToHashSet();
		//	HashSet<string> existed = this.checkExistedInDb(transactionNumbers);
		//	//HashSet<string> notExistedInDb = this.getNotExistedInDb(new List<string> { "1", "2", "3", "6", "10"});
		//	Utils.WriteToFile($"[Process] existed in db: {JsonSerializer.Serialize(existed)}");
		//    if(existed.Count == inquiryHeader.transactions.Count)
		//	{
  //              Utils.WriteToFile($"[Process] All transaction number is existed!");
		//		return;
		//	}
		//	inquiryHeader.transactions = inquiryHeader.transactions.Where(transaction => !existed.Contains(transaction.transactionNumber)).ToList();
		//	inquiryHeader.InsertData();
		//	Utils.WriteToFile($"[Process] Fetch data from Viettin and insert success!");
		//	Utils.WriteToFile($"[Process] END");
		//}

		//private InquiryHeader CallAPIViettin(DateTime fromDate, DateTime toDate)
		//{
  //          Utils.WriteToFile($"[CallAPIViettin] fromDate - to Date: {fromDate} - {toDate}");
		//	try
		//	{
		//		var options = new RestClientOptions()
		//		{
		//			MaxTimeout = -1,
		//		};

		//		var client = new RestClient(options);
		//		var request = new RestRequest(ConfigurationManager.AppSettings["UrlApiInquiry"], Method.Post);
		//		request.AddHeader("x-ibm-client-id", ConfigurationManager.AppSettings["ClientID"]);
		//		request.AddHeader("x-ibm-client-secret", ConfigurationManager.AppSettings["ClientSecret"]);
		//		request.AddHeader("Content-Type", "application/json");

		//		var data = new InquiryRequest()
		//		{
		//			requestId = DateTime.Now.ToString("yyyyMMddHHmmss"),
		//			merchantId = ConfigurationManager.AppSettings["MerchantId"],
		//			providerId = ConfigurationManager.AppSettings["ProviderId"],
		//			model = "2",
		//			account = "112000002609",
		//			fromDate = fromDate.ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime
		//			toDate = toDate.ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime
		//			accountType = "D",
		//			collectionType = "c,d",
		//			agencyType = "a",
		//			transTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
		//			channel = "ERP",
		//			version = "1",
		//			clientIP = "",
		//			language = "vi",
		//			signature = "", // Giá trị rỗng
		//			fromTime = "00:00:00",
		//			toTime = DateTime.Now.ToString("HH:mm:ss")
		//		};
		//		data.signature = (
		//			  data.requestId +
		//			  data.providerId +
		//			  data.merchantId +
		//			  data.account);
		//		var path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";
		//		data.signature = FPT.SHA256_RSA2048.Encrypt(data.signature, path);
  //              Utils.WriteToFile($"[CallAPIViettin] SIGNATURE: {JsonSerializer.Serialize(data.signature)}");
		//		var json = JsonSerializer.Serialize(data);
		//		request.AddParameter("application/json", json, ParameterType.RequestBody);
		//		var response = client.Execute(request);
  //              Utils.WriteToFile($"[CallAPIViettin] call api success!");
  //              Utils.WriteToFile($"[CallAPIViettin] response: {JsonSerializer.Serialize(response)}");
		//		if ((response.StatusCode != System.Net.HttpStatusCode.OK) || response?.Content == null)
		//		{
		//			return null;
		//		}
		//		var result = response.Content;
		//		var rps = JsonSerializer.Deserialize<InquiryHeader>(result);
  //              Utils.WriteToFile($"[CallAPIViettin] response: {JsonSerializer.Serialize(rps)}");
		//		if (rps == null || rps.transactions == null || rps.transactions.Count <= 0)
		//		{
		//			return null;
		//		}
		//		return rps;
		//	}
		//	catch(Exception e)
		//	{
  //              Utils.WriteToFile($"[CallAPIViettin] ERROR: {e.Message}");
		//		return null;
		//	}
		//}

        private List<string> VTBAccounts()
        {
            var result = new List<string>();
            var query = "SELECT * FROM  \"" + Constant.Schema + "\".\"vw_Bank_BankAccount\" WHERE \"Key\" = 'VT'";
            var datas = dbProvider.QueryList(query);
            if(datas != null && datas.Length > 0)
            {
                foreach(var data in datas)
                {
                    result.Add(data["Account"].ToString());
                }
            }
            return result;
        }
		//private HashSet<string> checkExistedInDb(ICollection<string> checkList)
  //      {
		//	var list = new HashSet<string>();
		//	try
		//	{

		//		string query = "WITH \"Transactions\" AS ( "
		//			+ $@"SELECT * FROM (VALUES {string.Join(",", checkList.Select(item => $"('{item}')"))}) AS T( "
		//			+ "\"transNumber\")"
		//			+ ")"
		//			+ "SELECT \"transNumber\""
		//			+ "FROM \"Transactions\""
		//			+ "WHERE \"transNumber\" IN (SELECT \"transNumber\" FROM \"tb_Bank_InquiryDetail\");";
		//		dbProvider dbProvider = new dbProvider();
		//		Hashtable[] results = dbProvider.QueryList(query);
		//		foreach (Hashtable row in results)
		//		{
		//			if (row.ContainsKey("transNumber"))
		//			{
		//				list.Add(row["transNumber"].ToString());
		//			}
		//		}
		//		return list;
		//	}
		//	catch (Exception e) {
  //              Utils.WriteToFile($"[Process] getNotExistedInDb ERROR: {JsonSerializer.Serialize(e.Message)}");
		//		return list;
		//	}
  //      }
    }
}
