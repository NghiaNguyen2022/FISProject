using System;
using System.Collections;
using STD.DataReader;
using STD.Models;
using STDApp.Models;
using SAPCore.Config;

namespace STDApp.DataReader
{
    public class DataHelper
    {

		public static void LoadConfig()
		{
            //listCustomerInSAP = ListCustomer();
            listCurrencyInSAP = ListCurrency();
		}

		private static Hashtable[] listCurrencyInSAP;
		private static Hashtable[] listCustomerInSAP;
		public static Hashtable[] ListCurrencyInSAP
		{
			get
			{
				if (listCurrencyInSAP == null)
				{
                    listCurrencyInSAP = ListCurrency();
				}
				
				return listCurrencyInSAP;
			}
		}

		public static Hashtable[] ListCustomerInSAP
		{
			get
			{
				if (listCustomerInSAP == null)
				{
					listCustomerInSAP = ListCustomer();
				}
				else
				{
					var count = CountCustomer();
					if (count != listCustomerInSAP.Length)
						listCustomerInSAP = ListCustomer();
				}

				return listCustomerInSAP;
			}
		}

        public static Hashtable[] LoadBranches()
        {
            Hashtable[] datas;
            using (var connection = CoreSetting.DataConnection)
            {
                datas = connection.ExecQueryToArrayHashtable(QueryString.BranchesLoad);
                connection.Dispose();
            }
            return datas;
        }

        private static Hashtable[] listBanks;
        public static Hashtable[] ListBanks
        {
            get
            {
                if (listBanks == null)
                {
                    listBanks = LoadBanks();
                }

                return listBanks;
            }
        }

        public static Hashtable[] LoadBanks()
        {
            Hashtable[] datas;
            using (var connection = CoreSetting.DataConnection)
            {
                datas = connection.ExecQueryToArrayHashtable(QueryString.BanksLoad);
                connection.Dispose();
            }
            return datas;
        }

        private static Hashtable[] listCashFlows;
        public static Hashtable[] ListCashFlows
        {
            get
            {
                if (listCashFlows == null)
                {
                    listCashFlows = LoadCashFlows();
                }

                return listCashFlows;
            }
        }

        public static Hashtable[] LoadCashFlows()
        {
            Hashtable[] datas;
            using (var connection = CoreSetting.DataConnection)
            {
                datas = connection.ExecQueryToArrayHashtable(QueryString.CflowsLoad);
                connection.Dispose();
            }
            return datas;
        }

        public static PaymentKeyByBranch LoadKey(string branchId, string type)
        {
            Hashtable data;
            using (var connection = CoreSetting.DataConnection)
            {
                data = connection.ExecQueryToHashtable(string.Format(QueryString.GenerateKeyQuery, branchId, type, DateTime.Now.ToString("yyyyMMdd")));
                connection.Dispose();
            }
            if(data != null)
            {
                var ret = new PaymentKeyByBranch();
                ret.BranchID =  data["Branch"].ToString();
                ret.Number = int.Parse(data["Number"].ToString());
                return ret;
            }
            return null;
        }


        public static Hashtable[] ListCustomer()
        {
            Hashtable[] datas;
            using (var connection = CoreSetting.DataConnection)
            {
                datas = connection.ExecQueryToArrayHashtable(QueryString.CustomerLoad);
                connection.Dispose();
            }
            return datas;
        }

		public static Hashtable[] ListCurrency()
		{
			Hashtable[] datas;
			using (var connection = CoreSetting.DataConnection)
			{
				datas = connection.ExecQueryToArrayHashtable(QueryString.CurrencyLoad);
				connection.Dispose();
			}
			return datas;
		}

		public static int CountCustomer()
        {
            Hashtable data;
            using (var connection = CoreSetting.DataConnection)
            {
                data = connection.ExecQueryToHashtable(QueryString.CountCustomer);
                connection.Dispose();
            }

            if(data != null)
            {
                var count = 0;
                if (int.TryParse(data["Count"].ToString(), out count))
                {
                    return count;
                }
            }

            return 0;
        }

        public static Hashtable[] LoadAccounts()
        {
            Hashtable[] datas;
            using (var connection = CoreSetting.DataConnection)
            {
                datas = connection.ExecQueryToArrayHashtable(QueryString.AccountsLoad);
                connection.Dispose();
            }
            return datas;
        }

        //public static bool CheckApproveUser(string userName)
        //{
        //    Hashtable[] datas;
        //    using (var connection = CoreSetting.DataConnection)
        //    {
        //        datas = connection.ExecQueryToArrayHashtable(string.Format(QueryString.ApproverApproveConfigLoad, userName));
        //        connection.Dispose();
        //    }
        //    return datas != null && datas.Length > 0;
        //}

        public static bool CheckUser(string userName, UserRole role)
        {
            var query = string.Empty;
            if (role == UserRole.Requester)
                query = string.Format(QueryString.CheckUserRequest, userName);
            else if (role == UserRole.Reviewer)
                query = string.Format(QueryString.CheckUserReviewer, userName);
            else if (role == UserRole.Approver)
                query = string.Format(QueryString.CheckUserApprover, userName);
            if (string.IsNullOrEmpty(query))
                return false;

            Hashtable data;
            using (var connection = CoreSetting.DataConnection)
            {
                data = connection.ExecQueryToHashtable(query);
                connection.Dispose();
            }
            var exist = false;
            if (data != null)
            {
                exist = data["Exist"].ToString() != "0";
            }
            return exist;// datas != null && datas.Length > 0;
        }
    }
}
