using System.Collections;

namespace STD.Models
{
    public class JEAllocate
    {
        public string Account { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string ProfitCode { get; set; }
        public string OcrCode2 { get; set; }
        public string OcrCode3 { get; set; }
        public string OcrCode4 { get; set; }
        public int Order { get; set; }
        public string Year { get; set; }
        public string Period { get; set; }
        public string Sector { get; set; }
        public string BrId { get; set; }
        public string Message { get; set; }// = string.Empty; 
        public string Status { get; set; }
        public string Error { get; set; }
        public int JEEntry { get; set; }

        public JEAllocate(Hashtable data)
        {
            this.Account = data["Account"].ToString();
            decimal debit = 0;
            if(decimal.TryParse(data["Debit"].ToString(), out debit))
                this.Debit = debit;
            decimal credit = 0;
            if (decimal.TryParse(data["Credit"].ToString(), out credit))
                this.Credit = credit;
            this.ProfitCode = data["ProfitCode"].ToString();
            this.OcrCode2 = data["OcrCode2"].ToString();
            this.OcrCode3 = data["OcrCode3"].ToString();
            this.OcrCode4 = data["OcrCode4"].ToString();
            int order = 0;
            if(int.TryParse(data["Order"].ToString(), out order))
                this.Order = order;
            this.Year = data["Year"].ToString();
            this.Period = data["Period"].ToString();
            this.Sector = data["Sector"].ToString();
            this.BrId = data["BrId"].ToString();
           // this.Message = data[""].ToString();
            this.Error = data["Error"].ToString();
            this.Message = data["Message1"].ToString();
        }
    }
}
