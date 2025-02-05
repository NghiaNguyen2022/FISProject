namespace STDApp.Models
{
    public class InquiryRequest : VTRequest
    {
        //public string requestId { get; set; }
        //public object merchantId { get; set; }
        //public string providerId { get; set; }
        //public string model { get; set; }
        public string account { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string accountType { get; set; }
        public string collectionType { get; set; }
        public string agencyType { get; set; }
        public string transTime { get; set; }
        public string channel { get; set; }
        public string version { get; set; }
        public string clientIP { get; set; }
        public string language { get; set; }
        public string signature { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
    }
}
