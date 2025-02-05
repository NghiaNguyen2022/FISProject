using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class TransferDetail
    {
        public string transId { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public object feeAmount { get; set; }
        public object vatAmount { get; set; }
        public object bankTransactionId { get; set; }
        public string description { get; set; }
        public string currencyCode { get; set; }
        public string fromAccountNo { get; set; }
        public string toAccountNo { get; set; }
        public string amount { get; set; }
        public string transType { get; set; }
        public string receiveName { get; set; }
        public string receiveBankCode { get; set; }
        public string receiveBank { get; set; }
    }
}
