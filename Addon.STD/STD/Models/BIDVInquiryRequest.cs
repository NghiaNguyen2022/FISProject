using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    //public class BodyReq
    //{
    //    public Body body { get; set; }
    //}

    public class BIDVInquiryRequest
    {
        public string actNumber { get; set; }
        public string curr { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string page { get; set; }
    }

    public class Req_000_JWEBIDVInquiryRequest
    {
        public Recipient[] recipients { get; set; }

        [JsonProperty(PropertyName = "protected")]
        public string protectedField { get; set; }
        public string ciphertext { get; set; }
        public string iv { get; set; }
        public string tag { get; set; }
    }
    public class Header
    {
    }

    public class Recipient
    {
        public Header header { get; set; }
        public string encrypted_key { get; set; }
    }
}
