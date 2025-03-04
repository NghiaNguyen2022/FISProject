using System;

namespace ERPService
{
    internal class GlobalConfig
    {
        public string ApiVTBank = "https://api-uat.vietinbank.vn/vtb-api-uat/development/erp/v1/statement/";
        public string ApiVTBankBase = "https://api-uat.vietinbank.vn";
        public string BaseAPIVTUrl = "https://api-uat.vietinbank.vn/vtb-api-uat/development/erp/v1/";
        public string UrlApiInquiryVT = "https://api-uat.vietinbank.vn/vtb-api-uat/development/erp/v1/statement/inquiry";
        public string VTClientID = "fbbf1989a3ad0de68446317f5f104df0";
        public string VTClientSecret = "2cd7b943f4d7c5115d44b81487497ae3";
        public string VTAccount = "108004261279";
        public string VTAccount_1 = "112000002609";
        public string VTCIF = "400156156";
        public string VTUSERTD = "pymepharco_taodien";
        public string VTUSERPD = "pymepharco_pheduyet";
        public string VTProviderId = "PYMEPHARCO";
        public string VTMerchantId = "";
        public string VTAccessToken = "";

        public string ApiPOOBase = "https://biz-sb.payoo.vn";
        public string POOSettlementTransactionsUrl = "/BusinessRestAPI.svc/GetSettlementTransactions";
        public string POOAPIUsername = "SB_Stada_BizAPI";
        public string POOAPIPassword = "9qW0k9/5khmSqdOE";
        public string POOAPISignature = "U5E0ykWrsQSqkjS6xR+Qnj3cbuNUi8YgSC2r6/BcXrpdcjOx6XYh7XNjQf806Yay";

        public static RunnerTime InquiryRunner;
        public static RunnerTime PayooRunner;
        public static RunnerTime ClearRunner;

    }

    public class RunnerTime
    {
        public DateTime RunDate { get; set; }
        public int Timer { get; set; }
        public RunnerTime()
        {
            RunDate = new DateTime();
            Timer = 0;
        }
    }
}
