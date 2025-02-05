using PN.SmartLib.Helper;
using RestSharp;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.Config;
using SAPCore.Helper;
using SAPCore.SAP;
using SAPCore.SAP.DIAPI;
using STD.DataReader;
using STDApp.AccessSAP;
using STDApp.Common;
using STDApp.DataReader;
using STDApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace STDApp.Payment
{
    [FormAttribute("STDApp.Payment.frmPayment", "Payment/frmPayment.b1f")]
    class frmPayment : UserFormBase
    {
        private static frmPayment instance;
        public bool IsGroup = false;
        private List<int> SelectedDataIndexs = new List<int>();
        public static bool IsFormOpen = false;
        private SAPbouiCOM.DataTable DataTableCbb;
        private List<ManualPaymentDetail> ManualList;
        private string FromDate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtFDate);
            }
        }

        private string ToDate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtTDate);
            }
        }
        //private string PostingDate
        //{
        //    get
        //    {
        //        return UIHelper.GetTextboxValue(txtPosDa, DateTime.Now.ToString("yyyyMMdd"));
        //    }
        //}

        private string Note
        {
            get
            {
                return UIHelper.GetTextboxValue(txtNote);
            }
        }

        private string Bank
        {
            get
            {
                //var query = "SELECT \"Key\" FROM \"" + DIConnection.Instance.CompanyDB + "\".\"vw_Bank_BankAccount\" WHERE \"Account\" = '" + BankAccount + "'";
                //var data = dbProvider.QuerySingle(query);
                //if (data != null)
                //    return data["Key"].ToString();
                //return "VT";
                return UIHelper.GetComboValue(cbbBank);
            }
        }

        private string BankAccount
        {
            get
            {
                // if(cbbBank)
                return UIHelper.GetComboValue(cbbAccount);
            }
        }
        private FeeType _FeeType
        {
            get
            {
                if (cbbType != null && cbbType.Selected != null)
                {
                    var type = cbbFeeType.Selected.Value.GetEnumValueByDescription<FeeType>();
                    return type;
                }

                return FeeType.BEN;
            }
        }
        private string CashCode
        {
            get
            {
                return UIHelper.GetComboValue(cbbAccCa);
            }
        }

        private string CashFlowID
        {
            get
            {
                return UIHelper.GetComboValue(cbbCFlow);
            }
        }

        //private PaymentDocumentType _PaymentDocumentType
        //{
        //    get
        //    {
        //        if (cbbAccount != null && cbbAccount.Selected != null)
        //        {
        //            var type = cbbAccount.Selected.Value.GetEnumValueByDescription<PaymentDocumentType>();

        //            return type;// PaymentDocumentType.UC;
        //            //return cbbPmTyp.Selected.Value == "PT" ? PaymentDocumentType.PT : PaymentType.C;
        //        }

        //        return PaymentDocumentType.PT;
        //    }
        //}

        private string FromDateReport
        {
            get
            {
                return UIHelper.GetTextboxValue(txtFDateL);
            }
        }
        ChooseFromList oCFL;
        Conditions oConCustomers = null;
        Conditions oConVendors = null;

        private string ToDateReport
        {
            get
            {
                return UIHelper.GetTextboxValue(txtTDateL);

            }
        }
        private frmPayment()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.folderCreate = ((SAPbouiCOM.Folder)(this.GetItem("F_Cr").Specific));
            this.folderList = ((SAPbouiCOM.Folder)(this.GetItem("F_Pm").Specific));
            this.lblFDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblFDa").Specific));
            this.lblTDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblTDa").Specific));
            this.txtFDate = ((SAPbouiCOM.EditText)(this.GetItem("txtFDa").Specific));
            this.txtTDate = ((SAPbouiCOM.EditText)(this.GetItem("txtTDa").Specific));
            this.lblType = ((SAPbouiCOM.StaticText)(this.GetItem("lblTy").Specific));
            this.cbbType = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbTy").Specific));
            //    this.cbbType.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbType_ComboSelectAfter);
            this.btnFind = ((SAPbouiCOM.Button)(this.GetItem("btnFind").Specific));
            this.btnFind.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFind_ClickBefore);
            this.grData = ((SAPbouiCOM.Grid)(this.GetItem("grData").Specific));
            this.grData.DoubleClickAfter += new SAPbouiCOM._IGridEvents_DoubleClickAfterEventHandler(this.grData_DoubleClickAfter);
            this.grData.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.grData_ClickAfter);
            this.btnCreate = ((SAPbouiCOM.Button)(this.GetItem("btnCrt").Specific));
            this.btnCreate.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCreate_ClickBefore);
            this.lblBr = ((SAPbouiCOM.StaticText)(this.GetItem("lblBr").Specific));
            this.cbbBr = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBr").Specific));
            this.lblFeeType = ((SAPbouiCOM.StaticText)(this.GetItem("lblFeeT").Specific));
            this.cbbFeeType = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbFeeT").Specific));
            this.cbbFeeType.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbFeeT_ComboSelectAfter);
            this.lblAccount = ((SAPbouiCOM.StaticText)(this.GetItem("lblAcc").Specific));
            this.cbbAccount = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbAcc").Specific));
            this.lblFDateL = ((SAPbouiCOM.StaticText)(this.GetItem("lblFDaL").Specific));
            this.lblTDateL = ((SAPbouiCOM.StaticText)(this.GetItem("lblTDaL").Specific));
            this.txtFDateL = ((SAPbouiCOM.EditText)(this.GetItem("txtFDaL").Specific));
            this.txtTDateL = ((SAPbouiCOM.EditText)(this.GetItem("txtTDaL").Specific));
            this.btnFindL = ((SAPbouiCOM.Button)(this.GetItem("btnFindL").Specific));
            this.btnFindL.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFindL_ClickBefore);
            this.grHdr = ((SAPbouiCOM.Grid)(this.GetItem("grHdr").Specific));
            this.grHdr.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.grHdr_ClickAfter);
            this.grDt = ((SAPbouiCOM.Grid)(this.GetItem("grDt").Specific));
            this.btnAddL = ((SAPbouiCOM.Button)(this.GetItem("btnAddL").Specific));
            this.btnAddL.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAddL_ClickBefore);
            this.btnRem = ((SAPbouiCOM.Button)(this.GetItem("btnRem").Specific));
            this.btnRem.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnRem_ClickBefore);
            this.ckbGr = ((SAPbouiCOM.CheckBox)(this.GetItem("ckbGr").Specific));
            this.ckbGr.PressedAfter += new SAPbouiCOM._ICheckBoxEvents_PressedAfterEventHandler(this.CheckBox0_PressedAfter);
            this.lblBankAccount = ((SAPbouiCOM.StaticText)(this.GetItem("lblBank").Specific));
            this.cbbBank = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBank").Specific));
            this.cbbBank.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbBank_ComboSelectAfter);
            this.lblCFlow = ((SAPbouiCOM.StaticText)(this.GetItem("lblCF").Specific));
            this.cbbCFlow = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbCF").Specific));
            this.txtRate = ((SAPbouiCOM.EditText)(this.GetItem("txtRate").Specific));
            this.ckAppRate = ((SAPbouiCOM.CheckBox)(this.GetItem("ckAppRate").Specific));
            this.ckAppRate.PressedAfter += new SAPbouiCOM._ICheckBoxEvents_PressedAfterEventHandler(this.ckAppRate_PressedAfter);
            this.cbbAccCa = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbAC").Specific));
            this.lblAccCa = ((SAPbouiCOM.StaticText)(this.GetItem("lblAccCa").Specific));
            this.btnChAl = ((SAPbouiCOM.Button)(this.GetItem("btnChAl").Specific));
            this.btnChAl.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnChAl_ClickBefore);
            this.btnUnck = ((SAPbouiCOM.Button)(this.GetItem("btnUnck").Specific));
            this.btnUnck.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnUnck_ClickBefore);
            this.lblPaymentType = ((SAPbouiCOM.StaticText)(this.GetItem("lblPmTy").Specific));
            this.txtNote = ((SAPbouiCOM.EditText)(this.GetItem("txtNote").Specific));
            this.lblNote = ((SAPbouiCOM.StaticText)(this.GetItem("lblNote").Specific));
            this.lblCusVen = ((SAPbouiCOM.StaticText)(this.GetItem("lblCV").Specific));
            this.txtCusVen = ((SAPbouiCOM.EditText)(this.GetItem("txtCV").Specific));
            //      this.txtCusVen.LostFocusAfter += new SAPbouiCOM._IEditTextEvents_LostFocusAfterEventHandler(this.txtCusVen_LostFocusAfter);
            this.btnFilter = ((SAPbouiCOM.Button)(this.GetItem("btnFil").Specific));
            this.btnFilter.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFilter_ClickBefore);
            this.DataTableCbb = this.UIAPIRawForm.DataSources.DataTables.Item("DT_FT");
            this.btnHis = ((SAPbouiCOM.Button)(this.GetItem("btnHis").Specific));
            //      this.btnHis.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnHis_ClickBefore);
            this.btnUpd = ((SAPbouiCOM.Button)(this.GetItem("btnUpd").Specific));
            this.btnUpd.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnUpd_ClickBefore);
            this.cbbPaymentType = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbPmty").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseAfter += new SAPbouiCOM.Framework.FormBase.CloseAfterHandler(this.Form_CloseAfter);
            // this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);
        }

        private void OnCustomInitialize()
        {

        }
        private void SetControlLocation()
        {
            var max = this.UIAPIRawForm.ClientHeight;
            var maxw = this.UIAPIRawForm.ClientWidth;
            SetLocationOfFolderCreateControl();
            SetLocationOfFolderListControl(max, maxw);
        }

        private void SetLocationOfFolderListControl(int max, int maxw)
        {
            //this.folderList.Item.Width = 500;

            //  this.lblFDateL.Item.Top = this.folderCreate.Item.Top + CoreSetting.UF_VerMargin;
            //this.lblFDateL.Item.Left = this.folderCreate.Item.Left + CoreSetting.UF_HorMargin;

            this.txtFDateL.Item.Top = this.lblFDateL.Item.Top;
            this.txtFDateL.Item.Left = this.lblFDateL.Item.Left + this.lblFDateL.Item.Width + 20;

            this.lblTDateL.Item.Top = this.lblFDateL.Item.Top;
            this.lblTDateL.Item.Left = this.txtFDateL.Item.Left + this.txtFDateL.Item.Width + 20;

            this.txtTDateL.Item.Top = this.lblFDateL.Item.Top;
            this.txtTDateL.Item.Left = this.lblTDateL.Item.Left + this.lblTDateL.Item.Width + 20;

            var labBottom = this.lblFDateL.Item.Top + this.lblFDateL.Item.Height;
            var bttTop = labBottom - this.btnFindL.Item.Height;
            this.btnFindL.Item.Top = bttTop;
            this.btnFindL.Item.Left = this.txtTDateL.Item.Left + this.txtTDateL.Item.Width + 30;

            this.grHdr.Item.Left = this.lblFDateL.Item.Left;
            this.grHdr.Item.Width = maxw - grHdr.Item.Left - 20;
            this.grHdr.Item.Top = lblFDateL.Item.Top + lblFDateL.Item.Height + 20;
            var bodyHeight = max - grHdr.Item.Top - 20;
            var headeHeight = (bodyHeight - 10) / 2;
            this.grHdr.Item.Height = headeHeight;

            this.grDt.Item.Left = this.lblFDateL.Item.Left;
            this.grDt.Item.Width = maxw - grDt.Item.Left - 20;
            this.grDt.Item.Top = this.grHdr.Item.Top + this.grHdr.Item.Height + 10;
            this.grDt.Item.Height = max - grDt.Item.Top - 20;
        }

        private void SetLocationOfFolderCreateControl()
        {
            var max = this.UIAPIRawForm.ClientHeight;
            var maxw = this.UIAPIRawForm.ClientWidth;
            //this.folderCreate.Item.Width = 500;

            // this.lblFDate.Item.Top = this.folderCreate.Item.Top + CoreSetting.UF_VerMargin;
            //this.lblFDate.Item.Left = this.folderCreate.Item.Left + CoreSetting.UF_HorMargin;

            this.txtFDate.Item.Top = this.lblFDate.Item.Top;
            this.txtFDate.Item.Left = this.lblFDate.Item.Left + this.lblFDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblTDate.Item.Top = this.lblFDate.Item.Top;
            this.lblTDate.Item.Left = this.txtFDate.Item.Left + this.txtFDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.txtTDate.Item.Top = this.lblFDate.Item.Top;
            this.txtTDate.Item.Left = this.lblTDate.Item.Left + this.lblTDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblBankAccount.Item.Left = this.txtTDate.Item.Left + this.txtTDate.Item.Width + CoreSetting.UF_HorizontallySpaced;
            this.lblBankAccount.Item.Top = this.lblFDate.Item.Top;

            this.cbbBank.Item.Top = this.lblFDate.Item.Top;
            this.cbbBank.Item.Left = this.lblBankAccount.Item.Left + this.lblBankAccount.Item.Width + CoreSetting.UF_HorizontallySpaced;


            this.lblAccount.Item.Left = this.cbbBank.Item.Left + this.cbbBank.Item.Width + CoreSetting.UF_HorizontallySpaced;
            this.lblAccount.Item.Top = this.lblFDate.Item.Top;

            this.cbbAccount.Item.Top = this.lblFDate.Item.Top;
            this.cbbAccount.Item.Left = this.lblAccount.Item.Left + this.lblAccount.Item.Width + CoreSetting.UF_HorizontallySpaced;

            var labBottom = this.lblFDate.Item.Top + this.lblFDate.Item.Height;
            var bttTop = labBottom - this.btnFind.Item.Height;
            this.btnFind.Item.Top = bttTop;
            this.btnFind.Item.Left = maxw - btnFind.Item.Width - CoreSetting.UF_HorMargin;// this.txtCusVen.Item.Left + this.txtCusVen.Item.Width + 10;

            this.lblFeeType.Item.Top = this.lblFDate.Item.Top + this.lblFDate.Item.Height + CoreSetting.UF_VerticallySpaced;
            this.lblFeeType.Item.Left = this.lblFDate.Item.Left;
            this.lblFeeType.Item.Width = this.lblTDate.Item.Width + this.txtFDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.cbbFeeType.Item.Top = this.lblFeeType.Item.Top;
            this.cbbFeeType.Item.Left = this.lblTDate.Item.Left;
            this.cbbFeeType.Item.Width = this.lblTDate.Item.Width + this.txtTDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblCFlow.Item.Top = this.cbbAccount.Item.Top;
            this.lblCFlow.Item.Left = this.lblBankAccount.Item.Left;
            this.lblCFlow.Item.Width = this.lblBankAccount.Item.Width;// + cbbType.Item.Width + 20;

            this.cbbCFlow.Item.Top = this.lblCFlow.Item.Top;
            this.cbbCFlow.Item.Left = this.cbbBank.Item.Left;
            this.cbbCFlow.Item.Width = this.cbbBank.Item.Width;// + this.cbbBr.Item.Width + 20;

            this.lblPaymentType.Item.Top = this.cbbCFlow.Item.Top;// this.txtPosDa.Item.Top;
            this.lblPaymentType.Item.Left = this.lblFeeType.Item.Left;// - this.lblPosDa.Item.Width;
            this.lblPaymentType.Item.Width = this.lblFeeType.Item.Width;

            this.cbbPaymentType.Item.Top = this.cbbCFlow.Item.Top;// this.grData.Item.Top - this.txtPosDa.Item.Height - 10;
            this.cbbPaymentType.Item.Left = this.lblPaymentType.Item.Left + this.lblPaymentType.Item.Width + 10;// maxw - this.txtPosDa.Item.Width - 20;

            this.btnCreate.Item.Left = this.btnFind.Item.Left;// this.txtPosDa.Item.Left + this.txtPosDa.Item.Width + 10;
                                                              //var labBottom1 = this.lblPmTyp.Item.Top + this.lblPmTyp.Item.Height;
                                                              // var bttTop1 = labBottom1 - this.btnCreate.Item.Height;
            this.btnCreate.Item.Top = this.btnFind.Item.Top + this.btnFind.Item.Height + CoreSetting.UF_VerticallySpaced;
            this.btnCreate.Item.Width = this.btnFind.Item.Width;

            this.btnUpd.Item.Left = this.btnFind.Item.Left;
            this.btnUpd.Item.Top = this.btnCreate.Item.Top + this.btnCreate.Item.Height + CoreSetting.UF_VerticallySpaced;
            this.btnUpd.Item.Width = this.btnFind.Item.Width;

            this.lblNote.Item.Left = this.txtFDate.Item.Left;
            this.lblNote.Item.Top = this.lblAccount.Item.Top + this.lblAccount.Item.Height + CoreSetting.UF_VerticallySpaced;

            this.txtNote.Item.Left = this.lblTDate.Item.Left;
            this.txtNote.Item.Top = this.lblNote.Item.Top;
            var lblNoteWidth = (this.cbbPaymentType.Item.Left + this.cbbPaymentType.Item.Width) - this.lblTDate.Item.Left;
            this.txtNote.Item.Width = lblNoteWidth;

            this.btnAddL.Item.Left = this.lblFDate.Item.Left;
            this.btnAddL.Item.Top = this.txtNote.Item.Top + this.txtNote.Item.Height + CoreSetting.UF_VerticallySpaced;

            this.btnChAl.Item.Top = this.btnAddL.Item.Top;
            this.btnChAl.Item.Left = this.btnAddL.Item.Left + this.btnAddL.Item.Width + CoreSetting.UF_VerticallySpaced;

            this.btnUnck.Item.Top = this.btnAddL.Item.Top;
            this.btnUnck.Item.Left = this.btnChAl.Item.Left + this.btnChAl.Item.Width + CoreSetting.UF_VerticallySpaced;

            this.grData.Item.Left = this.lblFDate.Item.Left;
            this.grData.Item.Top = btnAddL.Item.Top + btnAddL.Item.Height + 10;
            this.grData.Item.Width = maxw - grData.Item.Left - 20;
            this.grData.Item.Height = max - grData.Item.Top - 20;

        }


        private SAPbouiCOM.Folder folderCreate;
        private SAPbouiCOM.Folder folderList;
        private SAPbouiCOM.StaticText lblFDate;
        private SAPbouiCOM.StaticText lblTDate;
        private SAPbouiCOM.EditText txtFDate;
        private SAPbouiCOM.EditText txtTDate;
        private SAPbouiCOM.StaticText lblType;
        private SAPbouiCOM.ComboBox cbbType;
        private SAPbouiCOM.Button btnFind;
        private SAPbouiCOM.Grid grData;
        private SAPbouiCOM.Button btnCreate;
        private SAPbouiCOM.StaticText lblBr;
        private SAPbouiCOM.ComboBox cbbBr;
        private SAPbouiCOM.StaticText lblFeeType;
        private SAPbouiCOM.ComboBox cbbFeeType;
        private SAPbouiCOM.StaticText lblAccount;
        private SAPbouiCOM.ComboBox cbbAccount;
        private SAPbouiCOM.StaticText lblFDateL;
        private SAPbouiCOM.StaticText lblTDateL;
        private SAPbouiCOM.EditText txtFDateL;
        private SAPbouiCOM.EditText txtTDateL;
        private SAPbouiCOM.Button btnFindL;
        private SAPbouiCOM.Grid grHdr;
        private SAPbouiCOM.Grid grDt;
        private SAPbouiCOM.Button btnAddL;
        private SAPbouiCOM.Button btnRem;
        private StaticText lblBankAccount;
        private ComboBox cbbBank;
        private StaticText lblCFlow;
        private ComboBox cbbCFlow;

        public static void ShowForm()
        {
            if (instance == null)
            {
                try
                {
                    instance = new frmPayment();
                    instance.InitControl();
                    instance.Show();
                    IsFormOpen = true;
                }
                catch (Exception wex)
                {

                }
            }
        }
        private void InitControl()
        {
            this.folderCreate.Select();

            UIHelper.ComboboxSelectDefault(this.cbbFeeType);
            UIHelper.ComboboxSelectDefault(this.cbbAccount);

            UIHelper.ComboboxSelectDefault(this.cbbBank);
            LoadBankAccountCombobox();
            LoadCashflowCombobox();

            if (this.grData != null && this.grData.DataTable != null)
            {
                this.grData.Columns.Item("FeeAccount").ColumnConfig("Tài khoản chịu phí", false, false);
                this.grData.Columns.Item("ReceiveBankCode").ColumnConfig("Mã NH thụ hưởng", false, true);
                this.grData.Columns.Item("ReceiveAccount").ColumnConfig("Mã TK thụ hưởng", false, true);
                this.grData.Columns.Item("ReceiveBankName").ColumnConfig("Tài khoản thụ hưởng", false, true);
                this.grData.Columns.Item("ReceiveAccountName").ColumnConfig("Tên tài khoản  thụ hưởng", false, true);
            }
        }


        private void LoadBankAccountCombobox()
        {
            UIHelper.LoadAccount(cbbAccount, Bank);
            //this.cbbAccount.ValidValues.Add(STRING_CONTRANTS.AllOption, STRING_CONTRANTS.AllOptionDesc);

            //var datas = DataProvider.QueryList(CoreSetting.DataConnection, string.Format(QueryString.BankLoad, Bank));
            //if (datas != null && datas.Count() > 0)
            //{
            //    foreach (var data in datas)
            //    {
            //        this.cbbAccount.ValidValues.Add(data["Account"].ToString(), data["Account"].ToString());
            //    }
            //    UIHelper.ComboboxSelectDefault(cbbAccount);
            //}

        }

        private void LoadCashflowCombobox()
        {
            UIHelper.ClearSelectValidValues(cbbCFlow);
            var values = DataHelper.ListCashFlows;
            this.cbbCFlow.ValidValues.Add("-", STRING_CONTRANTS.NoChooseCashFlow);
            if (values != null && values.Count() > 0)
            {
                foreach (var data in values)
                {
                    this.cbbCFlow.ValidValues.Add(data["CFWId"].ToString(), data["CFWName"].ToString());
                }
                UIHelper.ComboboxSelectDefault(cbbCFlow);
            }
        }

        private void LoadPaymentTypeCombobox()
        {
            //UIHelper.ClearSelectValidValues(cbbPmTyp);
            //if (_PaymentType == PaymentType.T)
            //{
            //    cbbPmTyp.ValidValues.Add("PT", STRING_CONTRANTS.PaymentType_PT);
            //    SelectType("C");
            //}
            //else
            //{
            //    cbbPmTyp.ValidValues.Add("PC", STRING_CONTRANTS.PaymentType_PC);
            //    cbbPmTyp.ValidValues.Add("UC", STRING_CONTRANTS.PaymentType_UC);
            //    SelectType("V");
            //}
            //UIHelper.ComboboxSelectDefault(cbbPmTyp);

            //UIHelper.ClearSelectValidValues(cbbPmTyL);
            //cbbPmTyL.ValidValues.Add("PT", STRING_CONTRANTS.PaymentType_PT);
            //cbbPmTyL.ValidValues.Add("PC", STRING_CONTRANTS.PaymentType_PC);
            //cbbPmTyL.ValidValues.Add("UC", STRING_CONTRANTS.PaymentType_UC);
            //UIHelper.ComboboxSelectDefault(cbbPmTyL);
        }

        private void LoadDataGridReportDetail(string docnum)
        {
            if (this.grDt != null)
            {
                this.grDt.DataTable.Clear();
                this.grDt.DataTable.ExecuteQuery(string.Format(QueryString.LoadPaymentsReportDetail, docnum));

                this.grDt.Columns.Item("CardCode").TitleObject.Caption = STRING_CONTRANTS.Title_VendorCode;
                this.grDt.Columns.Item("CardCode").Editable = false;

                this.grDt.Columns.Item("CardName").Editable = false;
                this.grDt.Columns.Item("CardName").Visible = false;

                this.grDt.Columns.Item("DocNum").TitleObject.Caption = STRING_CONTRANTS.Title_DocNum;
                this.grDt.Columns.Item("DocNum").Editable = false;

                this.grDt.Columns.Item("DocDate").TitleObject.Caption = STRING_CONTRANTS.Title_DocDate;
                this.grDt.Columns.Item("DocDate").Editable = false;

                this.grDt.Columns.Item("DueDate").TitleObject.Caption = STRING_CONTRANTS.Title_DueDate;
                this.grDt.Columns.Item("DueDate").Editable = false;

                this.grDt.Columns.Item("DocCur").TitleObject.Caption = STRING_CONTRANTS.Title_Currency;
                this.grDt.Columns.Item("DocCur").Editable = false;

                this.grDt.Columns.Item("InsTotal").TitleObject.Caption = STRING_CONTRANTS.Title_InsTotal;
                this.grDt.Columns.Item("InsTotal").Editable = false;

                this.grDt.Columns.Item("MustPay").TitleObject.Caption = STRING_CONTRANTS.Title_MustPay;
                this.grDt.Columns.Item("MustPay").Editable = false;

                this.grDt.Columns.Item("SumAmout").TitleObject.Caption = STRING_CONTRANTS.Title_SumTotal;
                this.grDt.Columns.Item("SumAmout").Editable = false;

                this.grDt.Columns.Item("SumAmoutFC").TitleObject.Caption = STRING_CONTRANTS.Title_SumTotalFC;
                this.grDt.Columns.Item("SumAmoutFC").Editable = false;


                SAPbouiCOM.EditTextColumn oCol2 = null;
                oCol2 = (SAPbouiCOM.EditTextColumn)this.grDt.Columns.Item("CardCode");
                oCol2.LinkedObjectType = SAPObjectType.oBusinessPartners;

                SAPbouiCOM.EditTextColumn oCol1 = null;
                oCol1 = (SAPbouiCOM.EditTextColumn)this.grDt.Columns.Item("DocNum");

                oCol1.LinkedObjectType = SAPObjectType.oPurchaseInvoices;

                this.grDt.AutoResizeColumns();
                //ViewHelper.ColorGridRows(this.grDt, 0, true);
            }
        }

        private void LoadDataGridReport()
        {
            if (this.grHdr != null)
            {
                this.grHdr.DataTable.Clear();
                this.grHdr.DataTable.ExecuteQuery(string.Format(QueryString.LoadPaymentsReport, FromDateReport, ToDateReport));

                if (this.grHdr.DataTable.Rows.Count <= 0)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.NoData, UIHelper.MsgType.StatusBar, false);
                    return;
                }
                this.grHdr.Columns.Item("PaymentKey").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentKey;
                this.grHdr.Columns.Item("PaymentKey").Editable = false;

                this.grHdr.Columns.Item("CardCode").TitleObject.Caption = STRING_CONTRANTS.Title_VendorCode;
                this.grHdr.Columns.Item("CardCode").Editable = false;

                this.grHdr.Columns.Item("DocNum").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocNum;
                this.grHdr.Columns.Item("DocNum").Editable = false;

                this.grHdr.Columns.Item("DocDate").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocDate;
                this.grHdr.Columns.Item("DocDate").Editable = false;

                this.grHdr.Columns.Item("DocCurr").TitleObject.Caption = STRING_CONTRANTS.Title_Currency;
                this.grHdr.Columns.Item("DocCurr").Editable = false;

                this.grHdr.Columns.Item("Amount").TitleObject.Caption = STRING_CONTRANTS.Title_PayAmount;
                this.grHdr.Columns.Item("Amount").Editable = false;


                this.grHdr.Columns.Item("PaymentEntry").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocEntry;
                this.grHdr.Columns.Item("PaymentEntry").Editable = false;

                SAPbouiCOM.EditTextColumn oCol1 = null;
                oCol1 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("PaymentEntry");
                oCol1.LinkedObjectType = SAPObjectType.oVendorPayments;

                //  this.grHdr.CollapseLevel = 1;
                this.grHdr.AutoResizeColumns();
                SAPbouiCOM.EditTextColumn oCol = null;
                oCol = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("CardCode");
                oCol.LinkedObjectType = SAPObjectType.oBusinessPartners;

                var docnum = this.grHdr.DataTable.GetValue("DocNum", 0).ToString();
                if (!string.IsNullOrEmpty(docnum))
                {
                    LoadDataGridReportDetail(docnum);
                }
            }
        }
        private void LoadDataGridCreate(string requestID = "")
        {
            //this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = CardCode;

            if (this.grData != null)
            {
                this.grData.DataTable.Rows.Clear();

                var query = string.Format(QueryString.LoadInvoicesToPayment,
                                                                 FromDate,
                                                                 ToDate,
                                                                 Bank,
                                                                 BankAccount,
                                                                 _FeeType.GetDescription(),
                                                                 "");
                this.grData.DataTable.ExecuteQuery(query);

                if (this.grData.DataTable.Rows.Count <= 0)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.NoData, UIHelper.MsgType.StatusBar, false);
                    return;
                }

                this.grData.Columns.Item("Check").ColumnConfig(STRING_CONTRANTS.Title_Choose, true, true, BoGridColumnType.gct_CheckBox);
                this.grData.Columns.Item("ReceiveBankCode").ColumnConfig("Mã NH thụ hưởng", false, true);
                this.grData.Columns.Item("ReceiveAccount").ColumnConfig("Mã TK thụ hưởng", false, true);
                this.grData.Columns.Item("ReceiveBankName").ColumnConfig("Tài khoản thụ hưởng", false, true);
                this.grData.Columns.Item("ReceiveAccountName").ColumnConfig("Tên tài khoản thụ hưởng", false, true);

                this.grData.Columns.Item("SenderBankCode").ColumnConfig("Ngân hàng gửi", false, true);
                this.grData.Columns.Item("SenderAccount").ColumnConfig("Tài khoản gửi", false, true);
                this.grData.Columns.Item("SenderAccountName").ColumnConfig("Tên tài khoản gửi", false, true);

                this.grData.Columns.Item("CardCode").ColumnConfig(STRING_CONTRANTS.Title_VendorCode, false);

                this.grData.Columns.Item("CardName").TitleObject.Caption = STRING_CONTRANTS.Title_VendorName;
                this.grData.Columns.Item("CardName").Editable = false;

                this.grData.Columns.Item("DocNum").ColumnConfig(STRING_CONTRANTS.Title_DocNum, false);
                this.grData.Columns.Item("DocDate").ColumnConfig(STRING_CONTRANTS.Title_DocDate, false);
                this.grData.Columns.Item("DueDate").ColumnConfig(STRING_CONTRANTS.Title_DueDate, false);
                this.grData.Columns.Item("DocCur").ColumnConfig(STRING_CONTRANTS.Title_Currency, false);
                this.grData.Columns.Item("JrnlMemo").ColumnConfig(STRING_CONTRANTS.Title_Remark, false);
                this.grData.Columns.Item("InsTotal").ColumnConfig(STRING_CONTRANTS.Title_InsTotal, false);
                this.grData.Columns.Item("InsTotalFC").ColumnConfig(STRING_CONTRANTS.Title_InsTotalFC, false);
                this.grData.Columns.Item("MustPay").ColumnConfig(STRING_CONTRANTS.Title_MustPay, false);

                this.grData.Columns.Item("Content").ColumnConfig(STRING_CONTRANTS.Title_Content, false);
                this.grData.Columns.Item("SAPStatus").ColumnConfig(STRING_CONTRANTS.Title_SAPStatus, false);
                this.grData.Columns.Item("BankStatus").ColumnConfig(STRING_CONTRANTS.Title_BankStatus, false);
                this.grData.Columns.Item("Message").ColumnConfig(STRING_CONTRANTS.Title_Message, false);

                this.grData.Columns.Item("Manual").Visible = false;
                this.grData.Columns.Item("requestId").ColumnConfig(string.Empty, false, false);
                this.grData.Columns.Item("transId").ColumnConfig(string.Empty, false, false);

                SAPbouiCOM.EditTextColumn oCol2 = null;
                oCol2 = (SAPbouiCOM.EditTextColumn)this.grData.Columns.Item("CardCode");
                oCol2.LinkedObjectType = SAPObjectType.oBusinessPartners;

                SAPbouiCOM.EditTextColumn oCol1 = null;
                oCol1 = (SAPbouiCOM.EditTextColumn)this.grData.Columns.Item("DocNum");
                oCol1.LinkedObjectType = SAPObjectType.oPurchaseInvoices;

                this.grData.CollapseLevel = 1;
                RemoveEmptyRow();
                this.grData.AutoResizeColumns();

                //for (var rowIndex = 0; rowIndex < grData.Rows.Count; rowIndex++)
                //{
                //    var sapstatus = this.grData.GetValueCustom("SAPStatus", rowIndex); 
                //    if(sapstatus == "02")
                //    {
                //        var commonSetting = grData.CommonSetting;
                //        commonSetting.SetCellEditable(rowIndex, 2, false);
                //    }
                //}
            }
        }



        private void RemoveEmptyRow()
        {
            if (this.grData.Rows.Count > 0)
            {
                for (var index = this.grData.DataTable.Rows.Count - 1; index >= 0; index--)
                {
                    var cardcode = this.grData.DataTable.GetValue("CardCode", index).ToString();
                    if (string.IsNullOrEmpty(cardcode))
                    {
                        this.grData.DataTable.Rows.Remove(index);
                    }
                }
            }
        }


        //private void CreatePayment()
        //{
        //    var message = string.Empty;
        //    var payments = new List<PaymentDetail>();
        //    var currency = string.Empty;
        //    for (var i = 0; i < SelectedDataIndexs.Count; i++)
        //    {
        //        var index = SelectedDataIndexs[i];
        //        if (this.grData.DataTable.GetValue("Check", index).ToString() == "Y")
        //        {
        //            var data = new PaymentDetail();
        //            data.CardType = _PaymentType == PaymentType.T ? "C" : "S";
        //            data.CardCode = this.grData.DataTable.GetValue("CardCode", index).ToString();
        //            if (this.grData.DataTable.GetValue("Manual", index).ToString() == "Data")
        //            {
        //                data.DocNum = this.grData.DataTable.GetValue("DocNum", index).ToString();
        //                data.InvCode = this.grData.DataTable.GetValue("InvCode", index).ToString();
        //                data.DocEntry = this.grData.DataTable.GetValue("DocEntry", index).ToString();

        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("DocDate", index).ToString()))
        //                {
        //                    DateTime docdate;
        //                    if (DateTime.TryParse(this.grData.DataTable.GetValue("DocDate", index).ToString(), out docdate))
        //                    {
        //                        data.DocDate = docdate;
        //                    }
        //                }
        //            }
        //            //data.PaymentDate = 
        //            //if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("PostingDate", index).ToString()))
        //            //{
        //            DateTime duedate;
        //            if (DateTime.TryParseExact(PostingDate, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out duedate))
        //            // DateTime.TryParse(PostingDate, out duedate))
        //            {
        //                data.PaymentDate = duedate;
        //            }
        //            data.Remark = Note;
        //            //}

        //            decimal total = 0;
        //            if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPay", index).ToString()))
        //            {
        //                decimal mustpay;
        //                if (decimal.TryParse(this.grData.DataTable.GetValue("MustPay", index).ToString(), out mustpay))
        //                {
        //                    total = mustpay;
        //                }
        //            }

        //            if (_PaymentMethod == PaymentMethod.Cash)
        //            {
        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPayAsCash", index).ToString()))
        //                {
        //                    decimal mustpay;
        //                    if (decimal.TryParse(this.grData.DataTable.GetValue("MustPayAsCash", index).ToString(), out mustpay))
        //                    {
        //                        data.AmountCash = mustpay;
        //                        data.Amount = mustpay;
        //                    }
        //                }
        //            }
        //            else if (_PaymentMethod == PaymentMethod.Bank)
        //            {
        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPayAsBank", index).ToString()))
        //                {
        //                    decimal mustpay;
        //                    if (decimal.TryParse(this.grData.DataTable.GetValue("MustPayAsBank", index).ToString(), out mustpay))
        //                    {
        //                        data.AmountBank = mustpay;
        //                        data.Amount = mustpay;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                decimal cash = 0;
        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPayAsCash", index).ToString()))
        //                {
        //                    decimal mustpay;
        //                    if (decimal.TryParse(this.grData.DataTable.GetValue("MustPayAsCash", index).ToString(), out mustpay))
        //                    {
        //                        cash = mustpay;
        //                        //data.Amount = mustpay;
        //                    }
        //                }
        //                decimal bank = 0;
        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPayAsBank", index).ToString()))
        //                {
        //                    decimal mustpay;
        //                    if (decimal.TryParse(this.grData.DataTable.GetValue("MustPayAsBank", index).ToString(), out mustpay))
        //                    {
        //                        bank = mustpay;
        //                        //data.Amount = mustpay;
        //                    }
        //                }
        //                data.AmountBank = bank;
        //                data.AmountCash = cash;
        //                data.Amount = bank + cash;
        //            }

        //            var bankCode = this.grData.DataTable.GetValue("Bank", index).ToString();
        //            var cfID = this.grData.DataTable.GetValue("CFlow", index).ToString();

        //            if (string.IsNullOrEmpty(bankCode))
        //                bankCode = BankCode;
        //            data.BankInfo = bankCode;
        //            data.BankAccount = ViewHelper.Banks.Where(x => x.Code == bankCode).Select(x => x.Account).FirstOrDefault();

        //            var cashAccount = this.grData.DataTable.GetValue("Account", index).ToString();
        //            if (string.IsNullOrEmpty(cashAccount))
        //                cashAccount = CashCode;
        //            var CashAccount = ViewHelper.Banks.Where(x => x.Code == cashAccount).Select(x => x.Account).FirstOrDefault();
        //            data.Account = CashAccount;// this.grData.DataTable.GetValue("Account", index).ToString();

        //            data.Currency = this.grData.DataTable.GetValue("DocCur", index).ToString();
        //            if (data.Currency != GlobalsConfig.Instance.LocalCurrencyDefault)
        //            {
        //                decimal rate = 0;
        //                decimal.TryParse(this.grData.DataTable.GetValue("DocRate", index).ToString(), out rate);
        //                data.Rate = rate;
        //                // }
        //            }

        //            if (string.IsNullOrEmpty(cfID) || cfID == "-")
        //                cfID = CashFlowID;

        //            if (string.IsNullOrEmpty(cfID) || cfID == "-")
        //            {
        //                UIHelper.LogMessage(STRING_CONTRANTS.NoCFlow, UIHelper.MsgType.Msgbox, true);
        //                return;
        //            }

        //            data.Cashflow = cfID;

        //            if (data.Amount == 0)
        //            {
        //                UIHelper.LogMessage(STRING_CONTRANTS.PaymentZero, UIHelper.MsgType.Msgbox, true);
        //                return;
        //            }

        //            if (total < data.Amount)
        //            {
        //                UIHelper.LogMessage(STRING_CONTRANTS.OverTotalPayment, UIHelper.MsgType.Msgbox, true);
        //                return;
        //            }
        //            payments.Add(data);
        //            // }

        //        }
        //    }

        //    if (payments.Count <= 0)
        //    {
        //        UIHelper.LogMessage(STRING_CONTRANTS.NoData, UIHelper.MsgType.Msgbox, true);
        //        return;
        //    }
        //    var numberBranch = DataHelper.LoadKey(Branch, _PaymentDocumentType.GetDescription());
        //    if (numberBranch == null)
        //    {
        //        UIHelper.LogMessage(STRING_CONTRANTS.CanNotGenerateKey, UIHelper.MsgType.Msgbox, true);
        //        return;
        //    }
        //    var key = $"WL-{numberBranch.BranchID}-{_PaymentDocumentType.GetDescription()}-{DateTime.Now.Year}-{DateTime.Now.Month}-{numberBranch.Number.ToString("D5")}";

        //    var ret = PaymentViaDI.CreatePaymnents(_PaymentType, payments, key, _PaymentDocumentType, _PaymentMethod, Branch, ref message);

        //    var success = ret.Where(x => x.Flag).Count();
        //    var error = ret.Count - success;

        //    var mess = string.Format(STRING_CONTRANTS.Notice_EndAction, STRING_CONTRANTS.Action_Create, success, ret.Count, error);
        //    UIHelper.LogMessage(mess, UIHelper.MsgType.StatusBar);

        //    if (error > 0)
        //    {
        //        var errMessage = string.Empty;
        //        foreach (var data in ret.Where(x => !x.Flag))
        //        {
        //            errMessage += $"Thanh toán cho/của {data.CardCode} lỗi {data.Message} \n";
        //        }
        //        UIHelper.LogMessage(errMessage, UIHelper.MsgType.Msgbox);
        //    }

        //    LoadDataGridCreate();

        //}

        private TransferRecord DataToRecordApi(int index, string requestID)
        {
            try
            {
                var docnum = string.Empty;
                var source = this.grData.GetValueCustom("Manual", index);

                if (source == "Data")
                {
                    docnum = this.grData.GetValueCustom("DocNum", index);
                }
                else
                {
                    if (ManualList != null)
                    {
                        var paymentDetail = ManualList.Where(x => x.SourceID == source).FirstOrDefault();
                        paymentDetail.Check = "Y";
                        var queryManualPayemnt = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_PaymentOnAccount\" VALUES ( ";
                        queryManualPayemnt += $"'{requestID}',";
                        queryManualPayemnt += $"'{paymentDetail.SourceID}',";
                        queryManualPayemnt += $"'{paymentDetail.CardCode}',";
                        queryManualPayemnt += $"'{paymentDetail.Currency}',";
                        queryManualPayemnt += $"{paymentDetail.Amount}";
                        queryManualPayemnt += ")";

                        var retManualPayemnt = dbProvider.ExecuteNonQuery(queryManualPayemnt);
                    }
                }

                var cardCode = this.grData.GetValueCustom("CardCode", index);

                var transId = DateTime.Now.ToString("yyyyMMddHHmmss");
                decimal amount = 0;
                decimal mustpay = 0;
                if (CustomConverter.ConvertStringToDecimal(this.grData.GetValueCustom("MustPay", index), ref mustpay))
                {
                    amount = mustpay;
                }

                var query = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_TransferRecord\" VALUES ( ";
                query += $"'{cardCode}',";
                query += $"'{docnum}',";
                query += $"'{requestID}',";
                query += $"'{transId}',";
                query += $"{amount},";
                query += $"'N',";
                query += $"'', '', 'VT'";
                query += ")";

                var ret1 = dbProvider.ExecuteNonQuery(query);
                var receiverBank = this.grData.GetValueCustom("ReceiveBankName", index);
                var receiverName = this.grData.GetValueCustom("ReceiveAccountName", index);
                var receiverBankID = this.grData.GetValueCustom("ReceiveBankCode", index);
                var receiverBranch = "";
                var receiverAccount = this.grData.GetValueCustom("ReceiveAccount", index);
                var type = "";
                var senderBankId = this.grData.GetValueCustom("SenderBankCode", index);
                if (senderBankId == receiverBankID)
                {
                    type = "in";
                }
                else
                {
                    if (amount > 300000000)
                    {
                        type = "ou";
                    }
                    else
                    {
                        type = "np";
                    }
                }

                //// code test 
                //var valueTxT = this.txtNote.Value;
                //if (valueTxT == "napas")
                //{
                //    receiverBank = "vietinbank";
                //    receiverName = "Nguyen van napas";
                //    receiverBankID = "970406";
                //    receiverBranch = "";
                //    receiverAccount = "0129837294";
                //    type = "np";
                //}
                //else if (valueTxT == "citad")
                //{
                //    receiverBank = "ngân hàng nông nghiệp viet nam";
                //    receiverName = "mat tran to quoc viet nam";
                //    receiverBankID = "01204001";
                //    receiverBranch = "01204001";
                //    receiverAccount = "1483201009159";
                //    type = "ou";
                //}
                //else if (valueTxT == "in")
                //{
                //    receiverBank = this.grData.GetValueCustom("ReceiveBankName", index);
                //    receiverName = this.grData.GetValueCustom("ReceiveAccountName", index);
                //    receiverBankID = this.grData.GetValueCustom("ReceiveBankCode", index);
                //    receiverBranch = "";
                //    receiverAccount = this.grData.GetValueCustom("ReceiveAccount", index);
                //    type = "in";
                //}

                var record = new TransferRecord
                {
                    transId = transId, //mã giao dịch
                    approver = APIVietinBankConstrant.USER_APPROVE,// ConfigurationManager.AppSettings["USER_APPROVE"],
                    transType = type,
                    amount = amount.ToString(),
                    recvAcctId = receiverAccount,
                    recvBankId = receiverBankID,
                    recvBranchId = receiverBranch,
                    recvBankName = receiverBank,
                    recvAcctName = receiverName,
                    recvAddr = "",
                    currencyCode = this.grData.GetValueCustom("DocCur", index),
                    remark = txtNote.Value,// "test",// this.grData.GetValueCustom("JrnlMemo", index),
                    senderBankId = this.grData.GetValueCustom("SenderBankCode", index),
                    senderBranchId = this.grData.GetValueCustom("SenderBankCode", index),
                    senderAddr = "",
                    senderAcctName = this.grData.GetValueCustom("SenderAccountName", index),
                    senderAcctId = this.grData.GetValueCustom("SenderAccount", index)
                };

                return record;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private PaymentDetail DataInRow(int index, ref bool isReturn, ref string message)
        {
            try
            {
                //var record = new  
                var data = new PaymentDetail();

                var currency = this.grData.DataTable.GetValue("DocCur", index).ToString();
                if (string.IsNullOrEmpty(currency))
                {
                    message = STRING_CONTRANTS.NoCurrency;
                    isReturn = true;
                    return null;
                }

                data.Cashflow = CashFlowID;
                data.Currency = currency;


                data.BankAccount = BankAccount;// ViewHelper.Banks.Where(x => x.Code == bankCode).Select(x => x.Account).FirstOrDefault();  

                data.CardType = "S";
                data.CardCode = this.grData.GetValueCustom("CardCode", index);
                if (this.grData.GetValueCustom("Manual", index) == "Data")
                {
                    data.DocNum = this.grData.GetValueCustom("DocNum", index);
                    data.DocEntry = this.grData.GetValueCustom("DocEntry", index);
                    DateTime docdate = default(DateTime);
                    if (CustomConverter.ConvertStringToDate(this.grData.GetValueCustom("DocDate", index), ref docdate))
                    {
                        data.DocDate = docdate;
                    }
                }
                else
                {
                    data.DocNum = string.Empty;
                }
                DateTime duedate;
                //var postingDate = string.Empty;
                //if (!string.IsNullOrEmpty(PostingDate))
                //    postingDate = PostingDate;
                //else
                //    postingDate = this.grData.GetValueCustom("DueDate", index);

                //if (DateTime.TryParseExact(postingDate, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out duedate))
                //{
                //    data.PaymentDate = duedate;
                //}
                data.Remark = Note;

                decimal total = 0;
                decimal mustpay = 0;
                if (CustomConverter.ConvertStringToDecimal(this.grData.GetValueCustom("MustPay", index), ref mustpay))
                {
                    total = mustpay;
                }
                data.Amount = total;
                if (data.Amount == 0)
                {
                    message = STRING_CONTRANTS.PaymentZero;
                    isReturn = true;
                    return null;
                }

                if (total < data.Amount)
                {
                    message = STRING_CONTRANTS.OverTotalPayment;
                    isReturn = true;
                    return null;
                }
                isReturn = false;
                return data;
            }
            catch (Exception ex)
            {
                message = $"Error {ex.Message}";
                isReturn = true;
                return null;
            }

        }
        private void CreatePaymentBIDV()
        {
            var datas = new Dictionary<string, List<int>>();

            for (var i = 0; i < SelectedDataIndexs.Count; i++)
            {
                var index = SelectedDataIndexs[i];

                if (this.grData.GetValueCustom("Check", index) != "Y")
                {
                    continue;
                }

                var cardCode = this.grData.GetValueCustom("CardCode", index);
                if (datas.ContainsKey(cardCode))
                {
                    datas[cardCode].Add(index);
                }
                else
                {
                    var dataint = new List<int>();
                    dataint.Add(index);
                    datas.Add(cardCode, dataint);
                }

            }

            foreach (KeyValuePair<string, List<int>> itm in datas)
            {
                var requestID = "BIDV" + DateTime.Now.ToString("yyyyMMddHHmmss");
                foreach (var index in itm.Value)
                {
                    var docnum = string.Empty;
                    var source1 = this.grData.GetValueCustom("Manual", index);
                    if (source1 == "Data")
                    {
                        docnum = this.grData.GetValueCustom("DocNum", index);
                    }
                    else
                    {
                        if (ManualList != null)
                        {
                            var paymentDetail = ManualList.Where(x => x.SourceID == source1).FirstOrDefault();
                            paymentDetail.Check = "Y";
                            var queryManualPayemnt = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_PaymentOnAccount\" VALUES ( ";
                            queryManualPayemnt += $"'{requestID}',";
                            queryManualPayemnt += $"'{paymentDetail.SourceID}',";
                            queryManualPayemnt += $"'{paymentDetail.CardCode}',";
                            queryManualPayemnt += $"'{paymentDetail.Currency}',";
                            queryManualPayemnt += $"{paymentDetail.Amount}";
                            queryManualPayemnt += ")";

                            var retManualPayemnt = dbProvider.ExecuteNonQuery(queryManualPayemnt);
                        }
                    }

                    var cardCode = this.grData.GetValueCustom("CardCode", index);

                    var transId = DateTime.Now.ToString("yyyyMMddHHmmss");
                    decimal amount = 0;
                    decimal mustpay = 0;
                    if (CustomConverter.ConvertStringToDecimal(this.grData.GetValueCustom("MustPay", index), ref mustpay))
                    {
                        amount = mustpay;
                    }

                    var query = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_TransferRecord\" VALUES ( ";
                    query += $"'{cardCode}',";
                    query += $"'{docnum}',";
                    query += $"'{requestID}',";
                    query += $"'',";
                    query += $"{amount},";
                    query += $"'N',";
                    query += $"'', '', 'BI'";
                    query += ")";

                    var ret2 = dbProvider.ExecuteNonQuery(query);
                }

                var message = string.Empty;
                var currency = string.Empty;
                var source = this.grData.GetValueCustom("Manual", itm.Value.FirstOrDefault());

                var cardcode = this.grData.GetValueCustom("CardCode", itm.Value.FirstOrDefault());

                var senderAccount = this.grData.GetValueCustom("SenderAccount", itm.Value.FirstOrDefault());
                var receiverAccount = this.grData.GetValueCustom("ReceiveAccount", itm.Value.FirstOrDefault());
                var receiverName = this.grData.GetValueCustom("ReceiveAccountName", itm.Value.FirstOrDefault());
                var receiverBankID = this.grData.GetValueCustom("ReceiveBankCode", itm.Value.FirstOrDefault());
                var receiverBank = this.grData.GetValueCustom("ReceiveBankName", itm.Value.FirstOrDefault());
                var curr = this.grData.GetValueCustom("DocCur", itm.Value.FirstOrDefault());

                var transType = "1";
                var transText = txtNote.Value;
                if (transText == "ext")
                    transType = "2";
                else if (transText == "247")
                    transType = "3";
                else
                    transType = "1";
                var charge = "I";
                if (transType == "2")
                    charge = "E";
                var remark = "Test";

                decimal total = 0;
                foreach (var i in itm.Value)
                {
                    decimal mustpay = 0;
                    if (CustomConverter.ConvertStringToDecimal(this.grData.GetValueCustom("MustPay", i), ref mustpay))
                    {
                        total += mustpay;
                    }
                }

                var messageNotice = string.Empty;
                messageNotice = $"Bạn có muốn thanh toán số tiền {total} cho {cardcode}";

                var ret1 = UIHelper.LogMessage(messageNotice, UIHelper.MsgType.Msgbox, false, 1, "No", "Yes");
                if (ret1 == 2)
                {
                    var body = new BIDVTransferRequest()
                    {
                        appAcct = senderAccount,
                        benAcct = receiverAccount,
                        benAcctName = receiverName,
                        benbankCode = receiverBankID,
                        benbankName = receiverBank,
                        tranType = transType,
                        amount = total,
                        curr = curr,
                        charge = charge,
                        remark = remark,
                        @ref = requestID
                    };

                    CreatePaymentBIDV(body, ref requestID);
                }
            }

        }

        private void CreatePaymentBIDV(BIDVTransferRequest body, ref string requestID)
        {

            var mesage = string.Empty;
            var token = string.Empty;
            token = APIHelper.GetToken(ref mesage);
            if (string.IsNullOrEmpty(token))
            {
                if (string.IsNullOrEmpty(mesage))
                    UIHelper.LogMessage($"Không lấy được token từ phía ngân hàng, vui lòng thử lại",
                        UIHelper.MsgType.StatusBar, true);
                else
                    UIHelper.LogMessage(mesage,
                        UIHelper.MsgType.StatusBar, true);

                return;
            }
            // var path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "certificate.cer";
            var certificate = APIUtil.GetData();// string.Empty;

            try
            {
                UIHelper.LogMessage($"Bắt đầu vấn gửi yêu cầu tin tài khoản đến ngân hàng",
                    UIHelper.MsgType.StatusBar, false);
                var options = new RestClientOptions(APIBIDVConstrant.APILink);
                var client = new RestClient(options);
                var request = new RestRequest(APIBIDVConstrant.TransferBIDV + "?scope=create &JWE=Yes", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
                request.AddHeader("Channel", APIBIDVConstrant.ChannelBIDVAPI);// ConfigurationManager.AppSettings["ChannelBIDVAPI"]);
                request.AddHeader("User-Agent", APIBIDVConstrant.UserAgentIDVAPI);// ConfigurationManager.AppSettings["UserAgentIDVAPI"]);
                request.AddHeader("accept", "application/json");

                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddHeader("X-Client-Certificate", certificate);

                var requestId = DateTime.Now.ToString("yyyyMMddHHmmss");
                request.AddHeader("X-API-Interaction-ID", requestId);

                var symmetricKey = ConfigurationManager.AppSettings["SymmetricKey"];
                var symmetricKeys = APIUtil.HexToByteArray(symmetricKey);

                string payload = "{\"body\":" + JsonSerializer.Serialize(body) + "}";

                var encryptData = APIUtil.doEncryptJWE(payload, symmetricKey);
                request.AddStringBody(encryptData, DataFormat.Json);

                var signature = APIUtil.doSignatureJWS(encryptData);
                if (string.IsNullOrEmpty(signature))
                {
                    UIHelper.LogMessage($"Không tạo được chữ ký, vui lòng kiểm tra lại",
                       UIHelper.MsgType.StatusBar, true);
                    return;
                }
                request.AddHeader("X-JWS-Signature", signature);

                var response = client.Execute(request);

                var result = response.Content;
                try
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        var status = response.StatusCode;
                        if (status == System.Net.HttpStatusCode.InternalServerError)
                        {
                            var errorResp = JsonSerializer.Deserialize<BIDVResponse500Error>(result);
                            if (errorResp != null)
                            {
                                if (errorResp.errorResponse.additionalInfo != null
                                    && errorResp.errorResponse.additionalInfo.detailedError != null)
                                    UIHelper.LogMessage(errorResp.errorResponse.additionalInfo.detailedError.description, UIHelper.MsgType.StatusBar, true);
                            }
                            else
                            {
                                UIHelper.LogMessage($"Lỗi kết nối. Vui lòng thử lại", UIHelper.MsgType.StatusBar, true);
                            }
                            return;
                        }
                        if (status == System.Net.HttpStatusCode.Unauthorized)
                        {
                            UIHelper.LogMessage($"Lỗi chứng thực. Vui lòng kiểm tra thông tin chứng thực và kết nối", UIHelper.MsgType.StatusBar, true);
                            return;
                        }
                        if (status == System.Net.HttpStatusCode.BadRequest)
                        {
                            UIHelper.LogMessage($"Gửi yêu cầu thất bại, vui lòng thử lại", UIHelper.MsgType.StatusBar, true);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    UIHelper.LogMessage($"Lỗi {response.ErrorMessage}", UIHelper.MsgType.StatusBar, true);
                    return;
                }

                var rps = JsonSerializer.Deserialize<BIDVTransferResponse>(result);
                if (rps == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var data = rps.body;
                if (data != null && !string.IsNullOrEmpty(data.PaymentID))
                {
                    try
                    {
                        var query = "UPDATE \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_TransferRecord\" ";
                        query += "SET \"status\" = '11' ,";
                        query += "\"transId\" = '" + data.PaymentID + "' ";
                        query += "WHERE \"requestId\" = '" + requestID + "' ";
                        query += "AND \"bank\" = 'BI' ";

                        var ret = dbProvider.ExecuteNonQuery(query);
                        UIHelper.LogMessage("Hoàn tất gửi yêu cầu thanh toán", UIHelper.MsgType.StatusBar, false);
                    }
                    catch (Exception ex)
                    {
                        UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                        return;
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            { }

        }

        private void CreatePaymentVTB(int index)
        {
            var message = string.Empty;
            var currency = string.Empty;
            var source = this.grData.GetValueCustom("Manual", index);

            var cardcode = this.grData.GetValueCustom("CardCode", index);
            var messageNotice = string.Empty;
            var docnum = string.Empty;
            if (source == "Data")
            {
                docnum = this.grData.GetValueCustom("DocNum", index);
                messageNotice = $"Bạn có muốn thanh toán hóa đơn {docnum} cho {cardcode}";
            }
            else
            {
                var amount = this.grData.GetValueCustom("MustPay", index);
                messageNotice = $"Bạn có muốn thanh toán số tiền {amount} cho {cardcode}";
            }
            var ret1 = UIHelper.LogMessage(messageNotice, UIHelper.MsgType.Msgbox, false, 1, "No", "Yes");
            if (ret1 == 2)
            {
                UIHelper.LogMessage("Bắt đầu lấy dữ liệu", UIHelper.MsgType.StatusBar, false);
                var request = new TransferRequest
                {
                    model = "2",
                    requestId = "12345678" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    merchantId = APIVietinBankConstrant.MerchantId,// ConfigurationManager.AppSettings["MerchantId"],
                    providerId = APIVietinBankConstrant.ProviderId,// ConfigurationManager.AppSettings["ProviderId"],
                    priority = "3",
                    version = "1",
                    softwareProviderId = "FPT",
                    language = "vi",
                    appointedApprover = APIVietinBankConstrant.USER_APPROVE,// ConfigurationManager.AppSettings["USER_APPROVE"],
                    feeAccount = BankAccount,// ConfigurationManager.AppSettings["Account"],
                    feeType = _FeeType.GetDescription(),
                    scheduledDate = "",
                    approver = ConfigurationManager.AppSettings["USER_APPROVE"],
                    transTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    clientIP = APIVietinBankConstrant.USER_APPROVE,//APIVietinBankConstrant.ClientIP,//.AppSettings["ClientIP"],
                    channel = "MOBILE",
                    signature = "",
                    records = new List<TransferRecord>()
                };


                request.records.Add(DataToRecordApi(index, request.requestId)); // DataInRow(index, ref isReturn, ref message);

                request.signature = (
                    request.requestId +
                    request.providerId +
                    request.merchantId +
                    request.model +
                    request.priority +
                    request.softwareProviderId +
                    request.appointedApprover +
                    request.feeAccount +
                    request.feeType +
                    request.scheduledDate +
                    request.approver
                  );

                foreach (var item in request.records)
                {
                    request.signature += (
                        item.transId +
                        item.senderAcctId +
                        item.recvAcctId +
                        item.amount
                    );
                }

                request.signature += (
                    request.transTime +
                    request.channel +
                    request.version +
                    request.clientIP +
                    request.language
                );
                var path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";

                //request.signature = FPT.SHA256_RSA2048.Encrypt(request.signature, path);
                request.signature = FPT.SHA256_RSA2048.Encrypt(request.signature, path);
                var json = JsonSerializer.Serialize(request);

                UIHelper.LogMessage("Bắt đầu gửi yêu cầu", UIHelper.MsgType.StatusBar, false);

                var options = new RestClientOptions(APIVietinBankConstrant.APIVTB)
                {
                    MaxTimeout = -1,
                };

                var client = new RestClient(options);
                var request1 = new RestRequest(APIVietinBankConstrant.TransferVTB, Method.Post);
                request1.AddHeader("x-ibm-client-id", APIVietinBankConstrant.ClientID);// ConfigurationManager.AppSettings["ClientID"]);
                request1.AddHeader("x-ibm-client-secret", APIVietinBankConstrant.ClientSecret);// ConfigurationManager.AppSettings["ClientSecret"]);
                request1.AddHeader("Content-Type", "application/json");

                request1.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = client.Execute(request1);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    UIHelper.LogMessage($"Lỗi {response.ErrorMessage}", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var result = response.Content;

                var rps = JsonSerializer.Deserialize<VTResponse>(result);
                if (rps == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                if (rps.status.code == "0")
                {
                    UIHelper.LogMessage($"Lỗi {rps.status.message}", UIHelper.MsgType.StatusBar, true);
                    //return;
                }
                var dataResponse = JsonSerializer.Deserialize<TransferHeader>(result);

                if (dataResponse != null && dataResponse.records != null)
                {
                    foreach (var item in dataResponse.records)
                    {
                        if (item != null)
                        {
                            try
                            {
                                var query = "UPDATE \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_TransferRecord\" ";
                                query += "SET \"status\" = '" + item.code + "' ,";
                                query += "\"Message\" = '" + item.message + "' ";
                                query += "WHERE \"requestId\" = '" + dataResponse.requestId + "' ";
                                query += "AND \"transId\" = '" + item.transId + "' ";
                                query += "AND \"bank\" = 'VT' ";

                                var ret = dbProvider.ExecuteNonQuery(query);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }

        }
        private void CreatePayments()
        {
            UIHelper.LogMessage("Bắt đầu quá trình gửi yêu cầu lên ngân hàng", UIHelper.MsgType.StatusBar, false);

            if (Bank == Banks.ViettinBank.GetDescription())
            {
                for (var i = 0; i < SelectedDataIndexs.Count; i++)
                {
                    var index = SelectedDataIndexs[i];

                    if (this.grData.GetValueCustom("Check", index) != "Y")
                    {
                        continue;
                    }

                    this.CreatePaymentVTB(index);
                }
            }
            else
            {
                this.CreatePaymentBIDV();
            }

            //UIHelper.LogMessage("Hoàn tất gửi yêu cầu thanh toán", UIHelper.MsgType.StatusBar, false);
            LoadDataGridCreate();

        }
        private void AutoFillData(int selectedIndex = -1)
        {
            this.Freeze(true);
            try
            {
                if (selectedIndex == -1)
                {
                    foreach (var index in SelectedDataIndexs)
                    {
                        //    var mustpay = this.grData.DataTable.GetValue("MustPay", index);
                        //    if (_PaymentMethod == PaymentMethod.CashBank)
                        //    {
                        //        this.grData.DataTable.SetValue("MustPayAsCash", index, mustpay);
                        //        this.grData.DataTable.SetValue("MustPayAsBank", index, 0);
                        //    }
                        //    else if (_PaymentMethod == PaymentMethod.Cash)
                        //    {

                        //        this.grData.DataTable.SetValue("MustPayAsCash", index, mustpay);
                        //        this.grData.DataTable.SetValue("MustPayAsBank", index, 0);
                        //    }
                        //    else
                        //    {

                        //        this.grData.DataTable.SetValue("MustPayAsCash", index, 0);
                        //        this.grData.DataTable.SetValue("MustPayAsBank", index, mustpay);
                        //    }

                        //var bank = this.grData.DataTable.GetValue("Bank", index);
                        //var cashflow = this.grData.DataTable.GetValue("CFlow", index);
                    }
                }
                else
                {
                    //var mustpay = this.grData.DataTable.GetValue("MustPay", selectedIndex);
                    //if (_PaymentMethod == PaymentMethod.CashBank)
                    //{
                    //    this.grData.DataTable.SetValue("MustPayAsCash", selectedIndex, mustpay);
                    //    this.grData.DataTable.SetValue("MustPayAsBank", selectedIndex, 0);
                    //}
                    //else if (_PaymentMethod == PaymentMethod.Cash)
                    //{

                    //    this.grData.DataTable.SetValue("MustPayAsCash", selectedIndex, mustpay);
                    //    this.grData.DataTable.SetValue("MustPayAsBank", selectedIndex, 0);
                    //}
                    //else
                    //{

                    //    this.grData.DataTable.SetValue("MustPayAsCash", selectedIndex, 0);
                    //    this.grData.DataTable.SetValue("MustPayAsBank", selectedIndex, mustpay);
                    //}
                }
            }
            catch (Exception ex)
            {

            }
            this.Freeze(false);
        }
        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            instance = null;
            IsFormOpen = false;
        }

        private void btnFind_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

            if (string.IsNullOrEmpty(FromDate) || string.IsNullOrEmpty(ToDate))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_DateSelectNull, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }

            if (!StringUtils.CheckFromDateEarlyToDate(FromDate, ToDate))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_FromDateEarlyToDate, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }
            try
            {
                LoadDataGridCreate();
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndLoadData);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private void btnCreate_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            UIHelper.LogMessage(STRING_CONTRANTS.Notice_CreatePayment);
            try
            {
                CreatePayments();
                //UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndCreatePayment);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_CreatePayment, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);
            SetControlLocation();
            this.Freeze(false);

        }
        private void grData_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);
            if (pVal.Row < 0)
            {
                this.Freeze(false);
                return;
            }
            if (pVal.ColUID != "Check")
            {
                this.Freeze(false);
                return;
            }

            var index1 = this.grData.GetDataTableRowIndex(pVal.Row);
            var sapstatus = this.grData.GetValueCustom("SAPStatus", index1);
            if (sapstatus == "02")
            {
                UIHelper.LogMessage("Phiếu này đã gửi đi sang ngân hàng", UIHelper.MsgType.StatusBar);
                this.grData.DataTable.SetValue("Check", index1, "N");
                this.Freeze(false);
                return;
            }

            UIHelper.CheckInGrid(this.grData, pVal.Row, SelectedDataIndexs, "CardCode");

            foreach (var index in SelectedDataIndexs)
            {
                AutoFillData(index);
            }

            EnableButton();

            //this.grData.Rows.se
            ViewHelper.ColorGridRows(this.grData, 0, true);
            this.Freeze(false);
        }

        private void EnableButton()
        {
            if (SelectedDataIndexs.Count > 0)
            {
                this.btnCreate.Item.Enabled = true;
                this.btnUnck.Item.Enabled = true;
            }
            else
            {
                this.btnCreate.Item.Enabled = false;
                this.btnUnck.Item.Enabled = false;
            }

            if (SelectedDataIndexs.Count == this.grData.DataTable.Rows.Count)
            {
                this.btnChAl.Item.Enabled = false;
            }
            else
            {
                this.btnChAl.Item.Enabled = true;
            }
        }

        private void grData_DoubleClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

        }

        private void btnAddL_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //this.Freeze(true);
            if (this.grData == null || this.grData.DataTable == null)
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadDataBefAdd, UIHelper.MsgType.StatusBar);
                return;
            }
            try
            {
                frmAddPaymentLine.Instance.OnInputPaymentData += Instance_OnInputPaymentData;
                frmAddPaymentLine.Instance.ShowForm();
            }
            catch (Exception ex)
            { }
            // this.Freeze(false);
        }

        private void Instance_OnInputPaymentData(object sender, Models.PaymentEventArgs e)
        {
            try
            {
                var data = e.Selected;
                ((frmAddPaymentLine)sender).Close();
                if (data == null)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.Notice_NoDataToAdd, UIHelper.MsgType.StatusBar);
                    return;
                }

                if (this.grData == null || this.grData.DataTable == null)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.Notice_NoDataToAdd, UIHelper.MsgType.StatusBar);
                    return;
                }

                this.Freeze(true);
                if (ManualList == null)
                    ManualList = new List<ManualPaymentDetail>();
                ManualList.Add(data);

                this.grData.DataTable.Rows.Add();
                var index = grData.DataTable.Rows.Count - 1;

                this.grData.DataTable.SetValue("CardCode", index, data.CardCode);
                this.grData.DataTable.SetValue("CardName", index, data.CardName);
                this.grData.DataTable.SetValue("DocCur", index, data.Currency);
                this.grData.DataTable.SetValue("MustPay", index, data.Amount.ToString());
                this.grData.DataTable.SetValue("ReceiveBankCode", index, data.ReceiveBankCode);
                this.grData.DataTable.SetValue("ReceiveAccountName", index, data.ReceiveAccountName);
                this.grData.DataTable.SetValue("ReceiveBankName", index, data.ReceiveBankName);
                this.grData.DataTable.SetValue("ReceiveAccount", index, data.ReceiveAccount);
                this.grData.DataTable.SetValue("Manual", index, data.SourceID);

                var values = DataHelper.ListBanks;
                var bank = values.FirstOrDefault();
                if (bank != null)
                {
                    this.grData.DataTable.SetValue("SenderBankCode", index, bank["SenderBankCode"].ToString());
                    this.grData.DataTable.SetValue("SenderAccount", index, bank["Account"].ToString());
                    this.grData.DataTable.SetValue("SenderAccountName", index, bank["Name"].ToString());
                }
                //this.grData.
                this.Freeze(false);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage($"Error ", UIHelper.MsgType.StatusBar, true);
            }
        }

        private void btnRem_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            try
            {

            }
            catch (Exception ex)
            { }
            this.Freeze(false);
        }

        private CheckBox ckbGr;

        private void CheckBox0_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (IsGroup != this.ckbGr.Checked)
            {
                this.Freeze(true);
                //GroupData();

                IsGroup = this.ckbGr.Checked;
                this.Freeze(false);
            }
        }

        private void cbbFeeT_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            //AutoFillData();
            //if (_PaymentMethod == PaymentMethod.CashBank)
            //{
            //    this.cbbBankAccount.Item.Enabled = true;
            //    this.cbbAccCa.Item.Enabled = true;
            //}
            //else if (_PaymentMethod == PaymentMethod.Cash)
            //{

            //    this.cbbBankAccount.Item.Enabled = false;
            //    this.cbbAccCa.Item.Enabled = true;
            //}
            //else
            //{

            //    this.cbbBankAccount.Item.Enabled = true;
            //    this.cbbAccCa.Item.Enabled = false;
            //}
            //ChangePaymentMethod();
        }

        private void btnFindL_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

            if (string.IsNullOrEmpty(FromDateReport) || string.IsNullOrEmpty(ToDateReport))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_DateSelectNull, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }

            if (!StringUtils.CheckFromDateEarlyToDate(FromDateReport, ToDateReport))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_FromDateEarlyToDate, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }

            try
            {
                LoadDataGridReport();
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndLoadData);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);

        }

        private void grHdr_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            this.Freeze(true);
            if (pVal.Row < 0)
            {
                this.Freeze(false);
                return;
            }

            try
            {
                var docnum = this.grHdr.DataTable.GetValue("DocNum", this.grHdr.GetDataTableRowIndex(pVal.Row)).ToString();
                if (!string.IsNullOrEmpty(docnum))
                {
                    LoadDataGridReportDetail(docnum);
                    ViewHelper.ColorGridRows(this.grHdr, pVal.Row);
                }
            }
            catch (Exception ex)
            {
                if (this.grDt != null)
                {
                    this.grDt.DataTable.Clear();
                }
            }
            this.Freeze(false);
        }
        private EditText txtRate;
        private CheckBox ckAppRate;

        private void ckAppRate_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            //this.txtRate.Item.Enabled = this.ckAppRate.Checked;
            //if(this.grData != null && this.grData.DataTable != null)
            //{
            //    this.grData.Columns.Item("DocRate").Editable = !this.ckAppRate.Checked;
            //}
        }

        private ComboBox cbbAccCa;
        private StaticText lblAccCa;
        private Button btnChAl;
        private Button btnUnck;

        private void btnChAl_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            //UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

            try
            {
                for (var i = 0; i < this.grData.DataTable.Rows.Count; i++)
                {
                    var sapstatus = this.grData.GetValueCustom("SAPStatus", i);
                    if (sapstatus == "01")
                    {
                        if (!this.SelectedDataIndexs.Contains(i))
                        {
                            this.grData.DataTable.SetValue("Check", i, "Y");
                            this.SelectedDataIndexs.Add(i);
                        }
                    }
                }
                EnableButton();
                //this.SelectedDataIndexs = this.grData.DataTable.Rows.Count;

            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private void btnUnck_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            // UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

            try
            {
                for (var i = 0; i < this.grData.DataTable.Rows.Count; i++)
                {

                    if (this.SelectedDataIndexs.Contains(i))
                    {
                        this.grData.DataTable.SetValue("Check", i, "N");
                        this.SelectedDataIndexs.Remove(i);
                    }
                }
                EnableButton();
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private StaticText lblPaymentType;
        private EditText txtNote;
        private StaticText lblNote;
        private StaticText lblCusVen;
        private EditText txtCusVen;

        private string oldVal = string.Empty;

        private Button btnFilter;

        private void btnFilter_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                //var type = _PaymentDocumentType == PaymentDocumentType.PT ? "C" : "S";
                //if (type != CardTypeSelect)
                //{
                //    CardTypeSelect = type;
                //    CardCodeFilter = string.Empty;
                //}
                //if (!frmObjectFilter.IsFormOpen)
                //{
                //    frmObjectFilter.Instance.OnSelectData += Instance_OnSelectData;
                //    frmObjectFilter.Instance.ShowForm(CardCodeFilter, _PaymentDocumentType == PaymentDocumentType.PT ? "C" : "S", "Đề xuất Thanh toán");
                //}
                //else
                //{
                //    UIHelper.LogMessage($"Bộ lọc đang mở cho màn hình {frmObjectFilter.Instance.BaseForm}", UIHelper.MsgType.StatusBar);
                //}
            }
            catch (Exception ex)
            { }
        }

        private void Instance_OnSelectData(object sender, SelectionEventArgs e)
        {
            //var data = e.Selected;
            //((frmObjectFilter)sender).Close();
            //if (data != null)
            //{
            //    CardCodeFilter = data.ToString();
            //    CardTypeSelect = e.Type;
            //}
            //else
            //{
            //    CardCodeFilter = string.Empty;
            //    CardTypeSelect = string.Empty;
            //}
        }

        private Button btnHis;
        private Button btnUpd;

        private void btnUpd_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            //var count = 0;
            List<int> listUpdate = new List<int>();
            for (var i = 0; i < this.grData.DataTable.Rows.Count; i++)
            {
                var sapStatus = this.grData.DataTable.GetValue("SAPStatus", i).ToString();
                var bankStatus = this.grData.DataTable.GetValue("BankStatus", i).ToString();

                if (sapStatus == "02" && bankStatus != "05")
                {
                    listUpdate.Add(i);
                }
            }

            if (listUpdate.Count <= 0)
            {
                UIHelper.LogMessage("Chưa có đơn được gửi sang ngân hàng", UIHelper.MsgType.StatusBar);
                this.Freeze(false);
                return;
            }

            UpdateBanksStatus(listUpdate);

            this.Freeze(false);
        }

        private void UpdateBanksStatus(List<int> data)
        {
            foreach (var index in data)
            {
                var status = string.Empty;
                var requestID = string.Empty;
                var date = string.Empty;
                UpdateBankStatus(index, ref status, ref requestID, ref date);
                var message = string.Empty;
                var isReturn = false;
                // status = this.grData.DataTable.GetValue("BankStatus", index).ToString();
                //requestID = this.grData.DataTable.GetValue("requestId", index).ToString();
                if (status != "05")
                {
                    continue;
                }

                var dataPayment = DataInRow(index, ref isReturn, ref message);
                if (isReturn)
                {
                    UIHelper.LogMessage(message, UIHelper.MsgType.StatusBar, true);
                    return;
                }
                //var key = $"PM{DateTime.Now.ToString("yyyymmddhhmms")}";
                UIHelper.LogMessage("Bắt đầu tạo thanh toán trên SAP", UIHelper.MsgType.StatusBar, false);
                var ret = PaymentViaDI.CreatePayment(dataPayment, requestID, date, ref message);

                UIHelper.LogMessage(ret.Message, UIHelper.MsgType.StatusBar, !ret.Flag);

            }
            LoadDataGridCreate();
        }

        private void UpdateBankStatus(int index, ref string status, ref string requestID, ref string receiveDate)
        {
            try
            {
                status = "00";
                // requestID = "";
                //status = this.grData.DataTable.GetValue("BankStatus", index).ToString();
                requestID = this.grData.DataTable.GetValue("requestId", index).ToString();
                var order = this.grData.DataTable.GetValue("DocNum", index).ToString();
                //if (order == "239")
                //{

                //}

                UIHelper.LogMessage("Bắt đầu cập nhật trạng thái thanh toán", UIHelper.MsgType.StatusBar, false);
                TransferInqRequest request = new TransferInqRequest
                {
                    requestId = this.grData.DataTable.GetValue("requestId", index).ToString(), //field id mapping với api transfer
                    transId = this.grData.DataTable.GetValue("transId", index).ToString(), //field mã giao dịch mapping với api transfer
                    merchantId = APIVietinBankConstrant.MerchantId,// ConfigurationManager.AppSettings["MerchantId"],
                    providerId = APIVietinBankConstrant.ProviderId,// ConfigurationManager.AppSettings["ProviderId"],
                    clientIP = APIVietinBankConstrant.ClientIP,//ConfigurationManager.AppSettings["ClientIP"],
                    transTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    channel = "MOBILE",
                    version = "1.0",
                    language = "vi",
                    fromDate = "01/01/" + DateTime.Now.Year.ToString(),
                    toDate = "31/12/" + DateTime.Now.Year.ToString(),
                    signature = ""
                };

                request.signature += (
                    request.requestId +
                    request.providerId +
                    request.merchantId +
                    request.transTime +
                    request.channel +
                    request.version +
                    request.clientIP +
                    request.language
                );
                var path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";

                //request.signature = FPT.SHA256_RSA2048.Encrypt(request.signature, path);
                request.signature = FPT.SHA256_RSA2048.Encrypt(request.signature, path);
                var json = JsonSerializer.Serialize(request);


                var options = new RestClientOptions(APIVietinBankConstrant.APIVTB)
                {
                    MaxTimeout = -1,
                };

                var client = new RestClient(options);
                var request1 = new RestRequest(APIVietinBankConstrant.TransferInqVTB, Method.Post);
                request1.AddHeader("x-ibm-client-id", APIVietinBankConstrant.ClientID);// ConfigurationManager.AppSettings["ClientID"]);
                request1.AddHeader("x-ibm-client-secret", APIVietinBankConstrant.ClientSecret);// ConfigurationManager.AppSettings["ClientSecret"]);
                request1.AddHeader("Content-Type", "application/json");

                request1.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = client.Execute(request1);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    UIHelper.LogMessage($"Lỗi {response.ErrorMessage}", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var result = response.Content;

                var rps = JsonSerializer.Deserialize<VTResponse>(result);
                if (rps == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }

                if (rps.status.code == "0")
                {
                    UIHelper.LogMessage($"Lỗi {rps.status.message}", UIHelper.MsgType.StatusBar, true);

                    var query = "UPDATE \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_TransferRecord\" ";
                    query += "SET \"Message\" = '" + rps.status.message + "' ";
                    query += "WHERE \"requestId\" = '" + request.requestId + "' ";
                    query += "AND \"bank\" = 'VT' ";

                    var ret = dbProvider.ExecuteNonQuery(query);
                    return;
                }
                var dataResponse = JsonSerializer.Deserialize<TransferInq>(result);

                receiveDate = dataResponse.result[0].receivedDate;
                if (dataResponse != null && dataResponse.result != null)
                {
                    foreach (var item in dataResponse.result)
                    {
                        var query = "UPDATE \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Bank_TransferRecord\" ";
                        query += "SET \"bankStatus\" = '" + item.status + "' ,";
                        query += "\"Message\" = '" + item.message + "' ";
                        query += "WHERE \"requestId\" = '" + dataResponse.requestId + "' ";
                        query += "AND \"transId\" = '" + item.transId + "' ";
                        query += "AND \"bank\" = 'VT' ";

                        var ret = dbProvider.ExecuteNonQuery(query);
                        status = item.status;
                    }
                }

                UIHelper.LogMessage("Hoàn tất cập nhật thanh toán", UIHelper.MsgType.StatusBar, false);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage($"Lỗi {ex.Message}", UIHelper.MsgType.StatusBar, true);
            }
        }

        private ComboBox cbbPaymentType;

        private void cbbBank_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            LoadBankAccountCombobox();
        }
    }
}
