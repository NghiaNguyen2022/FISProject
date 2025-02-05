using System;

using Excel = Microsoft.Office.Interop.Excel;

namespace SAPCore.Helper
{
    public static class Extensions
    {
        
        /// <summary>
        /// Export to excel for SAP Grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="filename"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ExportToExcel(this SAPbouiCOM.Grid grid, string filename, ref string message)
        {
            try
            {
                if (grid.DataTable == null || grid.DataTable.Columns.Count == 0)
                {
                    message = string.Format("ExportToExcel: Null or empty input table!\n");
                    return false;
                }

                // load excel, and create a new workbook
                var excelApp = new Excel.Application();
                excelApp.Workbooks.Add();

                // single worksheet
                Excel._Worksheet workSheet = excelApp.ActiveSheet;

                // column headings
                var dataIndex = 0;
                for(var index = 0; index < grid.DataTable.Columns.Count; index ++)
                {
                    var id = grid.DataTable.Columns.Item(index).Name;
                    var visible = grid.Columns.Item(id).Visible;
                    if (!visible)
                    {
                        continue;
                    }
                    workSheet.Cells[1, dataIndex + 1] = grid.Columns.Item(id).TitleObject.Caption;
                    workSheet.Cells[1, dataIndex + 1].Font.Bold = true;

                    for (var i = 0; i < grid.DataTable.Rows.Count; i++)
                    {
                        workSheet.Cells[i + 2, dataIndex + 1] = grid.DataTable.GetValue(id, i).ToString();
                    }
                    dataIndex++;
                }

                var excelFilePath = UIHelper.SaveExcelDiaglog(filename);
                // check file path
                if (!string.IsNullOrEmpty(excelFilePath))
                {
                    try
                    {
                        workSheet.SaveAs(excelFilePath);
                        excelApp.Quit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        return false;
                    }
                }
                else
                { // no file path is given
                    excelApp.Visible = true;
                    message = "no file path is given";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = string.Format("ExportToExcel: \n" + ex.Message);
                return false;
            }
        }

        public static void Freeze(this SAPbouiCOM.Framework.FormBase form, bool freeze)
        {
            try
            {
                if (freeze)
                    UIHelper.Freeze(form.UIAPIRawForm);
                else
                    UIHelper.UnFreeze(form.UIAPIRawForm);
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                //this.UIAPIRawForm.Freeze(true);
            }
        }
        public static void Refresh(this SAPbouiCOM.Framework.FormBase form)
        {
            form.UIAPIRawForm.Refresh();
        }
        public static void Close(this SAPbouiCOM.Framework.FormBase form)
        {
            form.UIAPIRawForm.Close();
        }

        public static void ReloadDocumentForm(this SAPbouiCOM.Framework.FormBase form)
        {
            var oform = SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm;
            var edit = (SAPbouiCOM.EditText)oform.Items.Item("8").Specific;
            var docnum = edit.Value;

            SAPbouiCOM.Framework.Application.SBO_Application.Menus.Item("1281").Activate();

            edit.Value = docnum;

            oform.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular);
        }

        public static SAPbouiCOM.DBDataSource DataSources(this SAPbouiCOM.Framework.FormBase form, string name)
        {
            return form.UIAPIRawForm.DataSources.DBDataSources.Item(name);
        }
        public static void OKMode(this SAPbouiCOM.Framework.FormBase form)
        {
            form.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
        }
        public static string EInvoiceStatusDescription(this SAPbouiCOM.Framework.FormBase form, string status)
        {
            switch (status)
            {
                case "Y":
                    return "Have E Invoice";
                case "C":
                    return "Cancled";
                case "R":
                    return "Is Replace";
                case "A":
                    return "Is Adjust";
                default:
                    return "Not Yet";
            }
        }
        public static string GetValueCustom(this SAPbouiCOM.Grid grid, string fieldName, int index)
        {
            return grid.DataTable.GetValue(fieldName, index).ToString();
        }

        public static void ColumnConfig(this SAPbouiCOM.GridColumn col, string caption, bool editable = false, bool visible = true, SAPbouiCOM.BoGridColumnType type = SAPbouiCOM.BoGridColumnType.gct_EditText)
        {
            col.TitleObject.Caption = caption;
            col.Type = type;
            col.Editable = editable;
            col.Visible = visible;
        }
    }
}
