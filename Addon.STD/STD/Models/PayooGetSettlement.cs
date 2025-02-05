using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class PayoRequest
    {
        public string RequestData;
        public string SecureHash;
    }

    public class PayooGetSettlement
    {
        public string SettlementDate;
        public int BatchNumber;
        public int PageNumber;
    }
}
