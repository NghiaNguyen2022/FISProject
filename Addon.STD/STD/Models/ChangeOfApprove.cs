namespace STDApp.Models
{
    public class ChangeOfApprove
    {
        public int Index { get; set; }
        public int DocEntry { get; set; }
        public string OldAccount { get; set; }
        public string OldBank { get; set; }
        public string ChangeAccount { get; set; }
        public string ChangeBank { get; set; }

        public bool IsChange
        {
            get
            {
                return (OldAccount != ChangeAccount && !string.IsNullOrEmpty(ChangeAccount))
                    || (OldBank != ChangeBank && !string.IsNullOrEmpty(ChangeBank));
            }
        }
    }
}
