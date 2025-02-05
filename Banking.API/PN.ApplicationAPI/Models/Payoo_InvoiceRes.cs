using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PN.ApplicationAPI.Models
{
    public class Payoo_InvoiceRes
    {
        public string invoice_number { get; set; }
        public string vat_invoice_number { get; set; }
        public string vat_invoice_serial { get; set; }
        public string invoice_date { get; set; }
        public decimal total_amount { get; set; }
        public decimal paid_amount { get; set; }
        public decimal remaining_amount { get; set; }

    }
}