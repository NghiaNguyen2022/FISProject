using System.Collections.Generic;

namespace STDApp.Models
{
    public class TransferInq
    {
        public string requestId { get; set; }
        public string providerId { get; set; }
        public object merchantId { get; set; }
        public VTResponseStatus status { get; set; }
        public object totalTransaction { get; set; }
        public object totalAmount { get; set; }
        public object totalFee { get; set; }
        public List<TransferInqDetail> result { get; set; }
        public string signature { get; set; }
    }
    public  class TransferInqDetail
    {
        public string transId { get; set; }
        public string fromAccount { get; set; }
        public string fromBankId { get; set; }
        public string toAccount { get; set; }
        public string toBankId { get; set; }
        public string toBankname { get; set; }
        public string toBankBranchId { get; set; }
        public string receiverName { get; set; }
        public string remark { get; set; }
        public string transType { get; set; }
        public string amt { get; set; }
        public string feeAmt { get; set; }
        public string feeVat { get; set; }
        public string feeType { get; set; }
        public string receivedDate { get; set; }
        public string maker { get; set; }
        public string checker { get; set; }
        public object verifiedDate { get; set; }
        public string erpMtId { get; set; }
        public object hostMtId { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }
}
