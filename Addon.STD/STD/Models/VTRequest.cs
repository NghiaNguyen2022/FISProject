namespace STDApp.Models
{
    public class VTRequest
    {
        public string requestId { get; set; }
        public object merchantId { get; set; }
        public string providerId { get; set; }
        public string model { get; set; }
    }
    public class VTResponseStatus
    {
        public string code { get; set; }
        public string message { get; set; }
    }
    public class VTResponse
    {
        public string requestId { get; set; }
        public string providerId { get; set; }
        public string merchantId { get; set; }
        public VTResponseStatus status { get; set; }
    }
}
