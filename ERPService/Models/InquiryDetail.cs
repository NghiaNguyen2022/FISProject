using System;
using ERPService.DataReader;
using SAPCore.SAP.DIAPI;

namespace ERPService.Models
{
    public class InquiryDetail
    {
        public string order { get; set; }
        public string transactionDate { get; set; }
        public string transactionContent { get; set; }
        public string debit { get; set; }
        public string credit { get; set; }
        public string accountBal { get; set; }
        public string transactionNumber { get; set; }
        public string corresponsiveAccount { get; set; }
        public string corresponsiveAccountName { get; set; }
        public string agency { get; set; }
        public string virtualAccount { get; set; }
        public string corresponsiveBankName { get; set; }
        public string corresponsiveBankId { get; set; }
        public string serviceBranchId { get; set; }
        public object serviceBankName { get; set; }
        public string channel { get; set; }

        public bool InsertData(string requestId, string providerId, string merchantId)
        {
            try
            {
                var query = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_InquiryDetail\" VALUES ( ";
                query += $"'{requestId ?? ""}',";
                query += $"'{providerId ?? ""}',";
                query += $"'{merchantId ?? ""}',";
                query += $"'{transactionDate ?? ""}',";
                query += $"'{transactionContent ?? ""}',";
                query += $"{debit ?? "0"},";
                query += $"{credit ?? "0"},";
                query += $"{accountBal ?? "0"},";
                query += $"'{transactionNumber ?? "0"}',";
                query += $"'{corresponsiveAccount ?? ""}',";
                query += $"'{corresponsiveAccountName ?? ""}',";
                query += $"'{agency ?? ""}',";
                query += $"'{virtualAccount ?? ""}',";
                query += $"'{corresponsiveBankName ?? ""}',";
                query += $"'{corresponsiveBankId ?? ""}',";
                query += $"'{channel ?? ""}'";
                query += ")";

                var ret1 = dbProvider.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
            }
            return true;
        }
    }
}
