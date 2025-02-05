using System;
using System.Collections.Generic;
using SAPCore.SAP.DIAPI;
using STD.DataReader;

namespace STDApp.Models
{

    internal class InquiryHeader : VTResponse
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
        public bool InsertData()
        {
            //using(var trans)
            try
            {
                var query = "INSERT INTO CCC.\"InquiryHeader\" VALUES ( ";
                query += $"'{requestId}',";
                query += $"'{providerId}',";
                query += $"'{merchantId}',";
                query += $"'{account}',";
                query += $"'{companyName}',";
                query += $"'{accountType}',";
                query += $"'{curency}',";
                query += $"{accountBal},";
                query += $"{availableBal},";
                query += $"{openningBal},";
                query += $"{closingBal},";
                query += $"'{fromDate}',";
                query += $"{totalCredit},";
                query += $"{numberCreditTransaction},";
                query += $"{totalDebit},";
                query += $"{numberDebitTransaction},";
                query += $"'{toDate}',";
                query += $"'{fromTime}',";
                query += $"'{toTime}'";
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
