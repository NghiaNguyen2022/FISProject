using STD.DataReader;
using SAPCore;
using SAPCore.Config;
using SAPCore.Helper;
using SAPCore.SAP;
using STDApp.AccessSAP;
using STDApp.Models;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Application = SAPbouiCOM.Framework.Application;
using PN.SmartLib.Helper;

namespace STDApp.Payment
{
    [FormAttribute("STDApp.Payment.frmPaymentReview", "Payment/frmPaymentReview.b1f")]
    class frmPaymentReview : UserFormBase
    {
        private static frmPaymentReview instance;
        public bool IsGroup = false;
        private List<int> SelectedDataIndexs = new List<int>();
        public static bool IsFormOpen = false;
        private List<ChangeOfApprove> changeOfApproves = new List<ChangeOfApprove>();
        private SAPbouiCOM.DataTable DataTableCbb;

        private string CardCode
        {
            get
            {
                return UIHelper.GetTextboxValue(txtCusVen);
            }
        }

        ChooseFromList oCFL;
        Conditions oConCustomers = null;
        Conditions oConVendors = null;
        private void ConfigCFL()
        {
            ChooseFromListCollection oCFLs = null;
            oCFLs = this.UIAPIRawForm.ChooseFromLists;
            oCFL = oCFLs.Item("CFL_BP");
            oConCustomers = null;
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
                this.lblCusVen.Caption = STRING_CONTRANTS.Title_Customer;
            }
            else
            {
                this.lblCusVen.Caption = STRING_CONTRANTS.Title_Vendor;
            }
            SetCondition(type);
        }

        private string FromDateReport
        {
            get
            {
                return UIHelper.GetTextboxValue(txtFDateL);
                //if (txtFDateL == null || txtFDateL.Value == null || txtFDateL.Value == "")
                //{
                //    return "";
                //}

                //return txtFDateL.Value.ToString();
            }
        }

        private string ToDateReport
        {
            get
            {
                return UIHelper.GetTextboxValue(txtTDateL);
                //if (txtTDateL == null || txtTDateL.Value == null || txtTDateL.Value == "")
                //{
                //    return "";
                //}

                //return txtTDateL.Value.ToString();
            }
        }
        private string BranchReport
        {
            get
            {
                return UIHelper.GetComboValue(cbbBrL, "0");
                //if (cbbBrL == null || cbbBrL.Selected == null)
                //{
                //    return "0";
                //}

                //return cbbBrL.Selected.Value;
            }
        }
        private PaymentDocumentType _PaymentDocumentTypeReport
        {
            get
            {
                if (cbbPmTyL != null && cbbPmTyL.Selected != null)
                {
                    var type = cbbPmTyL.Selected.Value.GetEnumValueByDescription<PaymentDocumentType>();

                    return type;// PaymentDocumentType.UC;
                }

                return PaymentDocumentType.PT;
            }
        }

        private PaymentStatus _PaymentStatus
        {
            get
            {
                if (cbbStatus != null && cbbStatus.Selected != null)
                {
                    var stt = cbbStatus.Selected.Value.GetEnumValueByDescription<PaymentStatus>();

                    return stt;// PaymentDocumentType.UC;
                }

                return PaymentStatus.All;
            }
        }

