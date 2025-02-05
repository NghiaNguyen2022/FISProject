using System.Collections.Generic;
using System.ComponentModel;

namespace STDApp.Models
{
    public class PayooResponse
    {
        public string ResponseData { get; set; }
        public string SecureHash { get; set; }
    }

    public class PayooResponseData
    {
        public int ResponseCode { get; set; }
    }

    public class PayooResponseDataExt: PayooResponseData
    {
        public string BatchNo { get; set; }
        public double TotalSettlementAmount { get; set; }
        public int TotalSettlementRowCount { get; set; }
        public int PageSize { get; set; }
        public List<PayooTransactions> TransactionList { get; set; }

    }
    public class PayooTransactions
    {
        public string PurchaseDate { get; set; }    
        public string OrderNo { get; set; }
        public int ShopId { get; set; }
        public string SellerName { get; set; }
        public int Status { get; set; }
        public double MoneyAmount { get; set; }
    }

    public enum PayooResponseCode
    {
        [Description("OK")]
        Res0 = 0,

        [Description("Không có data")]
        Res8 = 8,

        [Description("support./ Lỗi hệ thống.Vui lòng liên hệ kỹ thuật Payoo để được hỗ trợ.")]
        Res1000 = 1000,

        [Description("/ Xác thực doanh nghiệp thất bại do API Credentials không hợp lệ.")]
        Res1001 = 1001,

        [Description("Giá trị checksum không hợp lệ.")]
        Res1002 = 1002,

        [Description("Các tham số yêu cầu không hợp lệ.")]
        Res1003 = 1003
    }
}
