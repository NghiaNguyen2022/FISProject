using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class TransferHeader
    {
        public string requestId { get; set; }
        public string providerId { get; set; }
        public object merchantId { get; set; }
        public VTResponseStatus status { get; set; }
        public string processedRecords { get; set; }
        public string totalAmount { get; set; }
        public List<TransferDetail> records { get; set; }
        public string signature { get; set; }
    }
}
