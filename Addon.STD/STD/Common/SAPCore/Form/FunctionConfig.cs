namespace SAPCore.Form
{
    public abstract class FunctionConfig
    {
        ///public List<AddonUserForm> FormMenus { get; set; }

        public AddonFolder FolderInfomation { get; set; }

        public FunctionConfig()
        {
            FolderInfomation = new AddonFolder();
            //FormMenus = new List<AddonUserForm>();
        }

        public void LoadMenu(string mainParentID, int index = 0)
        {
            AddFolder(mainParentID, index);
            AddMenus();
        }

        protected virtual void AddFolder(string mainParentID, int index = 0)
        {
            UIHelper.AddMenuFolder(FolderInfomation.MenuID, FolderInfomation.MenuName, FolderInfomation.ParentID, index);
        }

        protected void AddForms(AddonUserForm form, int index = 0)
        {
            UIHelper.AddMenuItem(form.MenuID, form.MenuName, form.ParentID, index);
        }

        protected virtual void AddMenus()
        {

        }
    }
}
