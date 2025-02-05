namespace STDApp.Models
{
    public class PaymentActionResult
    {
        public string CardCode { get; set; }
        public int Count { get; set; }
        public bool Flag { get; set; }
        //public int Created { get; set; }
        public string Message { get; set; }
    }

    public class PaymentApproveResult
    {
        public string DocNum { get; set; }
        public bool Flag { get; set; }
        public string Message { get; set; }
    }
}
