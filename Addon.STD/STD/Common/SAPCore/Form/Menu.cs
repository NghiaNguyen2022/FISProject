using SAPCore.Form;
using SAPbouiCOM.Framework;
using System;

namespace SAPCore.Menu
{
    public class Menu
    {
        /// <summary>
        /// Init Menu for system
        /// </summary>
        public Menu()
        {
        }

        protected void AddMenuItem(AddonUserForm infor)
        {
            AddMenuItem(infor.MenuID, infor.MenuName, infor.ParentID);
        }
        /// <summary>
        /// Check Menu id Exist
        /// </summary>
        /// <param name="menuID">MenuId to check</param>
        /// <returns></returns>
        protected bool MenuExists(string menuID)
        {
            try
            {
                Application.SBO_Application.Menus.Item(menuID);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Add Menu Item to Menus list
        /// </summary>
        /// <param name="menuID">MenuId</param>
        /// <param name="menuName">Menu Description</param>
        /// <param name="parentId">Parent menu ID to set</param>
        protected void AddMenuItem(string menuID, string menuName, string parentId)
        {
            if (MenuExists(menuID))
                return;
            try
            {
                // Get the menu collection of the newly added pop-up item
                var oMenuItem = Application.SBO_Application.Menus.Item(parentId);
                var oMenus = oMenuItem.SubMenus;

                var item = ((SAPbouiCOM.MenuCreationParams)
                           (Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));

                item.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                item.UniqueID = menuID;
                item.String = menuName;

                oMenus.AddEx(item);
            }
            catch (Exception er)
            { //  Menu already exists
                Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        /// <summary>
        /// Add Folder to Menu
        /// </summary>
        /// <param name="folderID">Folder ID</param>
        /// <param name="foldelName">Folder Description</param>
        /// <param name="parentId">Parent menu ID to set</param>
        protected void AddMenuFolder(string folderID, string foldelName, string parentId = "")
        {
            if (MenuExists(folderID))
                return;
            try
            {
                SAPbouiCOM.Menus oMenus = null;
                SAPbouiCOM.MenuItem oMenuItem = null;
                oMenus = Application.SBO_Application.Menus;
                SAPbouiCOM.MenuCreationParams oCreationPackage = null;
                oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));

                if (string.IsNullOrEmpty(parentId))
                {
                    oMenuItem = Application.SBO_Application.Menus.Item("43520"); // moudles'
                }
                else
                {
                    oMenuItem = Application.SBO_Application.Menus.Item(parentId); // moudles'
                }

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
                oCreationPackage.UniqueID = folderID;
                oCreationPackage.String = foldelName;
                oCreationPackage.Enabled = true;
                oCreationPackage.Position = -1;

                oMenus = oMenuItem.SubMenus;
                oMenus.AddEx(oCreationPackage);

            }
            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(string.Format("Create Folder {0} has error: {1}", ex.Message, folderID), SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }
    }
}
