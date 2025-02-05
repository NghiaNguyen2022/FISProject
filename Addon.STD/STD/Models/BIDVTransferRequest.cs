namespace STDApp.Models
{
    public class BIDVTransferResponse
    {
        public TransferResponse body { get; set; }
    }
    public class TransferResponse
    {
        public string PaymentID { get; set; }
    }

    public class BIDVTransferRequest
    {
        public string appAcct { get; set; } // Debit Account :Sender
        public string benAcct { get; set; } // Credit Account
        public string benAcctName { get; set; } // Credit Account Name
        public string benbankCode { get; set; } // Credit Bnak code
        public string benbankName { get; set; } // Credit Bnak Name 
        public string tranType { get; set; } //1/2/3 :internal/external/247
        public decimal amount { get; set; }
        public string curr { get; set; }
        public string charge { get; set; } //Fee I/E
                                           
        public string remark { get; set; }
        public string @ref { get; set; }
        public string effdate { get; set; }
    } 
}
