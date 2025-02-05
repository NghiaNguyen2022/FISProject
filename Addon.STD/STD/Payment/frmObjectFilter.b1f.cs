using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STD.DataReader;
using SAPCore;
using SAPCore.Helper;
using SAPCore.SAP;
using STDApp.Models;
using SAPbouiCOM.Framework;

namespace STDApp.Payment
{
    [FormAttribute("STDApp.Payment.frmObjectFilter", "Payment/frmObjectFilter.b1f")]
    class frmObjectFilter : UserFormBase
    {
        private static frmObjectFilter instance;
        public string BaseForm;
        public static bool IsFormOpen = false;
        public frmObjectFilter()
        {
            Selects = new List<string>();
        }

        private List<int> SelectedDataIndexs = new List<int>();
        public List<string> Selects;
        public event EventHandler<SelectionEventArgs> OnSelectData;
        private string SelectsString
        {
            get
            {
                var result = string.Empty;
                if (Selects.Count > 0)
                {
                    result += "(";
                    for (var index = 0; index < Selects.Count; index++)
                    {
                        if (index > 0)
                            result += ",";
                        result += $"'{Selects[index]}'";
                    }
                    result += ")";
                }
                return result;
            }
        }
        string ReturnString
        {
            get
            {
                var result = string.Empty;
                if (Selects.Count > 0)
                {
                    //result += "(";
                    for (var index = 0; index < Selects.Count; index++)
                    {
                        if (index > 0)
                            result += ";";
                        result += $"{Selects[index]}";
                    }
                    //result += ")";
                }
                return result;
            }
            set
            {
                var list = value.Split(';');
                Selects.AddRange(list);
            }
        }

        public bool IsFindName
        {
            get
            {
                if(ckType != null && !ckType.Checked)
                {
                    return false;
                }
                return true;
            }
        }

