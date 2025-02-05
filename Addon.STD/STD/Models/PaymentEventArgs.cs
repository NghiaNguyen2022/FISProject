using System;
using System.ComponentModel;

namespace STDApp.Models
{
    public enum FeeType
    {
        [Description("BEN")]
        BEN,
        [Description("OUR")]
        OUR
    }
    public enum PaymentType
    {
        [Description("T")] T, // phieu thu
        [Description("C")] C // phieu chi
    }

    public enum PaymentDocumentType
    {
        [Description("PT")] PT, // phieu thu
        [Description("PC")] PC, // Phieu chi
        [Description("UC")] UC // Uy nhiem chi
    }

    public enum PaymentMethod
    {
        [Description("C")] Cash, // Cash
        [Description("B")] Bank, // Bank
        [Description("A")] CashBank // Bank
    }

    public enum ApprovalStatus
    {
        Approve,
        Reject,
        Generate,
        Pending,
        NoAction
    }

    public enum UserRole
    {
        Requester,
        Reviewer,
        Approver
    }

    public enum PaymentStatus
    {
        [Description("-")] All,
        [Description("P")] Pending,
        [Description("V")] Reviewed,
        [Description("A")] Approved,
        [Description("R")] Rejected,
        [Description("G")] Generate
    }

    public class PaymentEventArgs : EventArgs
    {
        public ManualPaymentDetail Selected;

        public PaymentEventArgs(ManualPaymentDetail selected)
        {
            Selected = selected;
        }
    }

    public class SelectionEventArgs: EventArgs
    {
        public string Selected;
        public string Type;
        public SelectionEventArgs(string selected, string type)
        {
            Selected = selected;
            Type = type;
        }
    }
}
