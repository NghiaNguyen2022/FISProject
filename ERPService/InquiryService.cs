using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text.Json;
using System.Timers;
using System.Web.Script.Serialization;
using ERPService.DataReader;
using ERPService.Models;
using PN.SmartLib.Helper;
using RestSharp;
using SAPbobsCOM;
using SAPCore;
using SAPCore.Config;
using SAPCore.SAP.DIAPI;
using static System.Net.WebRequestMethods;

namespace ERPService

{
    public partial class InquiryService : ServiceBase
    {
        Timer timer = new Timer();
		TimeSpan? timeRun1 = null;
		TimeSpan? timeRun2 = null;

        public InquiryService()
        {
            InitializeComponent();
		}

        protected override void OnStop()
        {
            
        }

		protected override void OnStart(string[] args)
		{
			try
			{
				WriteToFile($"On Start");
				string timeRun1String = ConfigurationManager.AppSettings["timeRun1"];
				string timeRun2String = ConfigurationManager.AppSettings["timeRun2"];
				timeRun1 = TimeSpan.Parse(timeRun1String);
				timeRun2 = TimeSpan.Parse(timeRun2String);
				WriteToFile($"Success parse time from config: {timeRun1} - {timeRun2}");
			}
			catch (FormatException ex)
			{
				WriteToFile($"Error parsing time: {ex.Message}");
				return;
			}
			bool isDebugMode = Boolean.Parse(ConfigurationManager.AppSettings["isDebugMode"]);
			WriteToFile($"Is debug mode: {isDebugMode}");
			run(isDebugMode);
		}

		private void run(bool debugMode = false)
		{
			if (debugMode)
			{
				DateTime fromDate = new DateTime(2024, 9, 1);
				DateTime toDate = new DateTime(2024, 9, 30);
				this.Process(fromDate, toDate);
			}
			else
			{
				timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
				timer.Interval = 60000;
				timer.Enabled = true;
				timer.Start();
			}
		}

		private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
			try
			{
				DateTime now = DateTime.Now;
				TimeSpan currentTime = now.TimeOfDay;
				DateTime fromTime, toTime;
				// Match the current time to the configured times
				if (currentTime.Hours == timeRun1.Value.Hours && currentTime.Minutes == timeRun1.Value.Minutes)
				{
					// timeRun1 match: from = timeRun2 of yesterday, to = timeRun1 of today
					fromTime = DateTime.Today.AddDays(-1).Add(timeRun2.Value); // Yesterday's timeRun2
					toTime = DateTime.Today.Add(timeRun1.Value);              // Today's timeRun1
					Console.WriteLine($"Matched timeRun1: From {fromTime} to {toTime}");
				}
				else if (currentTime.Hours == timeRun2.Value.Hours && currentTime.Minutes == timeRun2.Value.Minutes)
				{
					// timeRun2 match: from = timeRun1 of today, to = timeRun2 of today
					fromTime = DateTime.Today.Add(timeRun1.Value);            // Today's timeRun1
					toTime = DateTime.Today.Add(timeRun2.Value);              // Today's timeRun2
					Console.WriteLine($"Matched timeRun2: From {fromTime} to {toTime}");
				}
			}
			catch (Exception ex)
			{
				WriteToFile($"Error catching runTime and generate fromDate toDate: {ex.Message}");
				return;
			}
		}

        private void DisConnect()
        {
           //DIServiceConnection.Instance.DIDisconnect();
        }
        private void Process(DateTime fromDate, DateTime toDate)
        {
			WriteToFile($"[Process] START from - to: {fromDate} - {toDate}");
			InquiryHeader inquiryHeader = CallAPIViettin(fromDate, toDate);
			WriteToFile($"[Process] CallAPIViettin success");
			if (inquiryHeader == null)
			{
				return;
			};
			HashSet<string> transactionNumbers = inquiryHeader.transactions.Select(transaction => transaction.transactionNumber).ToHashSet();
			HashSet<string> existed = this.checkExistedInDb(transactionNumbers);
			//HashSet<string> notExistedInDb = this.getNotExistedInDb(new List<string> { "1", "2", "3", "6", "10"});
			WriteToFile($"[Process] existed in db: {JsonSerializer.Serialize(existed)}");
		    if(existed.Count == inquiryHeader.transactions.Count)
			{
				WriteToFile($"[Process] All transaction number is existed!");
				return;
			}
			inquiryHeader.transactions = inquiryHeader.transactions.Where(transaction => !existed.Contains(transaction.transactionNumber)).ToList();
			inquiryHeader.InsertData();
			WriteToFile($"[Process] Fetch data from Viettin and insert success!");
			WriteToFile($"[Process] END");
		}