        public string CardType { get; set; }
        public static frmObjectFilter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new frmObjectFilter();
                    instance.InitControl();
                }
                return instance;
            }
        }
        private void InitControl()
        {
            ckType.Checked = true;
        }

        public void LoadData(string query)
        {
            this.Freeze(true);
            try
            {
                this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_0").ValueEx = txtCode.Value;

                if (this.grData != null)
                {
                    this.grData.DataTable.Clear();

                    this.grData.DataTable.ExecuteQuery(query);

                    this.grData.Columns.Item("Check").TitleObject.Caption = STRING_CONTRANTS.Title_Choose;
                    this.grData.Columns.Item("Check").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                    this.grData.Columns.Item("Check").Editable = true;

                    var bpCaptionCode = CardType == "C" ? STRING_CONTRANTS.Title_CustomerCode : STRING_CONTRANTS.Title_VendorCode;
                    this.grData.Columns.Item("CardCode").TitleObject.Caption = bpCaptionCode;
                    this.grData.Columns.Item("CardCode").Editable = false;

                    var bpCaptionName = CardType == "C" ? STRING_CONTRANTS.Title_CustomerName : STRING_CONTRANTS.Title_VendorName;
                    this.grData.Columns.Item("CardName").TitleObject.Caption = bpCaptionName;
                    this.grData.Columns.Item("CardName").Editable = false;

                    SAPbouiCOM.EditTextColumn oCol = null;
                    oCol = (SAPbouiCOM.EditTextColumn)this.grData.Columns.Item("CardCode");
                    oCol.LinkedObjectType = SAPObjectType.oBusinessPartners;
                    this.grData.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage($"Load Data error: {ex.Message}");
            }
            this.Freeze(false);
        }

        private string GetQuery()
        {
            string query;
            if (Selects.Count == 0)
            {
                query = string.Format(QueryString.ObjectCustomerFilter, CardType, "", "N");
            }
            else
            {
                var sub1 = " AND  \"CardCode\" IN " + SelectsString;
                var query1 = string.Format(QueryString.ObjectCustomerFilter, CardType, sub1, "Y");
                var sub2 = " AND  \"CardCode\" NOT IN " + SelectsString;
                var query2 = string.Format(QueryString.ObjectCustomerFilter, CardType, sub2, "N");

                query = query1 + " UNION ALL " + query2;
            }

            return query;
        }

        public void ShowForm()
        {
            Instance.Show();
            IsFormOpen = true;
        }
        public void ShowForm(string list, string type, string formName = "")
        {
            if (!IsFormOpen)
            {
                Instance.ReturnString = list;
                Instance.CardType = type;
                Instance.BaseForm = formName;
                Instance.Show();

                var query = string.Empty;
                query = GetQuery();
                LoadData(query);
                IsFormOpen = true;
                SetNote();
            }
            else
            {

            }
        }

        private void SetNote()
        {
            if (Selects.Contains(""))
                Selects.Remove("");
            if (Selects.Count == 0)
                lblNote.Caption = $"Chưa có mã được chọn";
            else
                lblNote.Caption = $"{Selects.Count} có mã được chọn";
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblFind = ((SAPbouiCOM.StaticText)(this.GetItem("lblFi").Specific));
            this.txtCode = ((SAPbouiCOM.EditText)(this.GetItem("txtCod").Specific));
            this.txtCode.ValidateAfter += new SAPbouiCOM._IEditTextEvents_ValidateAfterEventHandler(this.txtCode_ValidateAfter);
            this.btnFind = ((SAPbouiCOM.Button)(this.GetItem("btnFin").Specific));
            this.btnFind.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFind_ClickBefore);
            this.grData = ((SAPbouiCOM.Grid)(this.GetItem("grData").Specific));
            this.grData.ClickBefore += new SAPbouiCOM._IGridEvents_ClickBeforeEventHandler(this.grData_ClickBefore);
            this.grData.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.grData_ClickAfter);
            this.btnEx = ((SAPbouiCOM.Button)(this.GetItem("btnEx").Specific));
            this.btnEx.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnEx_ClickBefore);
            this.btnChoose = ((SAPbouiCOM.Button)(this.GetItem("btnCho").Specific));
            this.btnChoose.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnChoose_ClickBefore);
            this.lblNote = ((SAPbouiCOM.StaticText)(this.GetItem("lblNot").Specific));
            this.txtFocus = ((SAPbouiCOM.EditText)(this.GetItem("txtFocus").Specific));
            this.ckType = ((SAPbouiCOM.CheckBox)(this.GetItem("ckType").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseAfter += new CloseAfterHandler(this.Form_CloseAfter);

        }

        private SAPbouiCOM.StaticText lblFind;

        private void OnCustomInitialize()
        {

        }

        private SAPbouiCOM.EditText txtCode;
        private SAPbouiCOM.Button btnFind;
        private SAPbouiCOM.Grid grData;
        private SAPbouiCOM.Button btnEx;
        private SAPbouiCOM.Button btnChoose;

        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            instance = null;
            IsFormOpen = false;
        }

        private void btnEx_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.UIAPIRawForm.Close();
        }

        private void btnChoose_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //Selects.Clear();
            //for (var i = 0; i < SelectedDataIndexs.Count; i++)
            //{
            //    var index = SelectedDataIndexs[i];
            //    Selects.Add(this.grData.DataTable.GetValue("CardCode", index).ToString());
            //}
            OnSelectData?.Invoke(this, new SelectionEventArgs(ReturnString, CardType));
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

            // UIHelper.CheckInGrid(this.grData, pVal.Row, SelectedDataIndexs, "");
            var indexSelected = grData.GetDataTableRowIndex(pVal.Row);
            var cardcode = this.grData.DataTable.GetValue("CardCode", indexSelected).ToString();

            if (grData.DataTable.GetValue("Check", indexSelected).ToString() == "Y")
            {
                if (!Selects.Contains(cardcode))
                    Selects.Add(cardcode);

                //AutoFillData(indexSelected);
            }
            else
            {
                if (Selects.Contains(cardcode))
                    Selects.Remove(cardcode);
            }

            SetNote();
            this.Freeze(false);
        }

        private SAPbouiCOM.StaticText lblNote;

        int startIndex = 0;
        private void grData_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
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

            var startIndex = grData.GetDataTableRowIndex(pVal.Row);            
            this.Freeze(false);

        }

        private void btnFind_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            this.Freeze(true);
            if (this.txtCode.Value != string.Empty)
            {
                var query = string.Empty;
                query = GetQuery();
                if(!IsFindName)
                    query += " AND \"CardCode\" = '" + this.txtCode.Value + "'";
                else
                    query += " AND UPPER(\"CardName\") LIKE '%" + this.txtCode.Value.ToUpper() + "%'";
                LoadData(query);
            }
            else
            {
                var query = string.Empty;
                query = GetQuery();
                LoadData(query);
            }
            this.Freeze(false);
        }

        private SAPbouiCOM.EditText txtFocus;

        private void txtCode_ValidateAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            txtFocus.Item.Click();
        }

        private SAPbouiCOM.CheckBox ckType;
    }
}
