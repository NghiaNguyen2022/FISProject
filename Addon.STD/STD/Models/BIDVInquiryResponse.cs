using SAPCore.SAP.DIAPI;
using STD.DataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class BIDVInquiryResponse
    {
        public InquiryResponse body { get; set; }
    }
    public class InquiryResponse
    {
        public int totalRecords { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
        public long startingBal { get; set; }
        public long endingBal { get; set; }
        public List<InquiryResponseTran> trans { get; set; }
    }
    public class InquiryResponseTran
    {
        public string seq { get; set; }
        public string tranDate { get; set; }
        public string remark { get; set; }
        public string debitAmount { get; set; }
        public string creditAmount { get; set; }
        public string @ref { get; set; }
        public string currCode { get; set; }

        internal void InsertData(string requestId)
        {
            try
            {
                var query = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_BIDVInquiryTran\" VALUES ( ";
                query += $"'{requestId}',";
                query += $"'{seq}',";
                query += $"'{tranDate}',";
                query += $"'{remark}',";
                query += $"'{debitAmount ?? "0"}',";
                query += $"'{creditAmount ?? "0"}',";
                query += $"'{@ref ?? "0"}',";
                query += $"'{currCode}'";
                query += ")";

                var ret1 = dbProvider.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class BIDVResponse401Error
    {
        public string httpCode { get; set; }
        public string httpMessage { get; set; }
        public string moreInformation { get; set; }
    }
    public class BIDVResponse400Error
    {
        public ErrorResponse errorResponse { get; set; }
    }
    public class ErrorResponse
    {
        public Metadata metadata { get; set; }
        public AdditionalInfo additionalInfo { get; set; }
    }
    public class Metadata
    {
        public Status status { get; set; }
    }
    public class AdditionalInfo
    {
        public DetailedError detailedError { get; set; }
    }
    public class Status
    {
        public string code { get; set; }
        public string desc { get; set; }
    }
    public class DetailedError
    {
        public string errorCode { get; set; }
        public string description { get; set; }
    }
    public class BIDVResponse500Error
    {
        public ErrorResponse errorResponse { get; set; }
    }
}
