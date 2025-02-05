namespace SAPCore.Config
{
    public abstract class DatabaseConfiguration
    {
        public bool Init(ref string message)
        {
            if (!SAPUDTs(ref message))
                return false;
            if (!SAPUDFs(ref message))
                return false;
            if (!SAPUDOs(ref message))
                return false;
            if (!UserTables(ref message))
                return false;   
            if (!UserViews(ref message))
                return false;
            if (!UserSPs(ref message))
                return false;
            return true;
        }

        public bool Update(ref string message)
        {
            return true;
        }
        public virtual bool SAPUDFs(ref string message)
        {
            return true;
        }
        public virtual bool SAPUDTs(ref string message)
        {
            return true;
        }
        public virtual bool SAPUDOs(ref string message)
        {
            return true;
        }
        public virtual bool UserTables(ref string message)
        {
            return true;
        }
        public virtual bool UserViews(ref string message)
        {
            return true;
        }
        public virtual bool UserSPs(ref string message)
        {
            return true;
        }

    }
}
