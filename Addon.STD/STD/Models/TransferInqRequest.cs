    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class TransferInqRequest
    {
        public string requestId { get; set; }
        public string transId { get; set; }
        public string providerId { get; set; }
        public string merchantId { get; set; }
        public string clientIP { get; set; }
        public string transTime { get; set; }
        public string channel { get; set; }
        public string version { get; set; }
        public string language { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string signature { get; set; }
    }
}