        private frmPaymentReview()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.folderList = ((SAPbouiCOM.Folder)(this.GetItem("F_ListPm").Specific));
            this.lblFDateL = ((SAPbouiCOM.StaticText)(this.GetItem("lblFDateL").Specific));
            this.lblTDateL = ((SAPbouiCOM.StaticText)(this.GetItem("lblTDateL").Specific));
            this.txtFDateL = ((SAPbouiCOM.EditText)(this.GetItem("txtFDateL").Specific));
            this.txtTDateL = ((SAPbouiCOM.EditText)(this.GetItem("txtTDateL").Specific));
            this.btnFindL = ((SAPbouiCOM.Button)(this.GetItem("btnFindL").Specific));
            this.btnFindL.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFindL_ClickBefore);
            this.lblBrL = ((SAPbouiCOM.StaticText)(this.GetItem("lblBrL").Specific));
            this.cbbBrL = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBrL").Specific));
            this.lblPmTyL = ((SAPbouiCOM.StaticText)(this.GetItem("lblPmTyL").Specific));
            this.cbbPmTyL = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbPmTyL").Specific));
            this.grHdr = ((SAPbouiCOM.Grid)(this.GetItem("grHdr").Specific));
            this.grHdr.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.grHdr_ClickAfter);
            this.grHdr.ComboSelectBefore += new SAPbouiCOM._IGridEvents_ComboSelectBeforeEventHandler(this.grHdr_ComboSelectBefore);
            this.grHdr.ComboSelectAfter += new SAPbouiCOM._IGridEvents_ComboSelectAfterEventHandler(this.grHdr_ComboSelectAfter);
            this.grDt = ((SAPbouiCOM.Grid)(this.GetItem("grDt").Specific));
            this.lblStatus = ((SAPbouiCOM.StaticText)(this.GetItem("lblSta").Specific));
            this.cbbStatus = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbStatus").Specific));
            this.btnUpdate = ((SAPbouiCOM.Button)(this.GetItem("btnUpdate").Specific));
            this.btnApr = ((SAPbouiCOM.Button)(this.GetItem("btnApr").Specific));
            this.btnApr.ClickBefore += this.BtnApr_ClickBefore;
            this.btnCrea = ((SAPbouiCOM.Button)(this.GetItem("btnCrea").Specific));
            this.btnCrea.ClickBefore += this.BtnCrea_ClickBefore;
            this.btnChkAll = ((SAPbouiCOM.Button)(this.GetItem("btnChkAll").Specific));
            this.btnChkAll.ClickBefore += this.BtnChkAll_ClickBefore;
            this.btnUnChk = ((SAPbouiCOM.Button)(this.GetItem("btnUnChk").Specific));
            this.btnUnChk.ClickBefore += this.BtnUnChk_ClickBefore;
            this.lblCusVen = ((SAPbouiCOM.StaticText)(this.GetItem("lblCV").Specific));
            this.txtCusVen = ((SAPbouiCOM.EditText)(this.GetItem("txtCV").Specific));
            this.DataTableCbb = this.UIAPIRawForm.DataSources.DataTables.Item("DT_FT");
            this.btnFilter = ((SAPbouiCOM.Button)(this.GetItem("btnFilter").Specific));
            this.btnFilter.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFilter_ClickBefore);
            this.OnCustomInitialize();

        }

        private void BtnUnChk_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            // UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

            try
            {
                for (var i = 0; i < this.grHdr.DataTable.Rows.Count; i++)
                {

                    if (this.SelectedDataIndexs.Contains(i))
                    {
                        this.grHdr.DataTable.SetValue("Check", i, "N");
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

        private void BtnChkAll_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);

            try
            {
                for (var i = 0; i < this.grHdr.DataTable.Rows.Count; i++)
                {

                    if (!this.SelectedDataIndexs.Contains(i))
                    {
                        this.grHdr.DataTable.SetValue("Check", i, "Y");
                        this.SelectedDataIndexs.Add(i);
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

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseAfter += new SAPbouiCOM.Framework.FormBase.CloseAfterHandler(this.Form_CloseAfter);
            this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);
        }

        private void OnCustomInitialize()
        {

        }
        private void SetControlLocation()
        {
            var max = this.UIAPIRawForm.ClientHeight;
            var maxw = this.UIAPIRawForm.ClientWidth;
            SetLocationOfFolderListControl(max, maxw);
        }

        private void SetLocationOfFolderListControl(int max, int maxw)
        {
            this.folderList.Item.Width = 500;

            this.lblFDateL.Item.Top = this.folderList.Item.Top + CoreSetting.UF_VerMargin;
            this.lblFDateL.Item.Left = this.folderList.Item.Left + CoreSetting.UF_HorMargin;

            this.txtFDateL.Item.Top = this.lblFDateL.Item.Top;
            this.txtFDateL.Item.Left = this.lblFDateL.Item.Left + this.lblFDateL.Item.Width + 20;

            this.lblTDateL.Item.Top = this.lblFDateL.Item.Top;
            this.lblTDateL.Item.Left = this.txtFDateL.Item.Left + this.txtFDateL.Item.Width + 20;

            this.txtTDateL.Item.Top = this.lblFDateL.Item.Top;
            this.txtTDateL.Item.Left = this.lblTDateL.Item.Left + this.lblTDateL.Item.Width + 20;

            this.lblPmTyL.Item.Top = this.lblFDateL.Item.Top;
            this.lblPmTyL.Item.Left = this.txtTDateL.Item.Left + this.txtTDateL.Item.Width + 20;

            this.cbbPmTyL.Item.Top = this.lblFDateL.Item.Top;
            this.cbbPmTyL.Item.Left = this.lblPmTyL.Item.Left + this.lblPmTyL.Item.Width + 20;

            this.lblBrL.Item.Top = this.lblFDateL.Item.Top;
            this.lblBrL.Item.Left = this.cbbPmTyL.Item.Left + this.cbbPmTyL.Item.Width + 20;

            this.cbbBrL.Item.Top = this.lblFDateL.Item.Top;
            this.cbbBrL.Item.Left = this.lblBrL.Item.Left + this.lblBrL.Item.Width + 20;

            this.lblStatus.Item.Top = this.lblFDateL.Item.Top;
            this.lblStatus.Item.Left = this.cbbBrL.Item.Left + this.cbbBrL.Item.Width + 20;

            this.cbbStatus.Item.Top = this.lblFDateL.Item.Top;
            this.cbbStatus.Item.Left = this.lblStatus.Item.Left + this.lblStatus.Item.Width + 20;

            this.lblCusVen.Item.Left = this.cbbStatus.Item.Left + this.cbbStatus.Item.Width + 20;
            this.lblCusVen.Item.Top = this.lblFDateL.Item.Top;

            var labBottom = this.lblFDateL.Item.Top + this.lblFDateL.Item.Height;
            var bttTop = labBottom - this.btnFindL.Item.Height;

            this.btnFilter.Item.Left = this.lblCusVen.Item.Left + this.lblCusVen.Item.Width + 20;
            this.btnFilter.Item.Top = bttTop;// this.lblCusVen.Item.Top;

            //var labBottom = this.lblFDateL.Item.Top + this.lblFDateL.Item.Height;
            //var bttTop = labBottom - this.btnFindL.Item.Height;
            this.btnFindL.Item.Top = bttTop;
            this.btnFindL.Item.Left = this.btnFilter.Item.Left + this.btnFilter.Item.Width + 30;

            this.btnUpdate.Item.Left = this.lblFDateL.Item.Left;
            this.btnUpdate.Item.Top = lblFDateL.Item.Top + lblFDateL.Item.Height + 20;

            this.btnApr.Item.Top = this.btnUpdate.Item.Top;
            this.btnApr.Item.Left = this.lblFDateL.Item.Left;

            this.btnCrea.Item.Top = this.btnApr.Item.Top;
            this.btnCrea.Item.Left = btnApr.Item.Left + btnApr.Item.Width + 10;

            this.btnChkAll.Item.Top = this.btnApr.Item.Top;
            this.btnChkAll.Item.Left = btnCrea.Item.Left + btnCrea.Item.Width + 10;

            this.btnUnChk.Item.Top = this.btnApr.Item.Top;
            this.btnUnChk.Item.Left = btnChkAll.Item.Left + btnChkAll.Item.Width + 10;

            this.grHdr.Item.Left = this.lblFDateL.Item.Left;
            this.grHdr.Item.Width = maxw - grHdr.Item.Left - 20;
            this.grHdr.Item.Top = btnUpdate.Item.Top + btnUpdate.Item.Height + 20;
            var bodyHeight = max - grHdr.Item.Top - 20;
            var headeHeight = (bodyHeight - 10) / 2;
            this.grHdr.Item.Height = headeHeight;

            this.grDt.Item.Left = this.lblFDateL.Item.Left;
            this.grDt.Item.Width = maxw - grDt.Item.Left - 20;
            this.grDt.Item.Top = this.grHdr.Item.Top + this.grHdr.Item.Height + 10;
            this.grDt.Item.Height = max - grDt.Item.Top - 20;
        }



        private SAPbouiCOM.Folder folderList;
        private SAPbouiCOM.StaticText lblFDateL;
        private SAPbouiCOM.StaticText lblTDateL;
        private SAPbouiCOM.EditText txtFDateL;
        private SAPbouiCOM.EditText txtTDateL;
        private SAPbouiCOM.Button btnFindL;
        private SAPbouiCOM.StaticText lblBrL;
        private SAPbouiCOM.ComboBox cbbBrL;
        private SAPbouiCOM.StaticText lblPmTyL;
        private SAPbouiCOM.ComboBox cbbPmTyL;
        private SAPbouiCOM.Grid grHdr;
        private SAPbouiCOM.Grid grDt;
        private SAPbouiCOM.StaticText lblStatus;
        private SAPbouiCOM.ComboBox cbbStatus;
        private SAPbouiCOM.Button btnUpdate;
        private SAPbouiCOM.Button btnApr;
        private SAPbouiCOM.Button btnCrea;
        private SAPbouiCOM.Button btnChkAll;
        private SAPbouiCOM.Button btnUnChk;
        private StaticText lblCusVen;
        private EditText txtCusVen;


        public static void ShowForm()
        {
            if (instance == null)
            {
                instance = new frmPaymentReview();
                instance.InitControl();
                instance.Show();
                IsFormOpen = true;
            }
        }
        private void InitControl()
        {
            SetControlLocation();
            this.folderList.Select();

            LoadBranchestoCombobox();
            LoadPaymentTypeCombobox();
            LoadStatusCombobox();

            ConfigCFL();
            SelectType("V");

        }
        private void LoadBranchestoCombobox()
        {
            //ViewHelper.LoadBranchesToCombobox(cbbBrL);

            try
            {
                this.Freeze(true);
                UIHelper.LoadComboboxFromDataSource(cbbBrL, DataTableCbb, QueryString.BranchesLoad, "BPLId", "BPLName");
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage("Error GetBranch : " + ex.Message, UIHelper.MsgType.StatusBar, true);
            }
            finally
            {
                this.Freeze(false);
            }
        }

        private void LoadPaymentTypeCombobox()
        {
            UIHelper.ClearSelectValidValues(cbbPmTyL);
            // cbbPmTyL.ValidValues.Add("PT", "Phiếu Thu");
            cbbPmTyL.ValidValues.Add("PC", STRING_CONTRANTS.PaymentType_PC);
            cbbPmTyL.ValidValues.Add("UC", STRING_CONTRANTS.PaymentType_UC);

            UIHelper.ComboboxSelectDefault(cbbPmTyL);
        }

        private void LoadStatusCombobox()
        {
            UIHelper.ClearSelectValidValues(cbbStatus);
            cbbStatus.ValidValues.Add("P", STRING_CONTRANTS.PaymentStatus_Pending);
            cbbStatus.ValidValues.Add("V", STRING_CONTRANTS.PaymentStatus_Viewed);
            cbbStatus.ValidValues.Add("A", STRING_CONTRANTS.PaymentStatus_Approved);
            cbbStatus.ValidValues.Add("R", STRING_CONTRANTS.PaymentStatus_Rejected);
            cbbStatus.ValidValues.Add("G", STRING_CONTRANTS.PaymentStatus_Generated);
            cbbStatus.ValidValues.Add("-", STRING_CONTRANTS.PaymentStatus_All);

            UIHelper.ComboboxSelectDefault(cbbStatus, 0);
        }
        private void LoadDataGridReportDetail(string docnum)
        {
            if (this.grDt != null)
            {
                this.grDt.DataTable.Clear();
                this.grDt.DataTable.ExecuteQuery(string.Format(QueryString.LoadPaymentsReportDetail, docnum, _PaymentDocumentTypeReport.GetDescription(), BranchReport));

                var bpCaptionCode = _PaymentDocumentTypeReport == PaymentDocumentType.PT ? STRING_CONTRANTS.Title_CustomerCode : STRING_CONTRANTS.Title_VendorCode;

                this.grDt.Columns.Item("CardCode").TitleObject.Caption = bpCaptionCode;
                this.grDt.Columns.Item("CardCode").Editable = false;

                this.grDt.Columns.Item("CardName").Editable = false;
                this.grDt.Columns.Item("CardName").Visible = false;

                this.grDt.Columns.Item("Check").Visible = false;

                this.grDt.Columns.Item("DocNum").TitleObject.Caption = STRING_CONTRANTS.Title_DocNum;
                this.grDt.Columns.Item("DocNum").Editable = false;

                this.grDt.Columns.Item("InvCode").TitleObject.Caption = STRING_CONTRANTS.Title_InvCode;
                this.grDt.Columns.Item("InvCode").Editable = false;

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
                this.grDt.Columns.Item("MustPayAsCash").TitleObject.Caption = STRING_CONTRANTS.Title_PayAmount;
                this.grDt.Columns.Item("MustPayAsCash").Editable = false;
                //this.grDt.Columns.Item("MustPayAsBank").TitleObject.Caption = "Số tiền thanh toán (Bank)";
                this.grDt.Columns.Item("MustPayAsBank").Visible = false;

                this.grDt.Columns.Item("DocEntry").Visible = false;
                this.grDt.Columns.Item("Manual").Visible = false;
                SAPbouiCOM.EditTextColumn oCol1 = null;

                oCol1 = (SAPbouiCOM.EditTextColumn)this.grDt.Columns.Item("DocNum");
                if (_PaymentDocumentTypeReport == PaymentDocumentType.PT)
                {
                    oCol1.LinkedObjectType = SAPObjectType.oInvoices;
                }
                else
                {
                    oCol1.LinkedObjectType = SAPObjectType.oPurchaseInvoices;
                }
                this.grDt.AutoResizeColumns();
                this.grDt.AutoResizeColumns();
                this.grDt.AutoResizeColumns();
                ViewHelper.ColorGridRows(this.grDt, 0, true);
            }
        }

        private void LoadDataGridReport()
        {
            this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = CardCode;
            if (this.grHdr != null)
            {
                this.grHdr.DataTable.Clear();
                this.grHdr.DataTable.ExecuteQuery(string.Format(QueryString.LoadPaymentsReportWithStatus, FromDateReport, ToDateReport, _PaymentDocumentTypeReport.GetDescription(), BranchReport, _PaymentStatus.GetDescription(), CardCodeFilter));

                if (this.grHdr.DataTable.Rows.Count <= 0)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.NoData, UIHelper.MsgType.StatusBar, false);
                    return;
                }
                this.grHdr.Columns.Item("PaymentKey").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentKey;// "Mã thanh toán";
                this.grHdr.Columns.Item("PaymentKey").Editable = false;

                if (_PaymentStatus == PaymentStatus.Approved || _PaymentStatus == PaymentStatus.Pending)
                {
                    this.grHdr.Columns.Item("Check").TitleObject.Caption = STRING_CONTRANTS.Title_Choose;
                    this.grHdr.Columns.Item("Check").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                }
                else
                {
                    this.grHdr.Columns.Item("Check").Visible = false;
                }

                this.grHdr.Columns.Item("PaymentType").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentType;
                this.grHdr.Columns.Item("PaymentType").Editable = false;

                var bpCaptionCode = _PaymentDocumentTypeReport == PaymentDocumentType.PT ? STRING_CONTRANTS.Title_CustomerCode : STRING_CONTRANTS.Title_VendorCode;
                this.grHdr.Columns.Item("CardCode").TitleObject.Caption = bpCaptionCode;
                this.grHdr.Columns.Item("CardCode").Editable = false;

                this.grHdr.Columns.Item("DocNum").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocNum;
                this.grHdr.Columns.Item("DocNum").Editable = false;

                this.grHdr.Columns.Item("DocDate").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocDate;
                this.grHdr.Columns.Item("DocDate").Editable = false;

                this.grHdr.Columns.Item("DocCurr").TitleObject.Caption = STRING_CONTRANTS.Title_Currency;
                this.grHdr.Columns.Item("DocCurr").Editable = false;

                this.grHdr.Columns.Item("Cash").TitleObject.Caption = STRING_CONTRANTS.Title_MustPayAsCash;
                this.grHdr.Columns.Item("Cash").Editable = false;

                this.grHdr.Columns.Item("Bank").TitleObject.Caption = STRING_CONTRANTS.Title_MustPayAsBank;
                this.grHdr.Columns.Item("Bank").Editable = false;

                this.grHdr.Columns.Item("CreateName").TitleObject.Caption = STRING_CONTRANTS.Title_CreateBy;
                this.grHdr.Columns.Item("CreateName").Editable = false;

                this.grHdr.Columns.Item("StatusName").TitleObject.Caption = STRING_CONTRANTS.Title_Status;
                this.grHdr.Columns.Item("StatusName").Editable = false;

                this.grHdr.Columns.Item("CashAcct").TitleObject.Caption = STRING_CONTRANTS.Title_Account;
                this.grHdr.Columns.Item("CashAcct").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;

                this.grHdr.Columns.Item("BankCode").TitleObject.Caption = STRING_CONTRANTS.Title_Bank;
                this.grHdr.Columns.Item("BankCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;

                if (_PaymentStatus == PaymentStatus.Pending)
                {
                    this.grHdr.Columns.Item("BankCode").Editable = true;
                    this.grHdr.Columns.Item("CashAcct").Editable = true;
                }
                else
                {
                    this.grHdr.Columns.Item("BankCode").Editable = false;
                    this.grHdr.Columns.Item("CashAcct").Editable = false;
                }

                if (_PaymentDocumentTypeReport == PaymentDocumentType.PT)
                {
                    this.grHdr.Columns.Item("DocEntry").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocEntry;
                    this.grHdr.Columns.Item("DocEntry").Editable = false;

                    SAPbouiCOM.EditTextColumn oCol1 = null;
                    oCol1 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("DocEntry");
                    oCol1.LinkedObjectType = SAPObjectType.oIncomingPayments;
                }
                else
                {

                    this.grHdr.Columns.Item("DocEntry").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDraftEntry;
                    this.grHdr.Columns.Item("DocEntry").Editable = false;

                    this.grHdr.Columns.Item("PaymentEntry").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocEntry;
                    this.grHdr.Columns.Item("PaymentEntry").Editable = false;

                    SAPbouiCOM.EditTextColumn oCol1 = null;
                    oCol1 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("PaymentEntry");
                    oCol1.LinkedObjectType = SAPObjectType.oVendorPayments;

                    SAPbouiCOM.EditTextColumn oCol2 = null;
                    oCol2 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("DocEntry");
                    oCol2.LinkedObjectType = SAPObjectType.oPaymentsDrafts;
                }


                this.grHdr.Columns.Item("TrsfrAcct").TitleObject.Caption = STRING_CONTRANTS.Title_BankAccount;
                this.grHdr.Columns.Item("TrsfrAcct").Editable = false;



                var comboAcct = (SAPbouiCOM.ComboBoxColumn)this.grHdr.Columns.Item("CashAcct");

                comboAcct.DisplayType = BoComboDisplayType.cdt_Value;
                foreach (var data in ViewHelper.Accounts)
                {
                    comboAcct.ValidValues.Add(data.Code, data.Name);
                }

                var comboBank = (SAPbouiCOM.ComboBoxColumn)this.grHdr.Columns.Item("BankCode");
                comboBank.DisplayType = BoComboDisplayType.cdt_Description;
                foreach (var data in ViewHelper.Banks)
                {
                    comboBank.ValidValues.Add(data.Code, data.Name);
                }

                this.grHdr.Columns.Item("Status").Visible = false;

                this.grHdr.CollapseLevel = 1;
                this.grHdr.AutoResizeColumns();
                SAPbouiCOM.EditTextColumn oCol = null;
                oCol = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("CardCode");
                oCol.LinkedObjectType = SAPObjectType.oBusinessPartners;

                this.btnApr.Item.Enabled = false;
                this.btnCrea.Item.Enabled = false;
                this.btnUpdate.Item.Enabled = false;

                ViewHelper.ColorGridRows(this.grHdr, 0, true);
                var docnum = this.grHdr.DataTable.GetValue("DocNum", 0).ToString();
                if (!string.IsNullOrEmpty(docnum))
                {
                    LoadDataGridReportDetail(docnum);
                }

                SelectedDataIndexs.Clear();
            }
        }

        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            instance = null;
            IsFormOpen = false;
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);
            SetControlLocation();
            this.Freeze(false);
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


        private void BtnCrea_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            if (SelectedDataIndexs == null || SelectedDataIndexs.Count <= 0)
            {
                this.Freeze(false);
                return;
            }
            List<int> data = new List<int>();
            try
            {

                UIHelper.LogMessage(STRING_CONTRANTS.Notice_CreatePayment);
                var count = 0;
                for (var i = 0; i < SelectedDataIndexs.Count; i++)
                {
                    var index = SelectedDataIndexs[i];
                    var docnum = int.Parse(this.grHdr.DataTable.GetValue("DocEntry", index).ToString());
                    if (docnum >= 0)
                    {
                        data.Add(docnum);
                    }
                }
                var message = string.Empty;
                var ret = PaymentViaDI.ApprovePayment1(data, ApprovalStatus.Generate, string.Empty, ref message);

                var success = ret.Where(x => x.Flag).Count();
                var error = ret.Count - success;

                UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndCreatePayment);

                if (error > 0)
                {
                    var errMessage = string.Empty;
                    foreach (var data1 in ret.Where(x => !x.Flag))
                    {
                        errMessage += $"Tạo đơn thanh toán {data1.DocNum} lỗi {data1.Message} \n";
                    }
                    UIHelper.LogMessage(errMessage, UIHelper.MsgType.Msgbox);
                }
                UIHelper.LogMessage($"Tạo {success}/{ret.Count} đơn thanh toán, và {error}/{ret.Count} không thành công", UIHelper.MsgType.StatusBar);
                LoadDataGridReport();

            }
            catch (Exception ex)
            {
                //UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }


        private bool AprByIndex(int index, ref string message)
        {
            var docnum = int.Parse(this.grHdr.DataTable.GetValue("DocEntry", index).ToString());
            if (docnum <= 0)
            {
                message = "Lỗi không lấy được mã thanh toán";
                return false;
            }
            var change = changeOfApproves.Where(x => x.Index == index && x.IsChange).FirstOrDefault();

            if (change != null)
            {
                var cash = this.grHdr.DataTable.GetValue("Cash", change.Index).ToString();
                var bank = this.grHdr.DataTable.GetValue("Bank", change.Index).ToString();

                if (!string.IsNullOrEmpty(change.ChangeAccount))
                {
                    decimal cashpay = 0;
                    if (string.IsNullOrEmpty(cash) || !decimal.TryParse(cash, out cashpay) || cashpay == 0)
                    {
                        message = $"Đơn thanh toán {change.DocEntry} không dùng tiền mặt nên không đổi được tài khoản tiền mặt";
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(change.ChangeBank))
                {
                    decimal bankpay = 0;
                    if (string.IsNullOrEmpty(bank) || !decimal.TryParse(bank, out bankpay) || bankpay == 0)
                    {
                        message = $"Đơn thanh toán {change.DocEntry} không thanh toán qua ngân hàng nên không đổi được ngân hàng";
                        return false;
                    }
                }
                var update = PaymentViaDI.UpdateAccountPayment(change, ref message);
                // UIHelper.LogMessage(message, UIHelper.MsgType.StatusBar);                

                return update;
            }
            else
            {
                var update = PaymentViaDI.ReviewPayment(docnum, ref message);

                return update;
            }

            return false;
        }
        private void BtnApr_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            try
            {
                if (SelectedDataIndexs == null || SelectedDataIndexs.Count <= 0)
                {
                    this.Freeze(false);
                    return;
                }
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_UpdatePayment);
                var count = 0;
                var error = false;
                var message = string.Empty;

                for (var i = 0; i < SelectedDataIndexs.Count; i++)
                {
                    message = string.Empty;

                    var index = SelectedDataIndexs[i];
                    error = !AprByIndex(index, ref message);

                    UIHelper.LogMessage(message, UIHelper.MsgType.StatusBar, error);
                    if (!error)
                        count++;
                    else
                    {
                        UIHelper.LogMessage(message, UIHelper.MsgType.StatusBar, error);
                    }
                }
                var total = SelectedDataIndexs.Count;
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndUpdatePayment);
                Thread.Sleep(500);
                UIHelper.LogMessage($"Cập nhật thành công {count}/{total} đơn thanh toán, và {total - count}/{total} không thành công", UIHelper.MsgType.StatusBar);

                LoadDataGridReport();
            }
            catch (Exception ex)
            {
                //UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private void BtnUpdate_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            try
            {
                if (changeOfApproves == null || changeOfApproves.Count <= 0)
                {
                    this.Freeze(false);
                    return;
                }
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_UpdatePayment);
                var count = 0;

                var error = false;
                foreach (var change in changeOfApproves.Where(x => x.IsChange))
                {
                    var cash = this.grHdr.DataTable.GetValue("Cash", change.Index).ToString();
                    var bank = this.grHdr.DataTable.GetValue("Bank", change.Index).ToString();

                    if (!string.IsNullOrEmpty(change.ChangeAccount))
                    {
                        decimal cashpay = 0;
                        if (string.IsNullOrEmpty(cash) || !decimal.TryParse(cash, out cashpay) || cashpay == 0)
                        {
                            UIHelper.LogMessage($"Đơn thanh toán {change.DocEntry} không dùng tiền mặt nên không đổi được tài khoản tiền mặt", UIHelper.MsgType.Msgbox, true);
                            //this.Freeze(false);
                            //return;
                            error = true;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(change.ChangeBank))
                    {
                        decimal bankpay = 0;
                        if (string.IsNullOrEmpty(bank) || !decimal.TryParse(bank, out bankpay) || bankpay == 0)
                        {
                            UIHelper.LogMessage($"Đơn thanh toán {change.DocEntry} không thanh toán qua ngân hàng nên không đổi được ngân hàng", UIHelper.MsgType.Msgbox, true);

                            error = true;
                            break;
                        }
                    }
                    var message = string.Empty;
                    var update = PaymentViaDI.UpdateAccountPayment(change, ref message);
                    UIHelper.LogMessage(message, UIHelper.MsgType.StatusBar);

                    if (update)
                        count++;
                    else
                    {
                        UIHelper.LogMessage($"Cập nhật thanh toán {change.DocEntry} lỗi{message}", UIHelper.MsgType.Msgbox);
                    }
                }
                if (error)
                {
                    LoadDataGridReport();
                    this.Freeze(false);
                    return;
                }
                var total = changeOfApproves.Where(x => x.IsChange).ToList().Count;
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndUpdatePayment);
                Thread.Sleep(500);
                UIHelper.LogMessage($"Cập nhật thành công {count}/{total} đơn thanh toán, và {total - count}/{total} không thành công", UIHelper.MsgType.StatusBar);

                LoadDataGridReport();

            }
            catch (Exception ex)
            {
                //UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
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

            UIHelper.CheckInGrid(this.grHdr, pVal.Row, SelectedDataIndexs, "PaymentKey");

            try
            {
                var docnum = this.grHdr.DataTable.GetValue("DocNum", this.grHdr.GetDataTableRowIndex(pVal.Row)).ToString();
                if (!string.IsNullOrEmpty(docnum))
                {
                    LoadDataGridReportDetail(docnum);
                    ViewHelper.ColorGridRows(this.grHdr, pVal.Row);
                }

                //if (pVal.ColUID == "Check")
                //{
                //    var indexSelected = this.grHdr.GetDataTableRowIndex(pVal.Row);
                //    if (this.grHdr.DataTable.GetValue("Check", indexSelected).ToString() == "Y")
                //    {
                //        if (!SelectedDataIndexs.Contains(indexSelected))
                //            SelectedDataIndexs.Add(indexSelected);
                //    }
                //    else
                //    {
                //        if (SelectedDataIndexs.Contains(indexSelected))
                //            SelectedDataIndexs.Remove(indexSelected);
                //    }
                //}
                EnableButton();
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

        private void EnableButton()
        {
            if (SelectedDataIndexs.Count > 0)
            {
                this.btnUnChk.Item.Enabled = true;
                if (_PaymentStatus == PaymentStatus.Pending)
                {
                    this.btnApr.Item.Enabled = true;
                }
                else if (_PaymentStatus == PaymentStatus.Approved)
                {
                    this.btnCrea.Item.Enabled = true;
                }
            }
            else
            {
                this.btnApr.Item.Enabled = false;
                this.btnCrea.Item.Enabled = false;
                this.btnUnChk.Item.Enabled = false;
            }

            if (SelectedDataIndexs.Count == this.grHdr.DataTable.Rows.Count)
            {
                this.btnChkAll.Item.Enabled = false;
            }
            else
            {
                this.btnChkAll.Item.Enabled = true;
            }
        }

        private void grHdr_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            this.Freeze(true);
            try
            {
                if (pVal.ColUID == "CashAcct" || pVal.ColUID == "BankCode")
                {
                    var indexSelected = this.grHdr.GetDataTableRowIndex(pVal.Row);
                    var change = changeOfApproves.Where(x => x.Index == indexSelected).FirstOrDefault();
                    var isChange = false;
                    if (change != null)
                    {
                        if (pVal.ColUID == "CashAcct")
                        {
                            var old = this.grHdr.DataTable.GetValue("CashAcct", indexSelected).ToString();
                            if (change.OldAccount != old)
                            {
                                change.ChangeAccount = old;
                                isChange = true;
                            }
                        }
                        else if (pVal.ColUID == "BankCode")
                        {
                            var old = this.grHdr.DataTable.GetValue("BankCode", indexSelected).ToString();
                            if (change.OldBank != old)
                            {
                                change.ChangeBank = old;
                                isChange = true;
                            }
                        }
                    }

                    if (!isChange && change != null)
                    {
                        changeOfApproves.Remove(change);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            this.Freeze(false);
        }

        private void grHdr_ComboSelectBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            try
            {
                if (pVal.ColUID == "CashAcct" || pVal.ColUID == "BankCode")
                {
                    var indexSelected = this.grHdr.GetDataTableRowIndex(pVal.Row);
                    var change = changeOfApproves.Where(x => x.Index == indexSelected).FirstOrDefault();
                    if (change == null)
                    {
                        change = new ChangeOfApprove();
                        change.Index = indexSelected;
                        change.DocEntry = int.Parse(this.grHdr.DataTable.GetValue("DocEntry", indexSelected).ToString());
                        change.OldAccount = this.grHdr.DataTable.GetValue("CashAcct", indexSelected).ToString();
                        change.OldBank = this.grHdr.DataTable.GetValue("BankCode", indexSelected).ToString();
                        change.ChangeAccount = this.grHdr.DataTable.GetValue("CashAcct", indexSelected).ToString();
                        change.ChangeBank = this.grHdr.DataTable.GetValue("BankCode", indexSelected).ToString();
                        changeOfApproves.Add(change);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            this.Freeze(false);
        }

        private Button btnFilter;

        private void btnFilter_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (!frmObjectFilter.IsFormOpen)
                {
                    frmObjectFilter.Instance.OnSelectData += Instance_OnSelectData; ;
                    frmObjectFilter.Instance.ShowForm(CardCodeFilter, "S", "Thanh toán");
                }
                else
                {
                    UIHelper.LogMessage($"Bộ lọc đang mở cho màn hình {frmObjectFilter.Instance.BaseForm}", UIHelper.MsgType.StatusBar);
                }
            }
            catch (Exception ex)
            { }
        }

        private string CardCodeFilter { get; set; } = string.Empty;
        private void Instance_OnSelectData(object sender, SelectionEventArgs e)
        {
            var data = e.Selected;
            ((frmObjectFilter)sender).Close();
            if (data != null)
            {
                CardCodeFilter = data.ToString();
                // CardTypeSelect = e.Type;
            }
            else
            {
                CardCodeFilter = string.Empty;
                //CardTypeSelect = string.Empty;
            }
        }
    }
}
