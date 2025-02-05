namespace STD.Models
{
    public class PaymentKeyByBranch
    {
        public string BranchID { get; set; }
        public int Number { get; set; }
    }
    public class Branch
    {
        public string BPLId { get; set; }
        public string BPLName { get; set; }
        public string AliasName { get; set; }
    }
}
