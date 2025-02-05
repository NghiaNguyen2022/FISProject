using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class TransferRequest : VTRequest
    {
        public string priority { get; set; }
        public string version { get; set; }
        public string softwareProviderId { get; set; }
        public string language { get; set; }
        public string appointedApprover { get; set; }
        public string feeAccount { get; set; }
        public string feeType { get; set; }
        public string scheduledDate { get; set; }
        public string approver { get; set; }
        public string transTime { get; set; }
        public List<TransferRecord> records { get; set; }
        public string clientIP { get; set; }
        public string channel { get; set; }
        public string signature { get; set; }
    }

    public class TransferRecord
    {
        public string transId { get; set; }
        public string approver { get; set; }
        public string transType { get; set; }
        public string amount { get; set; }
        public string recvAcctId { get; set; }
        public string recvBankId { get; set; }
        public string recvBranchId { get; set; }
        public string recvBankName { get; set; }
        public string recvAcctName { get; set; }
        public string recvAddr { get; set; }
        public string currencyCode { get; set; }
        public string remark { get; set; }
        public string senderBankId { get; set; }
        public string senderBranchId { get; set; }
        public string senderAddr { get; set; }
        public string senderAcctName { get; set; }
        public string senderAcctId { get; set; }
    }
}
