using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using ERPService.DataReader;
using SAPCore.SAP.DIAPI;

namespace ERPService.Models
{
    public class InquiryResponse
    {
        public string requestId { get; set; }
        public string providerId { get; set; }
        public string merchantId { get; set; }
        public Status status { get; set; }
    }

    public class Status
    {
        public string code { get; set; }
        public string message { get; set; }
    }
    internal class InquiryHeader : InquiryResponse
    {       
        public string account { get; set; }
        public string companyName { get; set; }
        public string accountType { get; set; }
        public string curency { get; set; }
        public string accountBal { get; set; }
        public string availableBal { get; set; }
        public string openningBal { get; set; }
        public string closingBal { get; set; }
        public string fromDate { get; set; }
        public string totalCredit { get; set; }
        public string numberCreditTransaction { get; set; }
        public string totalDebit { get; set; }
        public string toDate { get; set; }
        public string numberDebitTransaction { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public List<InquiryDetail> transactions { get; set; }
        public string signature { get; set; }
        public bool InsertData(ref string query)
        {
            try
            {
                query = "INSERT INTO \""+ DIConnection.Instance.CompanyDB+ "\".\"tb_Bank_InquiryHeader\" VALUES ( ";
                query += $"'{requestId ?? ""}',";
                query += $"'{providerId??""}',";
                query += $"'{merchantId ?? ""}',";
                query += $"'{account ?? ""}',";
                query += $"'{companyName ?? ""}',";
                query += $"'{accountType ?? ""}',";
                query += $"'{curency ?? ""}',";
                query += $"{accountBal ?? "0"},";
                query += $"{availableBal ?? "0"},";
                query += $"{openningBal ?? "0"},";
                query += $"{closingBal ?? "0"},";
                query += $"'{fromDate ?? ""}',";
                query += $"{totalCredit ?? "0"},";
                query += $"{numberCreditTransaction ?? "0"},";
                query += $"{totalDebit ?? "0"},";
                query += $"{numberDebitTransaction ?? "0"},";
                query += $"'{toDate ?? ""}',";
                query += $"'{fromTime ?? ""}',";
                query += $"'{toTime ?? ""}'";
                query += ")";

                var ret1 = dbProvider.ExecuteNonQuery(query);

                //foreach(var item in transactions)
                //{
                //    item.InsertData(requestId, providerId, merchantId);
                //}
            }
            catch (Exception ex)
            {
                
            }
            return true;
        }
		public bool InsertData()
		{
			try
			{
				var query = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"InquiryHeader\" VALUES ( ";
				query += $"'{requestId ?? ""}',";
				query += $"'{providerId ?? ""}',";
				query += $"'{merchantId ?? ""}',";
				query += $"'{account ?? ""}',";
				query += $"'{companyName ?? ""}',";
				query += $"'{accountType ?? ""}',";
				query += $"'{curency ?? ""}',";
				query += $"{accountBal ?? "0"},";
				query += $"{availableBal ?? "0"},";
				query += $"{openningBal ?? "0"},";
				query += $"{closingBal ?? "0"},";
				query += $"'{fromDate ?? ""}',";
				query += $"{totalCredit ?? "0"},";
				query += $"{numberCreditTransaction ?? "0"},";
				query += $"{totalDebit ?? "0"},";
				query += $"{numberDebitTransaction ?? "0"},";
				query += $"'{toDate ?? ""}',";
				query += $"'{fromTime ?? ""}',";
				query += $"'{toTime ?? ""}'";
				query += ")";

				var ret1 = dbProvider.ExecuteNonQuery(query);

                foreach(var item in transactions)
                {
                    item.InsertData(requestId, providerId, merchantId);
                }
            }
			catch (Exception ex)
			{

			}
			return true;
		}
	}
}
