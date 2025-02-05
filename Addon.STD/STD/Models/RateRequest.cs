using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class RateRequest : VTRequest
    {
        public string trans_date { get; set; }
        public string language { get; set; }
        public string channel { get; set; }
        public string version { get; set; }
        public string clientIP { get; set; }
        public string signature { get; set; }
    }
}
