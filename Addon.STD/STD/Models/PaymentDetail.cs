using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class ManualPaymentDetail
    {
        public string Check { get; set; }
        public string SourceID { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string ReceiveBankName { get; set; }
        public string ReceiveBankCode { get; set; }
        public string ReceiveAccount { get; set; }
        public string ReceiveAccountName { get; set; }

        public string SenderBankId { get; set; }
        public string SenderAcctName { get; set; }
        public string SenderAcctId { get; set; }
    }                 

    public class PaymentDocument
    {
       // public string CardType { get; set; }
        public string CardCode { get; set; }
        public string Currency { get; set; }
        public string Bank { get; set; }
        public string Cashflow { get; set; }
        public string BankAccount { get; set; }
        public string Account { get; set; }
        public decimal Rate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Remark { get; set; }
        public List<PaymentDetail> Details { get; set; }

        public string Message { get; set; }
        public bool Error { get; set; }
    }
    public class PaymentDetail
    {
        public string CardType { get; set; }
        public string CardCode { get; set; }
        public string DocNum { get; set; }
        public string DocEntry { get; set; }
        //public string InvCode { get; set; }
        public DateTime DocDate { get; set; }
        //public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
       // public decimal AmountBank { get; set; }
      //  public decimal AmountCash { get; set; }
        public string Currency { get; set; }
       // public string BankInfo { get; set; }
        public string BankAccount { get; set; }
        //public string Account { get; set; }
        public string Cashflow { get; set; }
       // public decimal Rate { get; set; }
        public string Remark { get; set; }
    }
}
