using SAPCore.Helper;
using SAPCore.SAP.DIAPI;

namespace STD.DataReader
{
    public partial class QueryString
    {
        public static string DBName
        {
            get { return DIConnection.Instance.CompanyDB; }
        }
        private static string CallStoreBySystem(string query, string param = "")
        {
            return QueryUtil.CallStoreBySystem(DBName, query, param);            
        }

        public static string BranchesLoad
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT \"BPLId\", \"BPLName\", \"AliasName\" FROM \"" + schema + "\".\"OBPL\" ";
            }
        }
        public static string CustomerLoad
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT \"CardCode\", \"CardName\" FROM \"" + schema + "\".\"OCRD\" WHERE \"CardType\" = 'C'";
            }
        }

        public static string ObjectCustomerFilter
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT '{2}' AS \"Check\",  \"CardCode\", \"CardName\" FROM \"" + schema + "\".\"OCRD\" WHERE \"CardType\" = '{0}' {1}";
            }
        }

        public static string CountCustomer
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT Count(*) AS \"Count\" FROM \"" + schema + "\".\"OCRD\" WHERE \"CardType\" = 'C'";
            }
        }

        public static string CurrencyLoad
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT \"CurrCode\", \"CurrName\" FROM \"" + schema + "\".\"OCRN\" ";
            }

        }

        public static string LoadInvoicesToPayment
        {
            get
            {
                return CallStoreBySystem("usp_Bank_APInvoicesToPayment", "'{0}', '{1}', '{2}', '{3}', '{4}', '{5}'");
            }
        }

        public static string GetBPInformation
        {
            get
            {
                return CallStoreBySystem("usp_Bank_LoadBPInfor", "'{0}'");
            }
        }

        public static string LoadPaymentsReport
        {
            get
            {
                //CALL "usp_PaymentListReport"('20240102','20240102', 'PT', '4')
                return CallStoreBySystem("usp_PaymentListReport", "'{0}', '{1}'");
            }
        }
        public static string LoadPaymentsReportWithStatus
        {
            get
            {
                //CALL "usp_PaymentListReport"('20240102','20240102', 'PT', '4')
                return CallStoreBySystem("usp_PaymentListReport", "'{0}', '{1}', '{2}', '{3}', '{4}', '{5}'");
            }
        }
        public static string LoadPaymentsReportDetail
        {
            get
            {
                return CallStoreBySystem("usp_PaymentListReportDetail", "'{0}'");
            }
        }
        public static string GenerateKeyQuery
        {
            get
            {
                return CallStoreBySystem("usp_GeneratePaymentKey", "'{0}', '{1}', '{2}'");
            }
        }

        public static string BanksLoad
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT * FROM  \"" + schema + "\".\"vw_Bank_BankAccount\"";
            }
        }
        public static string BankLoad
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT * FROM  \"" + schema + "\".\"vw_Bank_BankAccount\" WHERE \"Key\" = '{0}'";
            }
        }
        public static string CflowsLoad
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT * FROM  \"" + schema + "\".\"vw_Bank_vwCashFlow\"";
            }
        }
        public static string AccountsLoad
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT \"AcctCode\", \"AcctName\" FROM \"" + schema + "\".\"OACT\" WHERE \"Finanse\" = 'Y'";
            }
        }
        public static string GetAPICode
        {
            get
            {
                var schema = DBName;
                /*
                    SELECT TOP 1 "U_Key" AS "Code" 
                      FROM "@ZAPIKEY"
                     ORDER BY "CreateDate" DESC
                 */
                return "SELECT TOP 1 \"U_Key\" AS \"Code\" FROM  \"" + schema + "\".\"@ZAPIKEY\"  ORDER BY \"CreateDate\" DESC";
            }   
        }
        //public static string ApproveConfigLoad
        //{
        //    get
        //    {
        //        var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
        //        return "SELECT * FROM \"" + schema + "\".\"vw_PaymentApproveConfig\" ";
        //    }
        //}

        //public static string RequesterApproveConfigLoad
        //{
        //    get
        //    {
        //        var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
        //        return "SELECT * FROM \"" + schema + "\".\"vw_PaymentApproveConfig\" WHERE \"Requester\" = '{0}' ";
        //    }
        //}

        //public static string ApproverApproveConfigLoad
        //{
        //    get
        //    {
        //        var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
        //        return "SELECT * FROM \"" + schema + "\".\"vw_PaymentApproveConfig\" WHERE \"Approver\" = '{0}' ";
        //    }
        //}

        public static string CheckUserRequest
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT Count(*) AS \"Exist\" FROM \"" + schema + "\".\"vw_PaymentUserRole\" WHERE \"Role\" = 'Requester' AND \"UserName\" = '{0}'";
            }
        }

        public static string CheckUserReviewer
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT Count(*) AS \"Exist\" FROM \"" + schema + "\".\"vw_PaymentUserRole\" WHERE \"Role\" = 'Reviewer' AND \"UserName\" = '{0}'";
            }
        }

        public static string CheckUserApprover
        {
            get
            {
                var schema = DBName; //ConfigurationManager.AppSettings["Schema"];
                return "SELECT Count(*) AS \"Exist\" FROM \"" + schema + "\".\"vw_PaymentUserRole\" WHERE \"Role\" = 'Approver' AND \"UserName\" = '{0}'";
            }
        }

        public static string CheckOrderExists
        {
            get
            {
                return CallStoreBySystem("usp_Payoo_CheckOrderExists", "'{0}'");
            }
        }
    }
}
