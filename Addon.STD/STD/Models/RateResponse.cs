using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class RateResponse : VTResponse
    {
        public List<ForeignExchangeRateInfo> ForeignExchangeRateInfo { get; set; }

    }
    public class ForeignExchangeRateInfo
    {
        public string Currency { get; set; }
        public string Mid_Rate { get; set; }
        public string Cash_Rate_Big { get; set; }
        public string Transfer_Rate { get; set; }
        public string Sell_Rate { get; set; }
    }
}
