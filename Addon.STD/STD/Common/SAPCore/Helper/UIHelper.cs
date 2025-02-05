using SAPCore.Form;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ComboBox = SAPbouiCOM.ComboBox;

namespace SAPCore
{
    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {

        private IntPtr _hwnd;
        // Property

        public virtual IntPtr Handle
        {
            get { return _hwnd; }
        }

        // Constructor

        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

    }
    public class SAPDialog
    {
        public DialogResult result;
        public FileDialog dialog;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        IntPtr ptr = GetForegroundWindow();
        public void ShowDialog()
        {
            //System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            //form.TopMost = true;
            WindowWrapper oWindow = new WindowWrapper(ptr);
            result = dialog.ShowDialog(oWindow);
        }

        public SAPDialog()
        { }
    }
    public class UIHelper
    {
        public enum MsgType
        {
            StatusBar,
            Msgbox,
            Windowbox
        }
        public static int RGB(int red, int green, int blue)
        {
            return (red & 0xFF) | ((green & 0xFF) << 8) | ((blue & 0xFF) << 16);
        }
        /// <summary>
        /// Show message in UI by type: Message box or Status bar
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="msgboxType"> Message box or Status bar</param>
        /// <param name="isError">this param is mean this message is error or notice</param>
        /// <param name="btnDefault">button default: only apply for message box type</param>
        /// <param name="btnCaption1"></param>
        /// <param name="btnCaption2"></param>
        /// <param name="btnCaption3"></param>
        /// <returns></returns>
        public static int LogMessage(string msg, MsgType msgboxType = MsgType.StatusBar, bool isError = false, int btnDefault = 1, string btnCaption1 = "Ok", string btnCaption2 = "", string btnCaption3 = "")
        {
            //IL_0061: Unknown result type (might be due to invalid IL or missing references)
            try
            {
                switch (msgboxType)
                {
                    case MsgType.StatusBar:
                        SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage(msg, SAPbouiCOM.BoMessageTime.bmt_Short, isError);
                        return 1;
                    default:// case MsgType.Msgbox:
                        return SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(msg, btnDefault, btnCaption1, btnCaption2, btnCaption3);
                        //default:
                        //    MessageBox.Show(msg);
                        //    return 1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Show Diaglog with new thread to make sure un frezee system
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        public static DialogResult ShowGTDialog(FileDialog dialog)
        {
            SAPDialog state = new SAPDialog();
            state.dialog = dialog;

            System.Threading.Thread thread = new System.Threading.Thread(state.ShowDialog);
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
            thread.Join();

            return state.result;
        }
        public static string SavePDFDiaglog(string fileName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = fileName;
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //System.Windows.Forms.Application.StartupPath;
            saveFileDialog.Filter = "Pdf Files|*.pdf";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Export PDF file To";
            DialogResult ret = ShowGTDialog(saveFileDialog);
            if (ret == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        ///  Show Save diaglog excel
        /// </summary>
        /// <param name="fileName">default name for file to save</param>
        /// <returns>full path for file to save</returns>
        public static string SaveExcelDiaglog(string fileName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = fileName;
            saveFileDialog.InitialDirectory = @"C:\"; //System.Windows.Forms.Application.StartupPath;
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Export Excel File To";
            DialogResult ret = ShowGTDialog(saveFileDialog);
            if (ret == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Browser diaglog to find file
        /// </summary>
        /// <returns>full path for file to open</returns>
        public static string BrowserExcelDiaglog(SAPbouiCOM.IForm form)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\"; //System.Windows.Forms.Application.StartupPath;
            openFileDialog1.Title = "Select a Excel file to open";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;
            DialogResult ret = ShowGTDialog(openFileDialog1);
            if (ret == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            return string.Empty;
        }

        public static void Freeze(IForm form)
        {
            form.Freeze(true);
        }

        public static void UnFreeze(IForm form)
        {
            form.Freeze(false);
        }

        public static void ComboboxSelectDefault(ComboBox comboBox, int defValue = 0)
        {
            comboBox.Select(defValue, SAPbouiCOM.BoSearchKey.psk_Index);
            comboBox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            comboBox.Item.DisplayDesc = true;
        }
        public static void ComboboxSelectDefaultValue(ComboBox comboBox, string defValue = "")
        {
            comboBox.Select(defValue, SAPbouiCOM.BoSearchKey.psk_ByValue);
            comboBox.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription;
            comboBox.Item.DisplayDesc = true;
        }

        public static void LoadComboboxFromDataSource(ComboBox comboBox, DataTable dataTable, string query, string keyField, string valueField, int defValue = 0, string allValue = "", string allDescription = "")
        {
            for (var i = comboBox.ValidValues.Count - 1; i >= 0; i--)
            {
                comboBox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
            }
            dataTable.Clear();
            dataTable.ExecuteQuery(query);
            int dtcount = dataTable.Rows.Count;

            if(!string.IsNullOrEmpty(allValue))
            {

                comboBox.ValidValues.Add(allValue, allDescription);
            }
            for (int j = 0; j < dtcount; j++)
            {
                comboBox.ValidValues.Add(dataTable.Columns.Item(keyField).Cells.Item(j).Value.ToString(), dataTable.Columns.Item(valueField).Cells.Item(j).Value.ToString());
            }
            comboBox.Select(defValue, SAPbouiCOM.BoSearchKey.psk_Index);
            comboBox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            comboBox.Item.DisplayDesc = true;
        }

        public static void ClearSelectValidValues(ComboBox comboBox)
        {
            for (var i = comboBox.ValidValues.Count - 1; i >= 0; i--)
            {
                comboBox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
            }
        }
        public static void ClearSelectValidValues(ComboBoxColumn comboBox)
        {
            for (var i = comboBox.ValidValues.Count - 1; i >= 0; i--)
            {
                comboBox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
            }
        }
        public static void CheckInGrid(Grid grid, int rowSelect, List<int> selectList, string groupName, string checkName = "Check")
        {
            var indexSelected = grid.GetDataTableRowIndex(rowSelect);
            if (indexSelected == -1)
            {
                if (groupName != string.Empty)
                {
                    var nextRow = rowSelect + 1;
                    var netIndex = grid.GetDataTableRowIndex(nextRow);
                    var groupCode = grid.DataTable.GetValue(groupName, netIndex).ToString();

                    var rowcount = grid.DataTable.Rows.Count;
                    for (int index = netIndex; index < rowcount; index++)
                    {
                        var groupselect = grid.DataTable.GetValue(groupName, index).ToString();
                        if (groupselect == groupCode)
                        {
                            grid.DataTable.SetValue(checkName, index, "Y");
                            if (!selectList.Contains(index))
                                selectList.Add(index);
                            //AutoFillData(index);
                        }
                        else
                        {
                            if (index > netIndex)
                                break;
                        }
                    }
                }
            }
            else
            {
                if (grid.DataTable.GetValue(checkName, indexSelected).ToString() == "Y")
                {
                    if (!selectList.Contains(indexSelected))
                        selectList.Add(indexSelected);

                    //AutoFillData(indexSelected);
                }
                else
                {
                    if (selectList.Contains(indexSelected))
                        selectList.Remove(indexSelected);
                }
            }
        }

        public static bool MenuExists(string menuID)
        {
            try
            {
                SAPbouiCOM.Framework.Application.SBO_Application.Menus.Item(menuID);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void AddMenuItem(string menuID, string menuName, string parentId, int position)
        {
            try
            {

                // Get the menu collection of the newly added pop-up item
                var oMenuItem = SAPbouiCOM.Framework.Application.SBO_Application.Menus.Item(parentId);
                var oMenus = oMenuItem.SubMenus;

                if (MenuExists(menuID))
                    oMenus.RemoveEx(menuID);
                //var item = ((SAPbouiCOM.MenuCreationParams)
                //           (SAPbouiCOM.Framework.Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));

                //item.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                //item.UniqueID = menuID;
                //item.String = menuName;

                //oMenus.AddEx(item);

                oMenus.Add(menuID, menuName, SAPbouiCOM.BoMenuType.mt_STRING, position);
            }
            catch (Exception er)
            { //  Menu already exists
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        /// <summary>
        /// Add Menu Item to Menus list
        /// </summary>
        /// <param name="menuID">MenuId</param>
        /// <param name="menuName">Menu Description</param>
        /// <param name="parentId">Parent menu ID to set</param>
        public static void AddMenuItem(string menuID, string menuName, string parentId)
        {
            if (MenuExists(menuID))
                return;
            try
            {
                // Get the menu collection of the newly added pop-up item
                var oMenuItem = SAPbouiCOM.Framework.Application.SBO_Application.Menus.Item(parentId);
                var oMenus = oMenuItem.SubMenus;

                var item = ((SAPbouiCOM.MenuCreationParams)
                           (SAPbouiCOM.Framework.Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));

                item.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                item.UniqueID = menuID;
                item.String = menuName;

                oMenus.AddEx(item);
            }
            catch (Exception er)
            { //  Menu already exists
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        public static void AddMenuFolder(string folderID, string foldelName, string parentId = "")
        {
            //if (MenuExists(folderID))
            //    return;
            try
            {
                SAPbouiCOM.Menus oMenus = null;
                SAPbouiCOM.MenuItem oMenuItem = null;
                oMenus = SAPbouiCOM.Framework.Application.SBO_Application.Menus;
                SAPbouiCOM.MenuCreationParams oCreationPackage = null;
                oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(SAPbouiCOM.Framework.Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));

                if (string.IsNullOrEmpty(parentId))
                {
                    oMenuItem = SAPbouiCOM.Framework.Application.SBO_Application.Menus.Item("43520"); // moudles'
                }
                else
                {
                    oMenuItem = SAPbouiCOM.Framework.Application.SBO_Application.Menus.Item(parentId); // moudles'
                }

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
                oCreationPackage.UniqueID = folderID;
                oCreationPackage.String = foldelName;
                oCreationPackage.Enabled = true;
                oCreationPackage.Position = -1;

                oMenus = oMenuItem.SubMenus;

                if (MenuExists(folderID))
                    oMenus.RemoveEx(folderID);

                oMenus.AddEx(oCreationPackage);

            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage(string.Format("Create Folder {0} has error: {1}", ex.Message, folderID), SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }
        public static void AddMenuItem(AddonUserForm infor)
        {
            AddMenuItem(infor.MenuID, infor.MenuName, infor.ParentID);
        }
        public static void AddMenuItem(AddonUserForm infor, int position)
        {
            AddMenuItem(infor.MenuID, infor.MenuName, infor.ParentID, position);
        }

        public static string GetTextboxValue(SAPbouiCOM.EditText txttext, string defaultValue = "")
        {
            if (txttext == null || txttext.Value == null || txttext.Value == "")
            {
                return defaultValue;
            }

            return txttext.Value.ToString();
        }

        public static string GetComboValue(SAPbouiCOM.ComboBox comboBox, string defaultValue = "")
        {
            if (comboBox == null || comboBox.Selected == null)
                return defaultValue;
            return comboBox.Selected.Value;
        }

        internal static void AddMenuFolder(string folderID, string foldelName, string parentId, int position)
        {
            try
            {

                // Get the menu collection of the newly added pop-up item
                var oMenuItem = SAPbouiCOM.Framework.Application.SBO_Application.Menus.Item(parentId);
                var oMenus = oMenuItem.SubMenus;

                if (MenuExists(folderID))
                    oMenus.RemoveEx(folderID);

                oMenus.Add(folderID, foldelName, SAPbouiCOM.BoMenuType.mt_POPUP, position);
            }
            catch (Exception er)
            { //  Menu already exists
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }
    }
}
