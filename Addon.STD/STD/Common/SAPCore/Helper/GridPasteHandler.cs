using System;
using System.Windows.Forms;
using SAPbouiCOM;

public class GridPasteHandler
{
    private SAPbouiCOM.Application SBO_Application;
    private SAPbouiCOM.Grid oGrid;

    public GridPasteHandler(SAPbouiCOM.Application app, SAPbouiCOM.Grid grid)
    {
        SBO_Application = app;
        oGrid = grid;
        //SBO_Application.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(ItemEvent);
    }

    private void ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
    {
        BubbleEvent = true;
        if (pVal.FormTypeEx == "GTVWoodsland_CostCalc.Forms.frmCostAllocation" && pVal.ItemUID == "grInv")
        {
            if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_VALIDATE && pVal.ItemChanged == true)
            {
                    var oForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm;
                try
                {
                    oForm.Freeze(true);
                    var oGrid = (SAPbouiCOM.Grid)oForm.Items.Item(pVal.ItemUID).Specific;
                    oGrid.DataTable.Rows.Add();
                }
                catch (Exception ex)
                {

                }
                oForm.Freeze(false);
                //if (pVal.Row == oGrid.Rows.Count)
                //{
                //    AddLineID(oGrid, "C_0_1", "C_0_2");
                //}
                //if(Clipboard.ContainsText())
                //{
                //    var ch = 1;
                //}
                //if(Control.ModifierKeys == Keys.Control)
                //{
                //    var ch = 2;
                //}
                //var pressId = pVal.CharPressed;
                //if (Control.ModifierKeys == Keys.Control && Clipboard.ContainsText())
                //{
                //    string clipboardText = Clipboard.GetText();
                //    PasteDataIntoGrid(clipboardText);
                //}
            }
        }
    }
    private static void AddLineID(SAPbouiCOM.Grid oGird, string LineID, string CFLID)
    {
        try
        {
            oGird.DataTable.Rows.Add();
            //Int64 LineId = 0;
            //try
            //{
            //    oGird.Columns.Item(LineID).Editable = true;
            //    var oEditText = (SAPbouiCOM.EditText)oGird.Columns.Item(LineID).Cells.Item(oGird.RowCount - 1).Specific;
            //    if (oEditText.Value.ToString() == "")
            //    {
            //        oEditText.Value = oGird.RowCount.ToString();
            //    }

            //    LineId = Int64.Parse(oEditText.Value.ToString()) + 1;
            //}
            //catch
            //{
            //    LineId = Int64.Parse(oGird.RowCount.ToString()) + 1;
            //}
            //oEditText = (SAPbouiCOM.EditText)oGird.Columns.Item(LineID).Cells.Item(oGird.RowCount).Specific;
            //oEditText.Value = LineId.ToString();
            //try
            //{
            //    //oEditText = (SAPbouiCOM.EditText)oMatrix.GetCellSpecific("C_0_2", Rowid + i);
            //    oEditText = (SAPbouiCOM.EditText)oMatrix.Columns.Item(CFLID).Cells.Item(oGird.RowCount).Specific;
            //    //oEditText.Value = dttemp.GetValue("Code", i).ToString();
            //    oEditText.Value = "";
            //}
            //catch (Exception ex)
            //{

            //}
            //oGird.Columns.Item(LineID).Editable = false;
        }
        catch (Exception ex)
        {

        }
    }
    private void PasteDataIntoGrid(string clipboardText)
    {
        string[] lines = clipboardText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            string[] cells = line.Split('\t');
            int rowIndex = oGrid.Rows.Count;
            if (rowIndex != -1)
            {
                oGrid.DataTable.Rows.Add();
            }
            rowIndex = oGrid.Rows.Count;
            //// Add new row if the grid is empty or if the last row is filled
            //if (rowIndex == 0 || IsRowFilled(rowIndex - 1))
            //{
            //    oGrid.DataTable.Rows.Add();
            //    rowIndex = oGrid.Rows.Count - 1;
            //}
            //else
            //{
            //    rowIndex -= 1; // Use the last row if it is not filled
            //}

            for (int i = 0; i < cells.Length; i++)
            {
                oGrid.DataTable.SetValue(i, rowIndex, cells[i]);
            }
        }
    }

    private bool IsRowFilled(int rowIndex)
    {
        for (int colIndex = 0; colIndex < oGrid.DataTable.Columns.Count; colIndex++)
        {
            if (string.IsNullOrEmpty(oGrid.DataTable.GetValue(colIndex, rowIndex).ToString()))
            {
                return false;
            }
        }
        return true;
    }
}