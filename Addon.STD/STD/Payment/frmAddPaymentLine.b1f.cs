using SAPCore;
using SAPCore.Helper;
using STDApp.DataReader;
using STDApp.Models;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Globalization;
using STD.DataReader;

namespace STDApp.Payment
{
    [FormAttribute("STDApp.Payment.frmAddPaymentLine", "Payment/frmAddPaymentLine.b1f")]
    class frmAddPaymentLine : UserFormBase
    {
        private static frmAddPaymentLine instance;
        public static bool IsFormOpen = false;
        public ManualPaymentDetail Payment;

        ChooseFromList oCFL;
        Conditions oConCustomers = null;
        Conditions oConVendors = null;
        public event EventHandler<PaymentEventArgs> OnInputPaymentData;
        public static frmAddPaymentLine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new frmAddPaymentLine();
                    instance.InitControl();
                }
                return instance;
            }
        }
        //private string cardtype;
        public string CardType
        {
            get
            {
                return this.optCus.Selected ? "C" : "V";
            }
        }
        public string CardCode
        {
            get
            {
                return UIHelper.GetTextboxValue(txtBP);
                //if (txtBP == null || txtBP.Value == null || txtBP.Value == "")
                //{
                //    return "";
                //}

                //return txtBP.Value.ToString();
            }
        }
        public string CardName
        {
            get
            {
                return UIHelper.GetTextboxValue(txtName);
                //if (txtName == null || txtName.Value == null || txtName.Value == "")
                //{
                //    return "";
                //}

                //return txtName.Value.ToString();
            }
        }
        //public string PaymentDate
        //{
        //    get
        //    {
        //        return UIHelper.GetTextboxValue(txtDate);
        //        //if (txtDate == null || txtDate.Value == null || txtDate.Value == "")
        //        //{
        //        //    return "";
        //        //}

        //        //return txtDate.Value.ToString();
        //    }
        //}
        private string Currency
        {
            get
            {
                return UIHelper.GetComboValue(cbbCur, "VND");
                //if (cbbCur == null || cbbCur.Selected == null)
                //{
                //    return "VND";
                //}

                //return cbbCur.Selected.Value;
            }
        }
        public decimal Amount
        {
            get
            {
                if (txtAmt == null || txtAmt.Value == null || txtAmt.Value == "")
                {
                    return 0;
                }
                decimal amt = 0;
                if (decimal.TryParse(txtAmt.Value.ToString(), out amt))
                    return amt;
                return 0;
            }
        }
        private frmAddPaymentLine()
        {
        }
        private frmAddPaymentLine(ManualPaymentDetail payment)
        {
            Payment = payment;
        }
        private void InitControl()
        {
            ConfigCFL();

            LoadCurrencyTypeCombobox();
        }


        private void ConfigCFL()
        {
            ChooseFromListCollection oCFLs = null;
            oCFLs = this.UIAPIRawForm.ChooseFromLists;
            oCFL = oCFLs.Item("CFL_BP");
            oConCustomers = null;
            // Condition oCon = null;
            oConCustomers = oCFL.GetConditions();

            var oConCustomer = oConCustomers.Add();
            oConCustomer.Alias = "CardType";

            oConCustomer.Operation = BoConditionOperation.co_EQUAL;
            oConCustomer.CondVal = "C";

            oConVendors = null;
            oConVendors = oCFL.GetConditions();

            var oConVendor = oConVendors.Add();
            oConVendor.Alias = "CardType";

            oConVendor.Operation = BoConditionOperation.co_EQUAL;
            oConVendor.CondVal = "S";
            SetCondition();
        }

        private void SetCondition(string type = "C")
        {
            if (oCFL != null)
            {
                if (type == "C" && oConCustomers != null)
                {
                    oCFL.SetConditions(oConCustomers);
                }
                else
                {
                    if (oConVendors != null)
                        oCFL.SetConditions(oConVendors);
                }
            }
        }

        private void SelectType(string type = "C")
        {
            if (type == "C")
            {
                this.lblBp.Caption = STRING_CONTRANTS.Title_CustomerCode;
                this.lblName.Caption = STRING_CONTRANTS.Title_CustomerName;// "Tên Khách hàng";
            }
            else
            {
                this.lblBp.Caption = STRING_CONTRANTS.Title_VendorCode;
                this.lblName.Caption = STRING_CONTRANTS.Title_VendorName;// "Tên Nhà Cung cấp";
            }
            SetCondition(type);

        }

        private void BindData()
        {

        }

        public void ShowForm(PaymentType paymentType = PaymentType.T)
        {
            //if (paymentType == PaymentType.T)
            //    Instance.SelectType("C");
            //else
                Instance.SelectType("V");

            Instance.Show();
            IsFormOpen = true;
        }
        public void ShowForm(ManualPaymentDetail payment)
        {
            Instance.Show();
            IsFormOpen = true;
        }

        private void LoadCurrencyTypeCombobox()
        {
            //for (var i = cbbCur.ValidValues.Count - 1; i >= 0; i--)
            //{
            //    cbbCur.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
            //}
            UIHelper.ClearSelectValidValues(cbbCur);


            var datas = DataHelper.ListCurrencyInSAP;

            if (datas != null && datas.Length > 0)
            {
                foreach (var data in datas)
                {
                    cbbCur.ValidValues.Add(data["CurrCode"].ToString(), data["CurrName"].ToString());
                }

                cbbCur.Select(GlobalsConfig.Instance.LocalCurrencyDefault, SAPbouiCOM.BoSearchKey.psk_ByValue);
                cbbCur.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
                cbbCur.Item.DisplayDesc = true;
            }
        }


        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.optCus = ((SAPbouiCOM.OptionBtn)(this.GetItem("optCus").Specific));
            this.optCus.PressedAfter += new SAPbouiCOM._IOptionBtnEvents_PressedAfterEventHandler(this.optCus_PressedAfter);
            this.optVen = ((SAPbouiCOM.OptionBtn)(this.GetItem("optVen").Specific));
            this.optVen.PressedAfter += new SAPbouiCOM._IOptionBtnEvents_PressedAfterEventHandler(this.optVen_PressedAfter);
            this.lblBp = ((SAPbouiCOM.StaticText)(this.GetItem("lblBp").Specific));
            this.lblName = ((SAPbouiCOM.StaticText)(this.GetItem("lblName").Specific));
            this.lblCur = ((SAPbouiCOM.StaticText)(this.GetItem("lblCur").Specific));
            this.lblAmt = ((SAPbouiCOM.StaticText)(this.GetItem("lblAmt").Specific));
            this.txtBP = ((SAPbouiCOM.EditText)(this.GetItem("txtBP").Specific));
            this.txtName = ((SAPbouiCOM.EditText)(this.GetItem("txtName").Specific));
            this.cbbCur = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbCur").Specific));
            this.txtAmt = ((SAPbouiCOM.EditText)(this.GetItem("txtAmt").Specific));
            this.btnAdd = ((SAPbouiCOM.Button)(this.GetItem("btnAdd").Specific));
            this.btnAdd.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAdd_ClickBefore);
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCan").Specific));
            this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new SAPbouiCOM.Framework.FormBase.LoadAfterHandler(this.Form_LoadAfter);
            this.CloseAfter += new CloseAfterHandler(this.Form_CloseAfter);

        }

        private SAPbouiCOM.OptionBtn optCus;

        private void OnCustomInitialize()
        {

        }

        private SAPbouiCOM.OptionBtn optVen;
        private SAPbouiCOM.StaticText lblBp;
        private SAPbouiCOM.StaticText lblName;
        private SAPbouiCOM.StaticText lblCur;
        private SAPbouiCOM.StaticText lblAmt;
        private SAPbouiCOM.EditText txtBP;
        private SAPbouiCOM.EditText txtName;
        private SAPbouiCOM.ComboBox cbbCur;
        private SAPbouiCOM.EditText txtAmt;
        private SAPbouiCOM.Button btnAdd;
        private SAPbouiCOM.Button btnCancel;

        private void optCus_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            //if (this.optCus.Selected)
            //    SelectType("C");
        }

        private void optVen_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            //    if (this.optVen.Selected)
            //        SelectType("V");
        }

        private void Form_LoadAfter(SBOItemEventArg pVal)
        {

        }

        private void btnCancel_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.UIAPIRawForm.Close();
        }

        private void btnAdd_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            this.Freeze(true);
            try
            {
                if (string.IsNullOrEmpty(CardCode))
                {
                    UIHelper.LogMessage("Vui lòng chọn thông tin Khách hàng/Nhà cung cấp", UIHelper.MsgType.StatusBar);
                    this.Freeze(false);
                    //this.UIAPIRawForm.Close();
                    return;
                }

                if (Amount == 0)
                {
                    UIHelper.LogMessage("Vui lòng nhập số tiền", UIHelper.MsgType.StatusBar);
                    this.Freeze(false);
                    // this.UIAPIRawForm.Close();
                    return;
                }

                var paymentdetail = new ManualPaymentDetail();
                paymentdetail.CardCode = CardCode;
                paymentdetail.CardName = CardName;
                paymentdetail.Currency = Currency;
                paymentdetail.Amount = Amount;
                paymentdetail.SourceID = $"{DateTime.Now.ToString("yyMMddHHmmss")}";
                paymentdetail.Check = "N"; 
                // pay

                var query = string.Format(QueryString.GetBPInformation, CardCode);
                var data = dbProvider.QuerySingle(query);
                if(data != null)
                {
                    paymentdetail.ReceiveBankCode = data["DflSwift"].ToString();
                    paymentdetail.ReceiveBankName = data["BankName"].ToString();
                    paymentdetail.ReceiveAccount = data["DflAccount"].ToString();
                    paymentdetail.ReceiveAccountName = data["AcctName"].ToString();
                }
                OnInputPaymentData?.Invoke(this, new PaymentEventArgs(paymentdetail));
                //paymentdetail.PaymentDate = 
                //paymentdetail.
            }
            catch (Exception ex)
            {

            }
            this.Freeze(false);
            //this.UIAPIRawForm.Close();
        }

        private void Form_CloseAfter(SBOItemEventArg pVal)
        {
            instance = null;
            IsFormOpen = false;
        }
    }
}
