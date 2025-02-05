using SAPCore.Form;

namespace STDApp.ConfigMenu
{
    public class BankConfig : FunctionConfig
    {
        public BankConfig() : base()
        { }

        public AddonUserForm InquiryForm { get; set; }
        public AddonUserForm PaymentForm { get; set; }

        public AddonUserForm PaymentDetailFormInfo { get; set; }


        protected override void AddFolder(string mainParentID, int index = 0)
        {
            FolderInfomation = new AddonFolder()
            {
                MenuID = "F_Bank",
                MenuName = "Tích hợp ngân hàng",
                ParentID = mainParentID
            };
            base.AddFolder(mainParentID, index);
        }

        protected override void AddMenus()
        {

            InquiryForm = new AddonUserForm()
            {
                FormID = "Inquiry_F",
                MenuID = "PM_Inquiry_M",
                MenuName = "Tích hợp vấn tin tài khoản",
                ParentID = FolderInfomation.MenuID,
                FormType = "STDApp.Bank.frmInquiry"
            };
            AddForms(InquiryForm, 0);

            PaymentForm = new AddonUserForm()
            {
                FormID = "Payment_F",
                MenuID = "PM_Payment_M",
                MenuName = "Thanh toán NCC qua ngân hàng",
                ParentID = FolderInfomation.MenuID,
                FormType = "STDApp.Payment.frmPayment"
            };
            AddForms(PaymentForm, 1);

            PaymentDetailFormInfo = new AddonUserForm()
            {
                //FormID = "PaymentDetail_F",
                MenuID = "PM_Detail_M",
               // MenuName = "Thanh toán chi tiết",
                //ParentID = CusPMFolderID,
                FormType = "STDApp.Payment.frmAddPaymentLine"
            };

        }
    }
}
