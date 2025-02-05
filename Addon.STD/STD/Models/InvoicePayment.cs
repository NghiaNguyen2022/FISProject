using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class InvoicePayment
    {
        public string CardCode { get; set; }
        public string DocCur { get; set; }
        public string DocEntry { get; set; }
        public string TransferAmount { get; set; }
        public string OrderNo { get; set; }
    }
}
