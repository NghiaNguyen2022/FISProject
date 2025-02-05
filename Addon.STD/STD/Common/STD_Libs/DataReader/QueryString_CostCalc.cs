namespace STD.DataReader
{
    public partial class QueryString
    {
        public static string LoadYears
        {
            get
            {
                return CallStoreBySystem("usp_GetYear");
            }
        }

        public static string LoadPeriods
        {
            get
            {
                return CallStoreBySystem("usp_GetPeriods", "'{0}'");
            }
        }

        public static string LoadBranches
        {
            get
            {
                return CallStoreBySystem("usp_GetBraches");
            }
        }

        public static string LoadSector
        {
            get
            {
                return CallStoreBySystem("usp_GetSector");
            }
        }
        public static string LoadInventoryDataCostCalc
        {
            get
            {
                return CallStoreBySystem("LoadDataInventory", "'{0}', '{1}', '{2}', '{3}', ? ");
            }
        }
        public static string LoadDataExpenseCostCalc
        {
            get
            {
                return CallStoreBySystem("LoadDataExpense", "'{0}', '{1}', '{2}' ");
            }
        }
        public static string LoadAllocateCostCalc
        {
            get
            {
                return CallStoreBySystem("LoadDataAllocate", "'{0}', '{1}', '{2}', '{3}', ? ");
            }
        }
        public static string LoadBalanceInventory
        {
            get
            {
                return CallStoreBySystem("LoadBalanceInventory", "'{0}', '{1}', '{2}', '{3}' ");
            }
        }
        public static string Allocate
        {
            get
            {
                return CallStoreBySystem("SP_GT_Allocate", "'{0}', '{1}', '{2}', '{3}' ");
            }
        }
        public static string BeforeAllocate
        {
            get
            {
                return CallStoreBySystem("SP_GT_BeforeAllocate", "'{0}', '{1}', '{2}', '{3}' ");
            }
        }
        public static string AfterAllocateSuccess
        {
            get
            {
                return CallStoreBySystem("SP_GT_AfterAllocate", "'{0}', '{1}', '{2}', '{3}', 'Y', 'Thành công', '{4}', '{5}', '{6}' ");
            }
        }
        public static string AfterAllocateError
        {
            get
            {
                return CallStoreBySystem("SP_GT_AfterAllocate", "'{0}', '{1}', '{2}', '{3}', 'N', '{4}'");
            }
        }
        public static string LoadJEDetail
        {
            get
            {
                return CallStoreBySystem("LoadJEDetail", "'{0}', '{1}', '{2}', '{3}'");
            }
        }
    }
}
