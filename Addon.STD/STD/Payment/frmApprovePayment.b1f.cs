using STD.DataReader;
using SAPCore;
using SAPCore.Helper;
using SAPCore.SAP;
using STDApp.AccessSAP;
using STDApp.DataReader;
using STDApp.Models;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PN.SmartLib.Helper;

namespace STDApp.Payment
{
    [FormAttribute("STDApp.Payment.frmApprovePayment", "Payment/frmApprovePayment.b1f")]
    class frmApprovePayment : UserFormBase
    {
        private static frmApprovePayment instance;
        private List<int> SelectedDataIndexs = new List<int>();
        public static bool IsFormOpen = false;
        private bool IsProcess = false;
        private List<ChangeOfApprove> changeOfApproves = new List<ChangeOfApprove>();
        private frmApprovePayment()
        {
        }
        private string FromDate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtFDate);
                //if (txtFDate == null || txtFDate.Value == null || txtFDate.Value == "")
                //    return "";
                //return txtFDate.Value.ToString();
            }
        }
        private string ToDate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtTDate);
                //if (txtTDate == null || txtTDate.Value == null || txtTDate.Value == "")
                //    return "";
                //return txtTDate.Value.ToString();
            }
        }

        private string Branch
        {
            get
            {
                return UIHelper.GetComboValue(cbbBr, "0");
                //if (cbbBr == null || cbbBr.Selected == null)
                //    return "0";
                //return cbbBr.Selected.Value;
            }
        }

        private ApprovalStatus approvalStatus
        {
            get
            {
                var val = btnList.Selected.Value;
                if (val == "Y")
                {
                    return ApprovalStatus.Approve;
                }
                else if (val == "N")
                {
                    return ApprovalStatus.Reject;
                }
                else if (val == "P")
                {
                    return ApprovalStatus.Generate;
                }
                else if (val == "W")
                {
                    return ApprovalStatus.Pending;
                }
                return ApprovalStatus.NoAction;
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
        //private string PaymentType
        //{
        //    get
        //    {
        //        if (cbbBr == null || cbbBr.Selected == null)
        //            return "0";
        //        return cbbBr.Selected.Value;
        //    }
        //}
        private PaymentDocumentType _PaymentDocumentType
        {
            get
            {
                if (cbbPmTyL != null && cbbPmTyL.Selected != null)
                {
                    var type = cbbPmTyL.Selected.Value.GetEnumValueByDescription<PaymentDocumentType>();
                    //if (cbbPmTyL.Selected.Value == "PT")
                    //{
                    //    return PaymentDocumentType.PT;
                    //}
                    //else if (cbbPmTyL.Selected.Value == "PC")
                    //{
                    //    return PaymentDocumentType.PC;
                    //}

                    return type;// PaymentDocumentType.UC;
                }

                return PaymentDocumentType.PT;
            }
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblFDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblFDate").Specific));
            this.lblTDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblTDate").Specific));
            this.txtFDate = ((SAPbouiCOM.EditText)(this.GetItem("txtFDate").Specific));
            this.txtTDate = ((SAPbouiCOM.EditText)(this.GetItem("txtTDate").Specific));
            this.btnFind = ((SAPbouiCOM.Button)(this.GetItem("btnFind").Specific));
            this.btnFind.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFind_ClickBefore);
            this.lblBr = ((SAPbouiCOM.StaticText)(this.GetItem("lblBr").Specific));
            this.cbbBr = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBr").Specific));
            this.lblPmTyp = ((SAPbouiCOM.StaticText)(this.GetItem("lblPmTyp").Specific));
            this.cbbPmTyL = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbPmTyL").Specific));
            this.cbbPmTyL.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbPmTyL_ComboSelectAfter);
            this.lblNote = ((SAPbouiCOM.StaticText)(this.GetItem("lblNote").Specific));
            this.txtNote = ((SAPbouiCOM.EditText)(this.GetItem("txtNote").Specific));
            this.btnList = ((SAPbouiCOM.ButtonCombo)(this.GetItem("btnList").Specific));
            this.btnList.ClickAfter += new SAPbouiCOM._IButtonComboEvents_ClickAfterEventHandler(this.btnList_ClickAfter);
            this.btnList.ComboSelectAfter += new SAPbouiCOM._IButtonComboEvents_ComboSelectAfterEventHandler(this.btnList_ComboSelectAfter);
            this.grHdr = ((SAPbouiCOM.Grid)(this.GetItem("grHdr").Specific));
            //   this.grHdr.ComboSelectBefore += new SAPbouiCOM._IGridEvents_ComboSelectBeforeEventHandler(this.grHdr_ComboSelectBefore);
            //   this.grHdr.ComboSelectAfter += new SAPbouiCOM._IGridEvents_ComboSelectAfterEventHandler(this.grHdr_ComboSelectAfter);
            this.grHdr.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.grHdr_ClickAfter);
            this.grDt = ((SAPbouiCOM.Grid)(this.GetItem("grDt").Specific));
            this.grDt.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.grDt_ClickAfter);
            //   this.btnUpdate.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnUpdate_ClickBefore);
            this.lblStt = ((SAPbouiCOM.StaticText)(this.GetItem("lblStt").Specific));
            this.cbbStatus = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbStt").Specific));
            this.cbbStatus.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbStatus_ComboSelectAfter);
            this.btnchkAll = ((SAPbouiCOM.Button)(this.GetItem("btnchkAll").Specific));
            this.btnchkAll.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnchkAll_ClickBefore);
            this.btnUnchk = ((SAPbouiCOM.Button)(this.GetItem("btnUnchk").Specific));
            this.btnUnchk.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnUnchk_ClickBefore);
            this.OnCustomInitialize();

        }

        private void SetControlLocation()
        {
            var max = this.UIAPIRawForm.ClientHeight;
            var maxw = this.UIAPIRawForm.ClientWidth;

            this.lblFDate.Item.Top = 20;
            this.lblFDate.Item.Left = 10;

            this.txtFDate.Item.Top = this.lblFDate.Item.Top;
            this.txtFDate.Item.Left = this.lblFDate.Item.Left + this.lblFDate.Item.Width + 20;

            this.lblTDate.Item.Top = this.lblFDate.Item.Top;
            this.lblTDate.Item.Left = this.txtFDate.Item.Left + this.txtFDate.Item.Width + 20;

            this.txtTDate.Item.Top = this.lblFDate.Item.Top;
            this.txtTDate.Item.Left = this.lblTDate.Item.Left + this.lblTDate.Item.Width + 20;

            this.lblPmTyp.Item.Top = this.lblFDate.Item.Top;
            this.lblPmTyp.Item.Left = this.txtTDate.Item.Left + this.txtTDate.Item.Width + 20;

            this.cbbPmTyL.Item.Top = this.lblFDate.Item.Top;
            this.cbbPmTyL.Item.Left = this.lblPmTyp.Item.Left + this.lblPmTyp.Item.Width + 20;

            this.lblBr.Item.Top = this.lblFDate.Item.Top;
            this.lblBr.Item.Left = this.cbbPmTyL.Item.Left + this.cbbPmTyL.Item.Width + 20;

            this.cbbBr.Item.Top = this.lblFDate.Item.Top;
            this.cbbBr.Item.Left = this.lblBr.Item.Left + this.lblBr.Item.Width + 20;


            this.lblStt.Item.Top = this.lblFDate.Item.Top;
            this.lblStt.Item.Left = this.cbbBr.Item.Left + this.cbbBr.Item.Width + 20;

            this.cbbStatus.Item.Top = this.lblFDate.Item.Top;
            this.cbbStatus.Item.Left = this.lblStt.Item.Left + this.lblStt.Item.Width + 20;

            var labBottom = this.lblFDate.Item.Top + this.lblFDate.Item.Height;
            var bttTop = labBottom - this.btnFind.Item.Height;
            this.btnFind.Item.Top = bttTop;
            this.btnFind.Item.Left = this.cbbStatus.Item.Left + this.cbbStatus.Item.Width + 30;

            this.lblNote.Item.Left = this.lblFDate.Item.Left;
            this.txtNote.Item.Left = this.lblNote.Item.Left + this.lblNote.Item.Width + 20;
            this.txtNote.Item.Top = lblFDate.Item.Top + lblFDate.Item.Height + 10;

            var txtNoteBottom = this.txtNote.Item.Top + this.txtNote.Item.Height;
            var lblHeight = this.lblNote.Item.Height;
            this.lblNote.Item.Top = txtNoteBottom - lblHeight;
            var btnHeight = this.btnFind.Item.Height;

            var btnFindRight = this.btnFind.Item.Left + this.btnFind.Item.Width;
            //this.btnUpdate.Item.Top = txtNoteBottom - btnHeight;
            //this.btnUpdate.Item.Left = btnFindRight - this.btnUpdate.Item.Width;

            this.btnList.Item.Top = txtNoteBottom - btnHeight;
            this.btnList.Item.Width = this.btnFind.Item.Width;
            this.btnList.Item.Left = lblStt.Item.Left;

            this.btnchkAll.Item.Left = this.lblFDate.Item.Left;
            this.btnchkAll.Item.Top = txtNote.Item.Top + txtNote.Item.Height + 20;

            this.btnUnchk.Item.Top = this.btnchkAll.Item.Top;
            this.btnUnchk.Item.Left = this.btnchkAll.Item.Left + this.btnchkAll.Item.Width + 20;

            this.grHdr.Item.Left = this.lblFDate.Item.Left;
            this.grHdr.Item.Width = maxw - grHdr.Item.Left - 20;
            this.grHdr.Item.Top = btnUnchk.Item.Top + btnUnchk.Item.Height + 20;
            var bodyHeight = max - grHdr.Item.Top - 20;
            var headeHeight = (bodyHeight - 10) / 2;
            this.grHdr.Item.Height = headeHeight;

            this.grDt.Item.Left = this.lblFDate.Item.Left;
            this.grDt.Item.Width = maxw - grDt.Item.Left - 20;
            this.grDt.Item.Top = this.grHdr.Item.Top + this.grHdr.Item.Height + 10;
            this.grDt.Item.Height = max - grDt.Item.Top - 20;
        }

        private void InitControl()
        {
            SetControlLocation();

            ViewHelper.LoadBranchesToCombobox(cbbBr);
            LoadPaymentTypeCombobox();
            if (_PaymentDocumentType == PaymentDocumentType.PT)
            {
                this.txtNote.Item.Enabled = false;
                this.btnList.Item.Visible = false;

                UIHelper.ClearSelectValidValues(cbbStatus);
                this.cbbStatus.Item.Enabled = false;
            }
            else
            {
                this.txtNote.Item.Enabled = true;
                this.btnList.Item.Visible = true;
                LoadComboboxAction();

                this.cbbStatus.Item.Enabled = true;
                LoadStatusCombobox();
            }

        }
        private void LoadPaymentTypeCombobox()
        {
            UIHelper.ClearSelectValidValues(cbbPmTyL);
            cbbPmTyL.ValidValues.Add("PT", STRING_CONTRANTS.PaymentType_PT);
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

        private void LoadComboboxAction()
        {
            var status = cbbStatus.Value;
            if (status == "V" || status == "R" || status == "A")
            {
                for (var i = btnList.ValidValues.Count - 1; i >= 0; i--)
                {
                    btnList.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
                }

                if (status == "V")
                {
                    btnList.ValidValues.Add("Y", "Phê duyệt");
                    btnList.ValidValues.Add("N", "Từ chối");
                }
                else
                {
                    btnList.ValidValues.Add("W", "Đề xuất thanh toán");
                }
                this.btnList.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription;
                this.btnList.Item.DisplayDesc = true;

                this.btnList.Caption = "Lựa chọn";
                this.btnList.Item.Enabled = true;
            }
            else
            {
                this.btnList.Caption = "Lựa chọn";
                this.btnList.Item.Enabled = false;
            }

            //btnList.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
        }

        public static void ShowForm()
        {
            if (instance == null)
            {
                instance = new frmApprovePayment();
                instance.InitControl();
                instance.Show();
                IsFormOpen = true;
            }
        }
        private void LoadDataGridReport()
        {
            if (this.grHdr != null)
            {
                if (_PaymentDocumentType == PaymentDocumentType.PT)
                {
                    UIHelper.LogMessage("Phiếu thu không cần quá trình phê duyệt, dữ liệu chỉ để chế độ xem", UIHelper.MsgType.StatusBar);
                }

                this.grHdr.DataTable.Clear();
                this.grHdr.DataTable.ExecuteQuery(string.Format(QueryString.LoadPaymentsReportWithStatus, FromDate, ToDate, _PaymentDocumentType.GetDescription(), Branch, _PaymentStatus.GetDescription(), ""));

                if (this.grHdr.DataTable.Rows.Count <= 0)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.NoData, UIHelper.MsgType.StatusBar, false);
                    return;
                }
                if (_PaymentDocumentType != PaymentDocumentType.PT &&
                    (_PaymentStatus == PaymentStatus.Reviewed || _PaymentStatus == PaymentStatus.Approved || _PaymentStatus == PaymentStatus.Rejected))
                {
                    this.grHdr.Columns.Item("Check").TitleObject.Caption = STRING_CONTRANTS.Title_Choose;
                    this.grHdr.Columns.Item("Check").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                    this.grHdr.Columns.Item("Check").Editable = true;
                }
                else
                {
                    this.grHdr.Columns.Item("Check").Visible = false;
                }

                this.grHdr.Columns.Item("PaymentKey").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentKey;
                this.grHdr.Columns.Item("PaymentKey").Editable = false;

                this.grHdr.Columns.Item("PaymentType").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentType;
                this.grHdr.Columns.Item("PaymentType").Editable = false;

                var bpCaptionCode = _PaymentDocumentType == PaymentDocumentType.PT ? STRING_CONTRANTS.Title_CustomerCode : STRING_CONTRANTS.Title_VendorCode;
                this.grHdr.Columns.Item("CardCode").TitleObject.Caption = bpCaptionCode;
                this.grHdr.Columns.Item("CardCode").Editable = false;

                //this.grHdr.Columns.Item("DocEntry").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocEntry;
                //this.grHdr.Columns.Item("DocEntry").Editable = false;

                this.grHdr.Columns.Item("DocNum").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocNum;
                this.grHdr.Columns.Item("DocNum").Editable = false;

                this.grHdr.Columns.Item("DocDate").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocDate;
                this.grHdr.Columns.Item("DocDate").Editable = false;

                this.grHdr.Columns.Item("DocCurr").TitleObject.Caption = STRING_CONTRANTS.Title_Currency;
                this.grHdr.Columns.Item("DocCurr").Editable = false;

                this.grHdr.Columns.Item("Cash").TitleObject.Caption = STRING_CONTRANTS.Title_MustPayAsCash;
                this.grHdr.Columns.Item("Cash").Editable = false;

                if (this._PaymentDocumentType != PaymentDocumentType.PT)
                {
                    this.grHdr.Columns.Item("CashAcct").Visible = false;
                    this.grHdr.Columns.Item("BankCode").Visible = false;
                    this.grHdr.Columns.Item("TrsfrAcct").Visible = false;

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
                else
                {

                    this.grHdr.Columns.Item("DocEntry").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocEntry;
                    this.grHdr.Columns.Item("DocEntry").Editable = false;

                    SAPbouiCOM.EditTextColumn oCol1 = null;
                    oCol1 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("DocEntry");
                    oCol1.LinkedObjectType = SAPObjectType.oIncomingPayments;

                }

                this.grHdr.Columns.Item("Bank").TitleObject.Caption = STRING_CONTRANTS.Title_MustPayAsBank;
                this.grHdr.Columns.Item("Bank").Editable = false;

                this.grHdr.Columns.Item("CreateName").TitleObject.Caption = STRING_CONTRANTS.Title_CreateBy;
                this.grHdr.Columns.Item("CreateName").Editable = false;


                this.grHdr.Columns.Item("StatusName").TitleObject.Caption = STRING_CONTRANTS.Title_Status;
                this.grHdr.Columns.Item("StatusName").Editable = false;


                this.grHdr.Columns.Item("Status").Visible = false;

                this.grHdr.CollapseLevel = 1;
                this.grHdr.AutoResizeColumns();

                SAPbouiCOM.EditTextColumn oCol = null;
                oCol = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("CardCode");
                oCol.LinkedObjectType = SAPObjectType.oBusinessPartners;

                //SAPbouiCOM.EditTextColumn oCol1 = null;
                //oCol1 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("DocEntry");
                //if (_PaymentDocumentType == PaymentDocumentType.PT)
                //{
                //    oCol1.LinkedObjectType = SAPObjectType.oIncomingPayments;
                //}
                //else
                //{
                //    oCol1.LinkedObjectType = SAPObjectType.oVendorPayments;
                //}

                ViewHelper.ColorGridRows(this.grHdr, 0, true);
                var docnum = this.grHdr.DataTable.GetValue("DocNum", 0).ToString();
                if (!string.IsNullOrEmpty(docnum))
                {
                    LoadDataGridReportDetail(docnum);
                }
            }
        }
        private void LoadDataGridReportDetail(string docnum)
        {
            if (this.grDt != null)
            {
                this.grDt.DataTable.Clear();
                this.grDt.DataTable.ExecuteQuery(string.Format(QueryString.LoadPaymentsReportDetail, docnum, _PaymentDocumentType.GetDescription(), Branch));

                var bpCaptionCode = _PaymentDocumentType == PaymentDocumentType.PT ? STRING_CONTRANTS.Title_CustomerCode : STRING_CONTRANTS.Title_VendorCode;

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

                SAPbouiCOM.EditTextColumn oCol2 = null;
                oCol2 = (SAPbouiCOM.EditTextColumn)this.grDt.Columns.Item("CardCode");
                oCol2.LinkedObjectType = SAPObjectType.oBusinessPartners;

                SAPbouiCOM.EditTextColumn oCol1 = null;

                oCol1 = (SAPbouiCOM.EditTextColumn)this.grDt.Columns.Item("DocNum");
                if (_PaymentDocumentType == PaymentDocumentType.PT)
                {
                    oCol1.LinkedObjectType = SAPObjectType.oInvoices;
                }
                else
                {
                    oCol1.LinkedObjectType = SAPObjectType.oPurchaseInvoices;
                }
                this.grDt.AutoResizeColumns();
                ViewHelper.ColorGridRows(this.grDt, 0, true);
            }
        }

        private void HandleApproveAction()
        {
            try

            {
                if (approvalStatus == ApprovalStatus.NoAction)
                {
                    return;
                }
                var canHandle = true;
                var message = string.Empty;
                List<int> data = new List<int>();

                var notice = string.Empty;
                if (approvalStatus == ApprovalStatus.Generate)
                    notice = "Tạo";
                else if (approvalStatus == ApprovalStatus.Approve)
                    notice = "Duyệt";
                else if (approvalStatus == ApprovalStatus.Reject)
                    notice = "Từ chối";
                else
                    notice = "Chuyển về Đề xuất thanh toán";
                UIHelper.LogMessage($"Bắt đầu {notice}", UIHelper.MsgType.StatusBar);

                for (var i = 0; i < SelectedDataIndexs.Count; i++)
                {
                    var index = SelectedDataIndexs[i];
                    var status = this.grHdr.DataTable.GetValue("Status", index).ToString();
                    var StatusName = this.grHdr.DataTable.GetValue("StatusName", index).ToString();
                    var key = this.grHdr.DataTable.GetValue("DocEntry", index).ToString();
                    //var docnum = this.grHdr.DataTable.GetValue("DocNum", index).ToString();

                    var docentry = 0;
                    docentry = int.Parse(key);
                    data.Add(docentry);
                    if (approvalStatus == ApprovalStatus.Approve && status != "Reviewed")
                    {
                        canHandle = false;
                        message = $"Phiếu {key} ở trạng thái [{StatusName}] không thể được duyệt. Chỉ duyệt các phiếu ở trạng thái [Đề nghị thanh toán]";
                        break;
                    }
                    if (approvalStatus == ApprovalStatus.Reject && status != "Reviewed")
                    {
                        canHandle = false;
                        message = $"Phiếu {key} ở trạng thái [{StatusName}] không thể được từ chối. Chỉ từ chối các phiếu ở trạng thái [Đề nghị thanh toán]";
                        break;
                    }
                    if (approvalStatus == ApprovalStatus.Pending &&
                        (status != "Approved" && status != "Rejected" && status != "Reviewed"))
                    {
                        canHandle = false;
                        message = $"Phiếu {key} ở trạng thái [{StatusName}] không thể chuyển về [Yêu cầu thanh toán]. Chỉ thao tác các phiếu ở trạng thái [Đã duyệt], [Từ chối], [Đề nghị thanh toán]";
                        break;
                    }
                }
                if (!canHandle)
                {
                    UIHelper.LogMessage(message, UIHelper.MsgType.StatusBar);
                    return;
                }
                var remark = this.txtNote.Value;
                var ret = PaymentViaDI.ApprovePayment1(data, approvalStatus, remark, ref message);

                var success = ret.Where(x => x.Flag).Count();
                var error = ret.Count - success;

                UIHelper.LogMessage($"{notice} {success}/{ret.Count} đơn thanh toán, và {error}/{ret.Count} không thành công", UIHelper.MsgType.StatusBar);

                if (error > 0)
                {
                    var errMessage = string.Empty;
                    foreach (var data1 in ret.Where(x => !x.Flag))
                    {
                        errMessage += $"{notice} đơn thanh toán {data1.DocNum} lỗi {data1.Message} \n";
                    }
                    UIHelper.LogMessage(errMessage, UIHelper.MsgType.Msgbox);
                }
                else
                {
                    //Thread.Sleep(1000);
                    LoadDataGridReport();
                }
            }
            catch
            {

            }
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

        private void btnList_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);

            HandleApproveAction();
            IsProcess = true;
            LoadComboboxAction();
            this.Freeze(false);
        }

        private SAPbouiCOM.StaticText lblTDate;
        private SAPbouiCOM.EditText txtFDate;
        private SAPbouiCOM.EditText txtTDate;
        private SAPbouiCOM.Button btnFind;
        private SAPbouiCOM.StaticText lblBr;
        private SAPbouiCOM.ComboBox cbbBr;
        private SAPbouiCOM.StaticText lblPmTyp;
        private SAPbouiCOM.ComboBox cbbPmTyL;
        private SAPbouiCOM.StaticText lblNote;
        private SAPbouiCOM.EditText txtNote;
        private SAPbouiCOM.ButtonCombo btnList;
        private SAPbouiCOM.Grid grHdr;
        private SAPbouiCOM.Grid grDt;

        private SAPbouiCOM.StaticText lblFDate;

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
                LoadDataGridReport();
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndLoadData);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private void grHdr_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
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

                if (pVal.ColUID != "Check")
                {
                    this.Freeze(false);
                    return;
                }
                UIHelper.CheckInGrid(this.grHdr, pVal.Row, SelectedDataIndexs, "PaymentKey");
                //var indexSelected = this.grHdr.GetDataTableRowIndex(pVal.Row);
                //if (this.grHdr.DataTable.GetValue("Check", indexSelected).ToString() == "Y")
                //{
                //    if (!SelectedDataIndexs.Contains(indexSelected))
                //        SelectedDataIndexs.Add(indexSelected);
                //}
                //else
                //{
                //    if (SelectedDataIndexs.Contains(indexSelected))
                //        SelectedDataIndexs.Remove(indexSelected);
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
                this.btnList.Item.Enabled = true;
                this.btnUnchk.Item.Enabled = true;
            }
            else
            {
                this.btnList.Item.Enabled = false;
                this.btnUnchk.Item.Enabled = false;
            }

            if (SelectedDataIndexs.Count == this.grHdr.DataTable.Rows.Count)
            {
                this.btnchkAll.Item.Enabled = false;
            }
            else
            {
                this.btnchkAll.Item.Enabled = true;
            }
        }

        private void grDt_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

        }

        private void btnList_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            //    this.Freeze(true);
            //    if (!IsProcess)
            //        HandleApproveAction();
            //    IsProcess = false;
            //    this.Freeze(false);
        }

        private void cbbPmTyL_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);
            if (_PaymentDocumentType == PaymentDocumentType.PT)
            {
                this.txtNote.Item.Enabled = false;
                this.btnList.Item.Visible = false;
                //this.btnUpdate.Item.Visible = false;
                this.cbbStatus.Item.Enabled = false;
                UIHelper.ClearSelectValidValues(cbbStatus);
            }
            else
            {
                this.txtNote.Item.Enabled = true;
                this.btnList.Item.Visible = true;
                //this.btnUpdate.Item.Visible = true;
                LoadComboboxAction();

                this.cbbStatus.Item.Enabled = true;
                LoadStatusCombobox();
            }
            this.Freeze(false);
        }


        private StaticText lblStt;
        private ComboBox cbbStatus;

        private void cbbStatus_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            this.Freeze(true);
            this.LoadComboboxAction();
            this.Freeze(false);
        }

        private Button btnchkAll;
        private Button btnUnchk;

        private void btnchkAll_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            //UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

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
                //this.SelectedDataIndexs = this.grData.DataTable.Rows.Count;

            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);

        }

        private void btnUnchk_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
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
    }
}

