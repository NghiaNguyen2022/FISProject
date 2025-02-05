using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class BIDVDomesticPaymentRequest
    {
        public string appAcct { get; set; }
        public string benAcct { get; set; }
        public string benAcctName { get; set; }
        public string benbankCode { get; set; }
        public string benbankName { get; set; }
        public string tranType { get; set; }
        public string amount { get; set; }
        public string curr { get; set; }
        public string charge { get; set; }
        public string remark { get; set; }
        public string @ref { get; set; }
        public string effdate { get; set; }
    }

    public class BIDVDomesticPaymentResponse
    {
        public DomesticPaymentResponse body { get; set; }
    }
    public class DomesticPaymentResponse
    {
        public string PaymentID { get; set; }
    }
}