		private InquiryHeader CallAPIViettin(DateTime fromDate, DateTime toDate)
		{
			WriteToFile($"[CallAPIViettin] fromDate - to Date: {fromDate} - {toDate}");
			try
			{
				var options = new RestClientOptions()
				{
					MaxTimeout = -1,
				};

				var client = new RestClient(options);
				var request = new RestRequest(ConfigurationManager.AppSettings["UrlApiInquiry"], Method.Post);
				request.AddHeader("x-ibm-client-id", ConfigurationManager.AppSettings["ClientID"]);
				request.AddHeader("x-ibm-client-secret", ConfigurationManager.AppSettings["ClientSecret"]);
				request.AddHeader("Content-Type", "application/json");

				var data = new InquiryRequest()
				{
					requestId = DateTime.Now.ToString("yyyyMMddHHmmss"),
					merchantId = ConfigurationManager.AppSettings["MerchantId"],
					providerId = ConfigurationManager.AppSettings["ProviderId"],
					model = "2",
					account = "112000002609",
					fromDate = fromDate.ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime
					toDate = toDate.ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime
					accountType = "D",
					collectionType = "c,d",
					agencyType = "a",
					transTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
					channel = "ERP",
					version = "1",
					clientIP = "",
					language = "vi",
					signature = "", // Giá trị rỗng
					fromTime = "00:00:00",
					toTime = DateTime.Now.ToString("HH:mm:ss")
				};
				data.signature = (
					  data.requestId +
					  data.providerId +
					  data.merchantId +
					  data.account);
				var path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";
				data.signature = FPT.SHA256_RSA2048.Encrypt(data.signature, path);
				WriteToFile($"[CallAPIViettin] SIGNATURE: {JsonSerializer.Serialize(data.signature)}");
				var json = JsonSerializer.Serialize(data);
				request.AddParameter("application/json", json, ParameterType.RequestBody);
				var response = client.Execute(request);
				WriteToFile($"[CallAPIViettin] call api success!");
				WriteToFile($"[CallAPIViettin] response: {JsonSerializer.Serialize(response)}");
				if ((response.StatusCode != System.Net.HttpStatusCode.OK) || response?.Content == null)
				{
					return null;
				}
				var result = response.Content;
				var rps = JsonSerializer.Deserialize<InquiryHeader>(result);
				WriteToFile($"[CallAPIViettin] response: {JsonSerializer.Serialize(rps)}");
				if (rps == null || rps.transactions == null || rps.transactions.Count <= 0)
				{
					return null;
				}
				return rps;
			}catch(Exception e)
			{
				WriteToFile($"[CallAPIViettin] ERROR: {e.Message}");
				return null;
			}
		}

		private HashSet<string> checkExistedInDb(ICollection<string> checkList)
        {
			var list = new HashSet<string>();
			try
			{

				string query = "WITH \"Transactions\" AS ( "
					+ $@"SELECT * FROM (VALUES {string.Join(",", checkList.Select(item => $"('{item}')"))}) AS T( "
					+ "\"transNumber\")"
					+ ")"
					+ "SELECT \"transNumber\""
					+ "FROM \"Transactions\""
					+ "WHERE \"transNumber\" IN (SELECT \"transNumber\" FROM \"tb_Bank_InquiryDetail\");";
				dbProvider dbProvider = new dbProvider();
				Hashtable[] results = dbProvider.QueryList(query);
				foreach (Hashtable row in results)
				{
					if (row.ContainsKey("transNumber"))
					{
						list.Add(row["transNumber"].ToString());
					}
				}
				return list;
			}
			catch (Exception e) {
				WriteToFile($"[Process] getNotExistedInDb ERROR: {JsonSerializer.Serialize(e.Message)}");
				return list;
			}
        }
      
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!System.IO.File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = System.IO.File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = System.IO.File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
