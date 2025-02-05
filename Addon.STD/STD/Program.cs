using System;
using System.Collections.Generic;
using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.SAP;

namespace STDApp
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application MainApplication = null;
                if (args.Length < 1)
                {
                    MainApplication = new Application();
                }
                else
                {
                    MainApplication = new Application(args[0]);
                }
               // ConfigDatabase();
                LoadDatabaseConfig();
                GlobalsConfig.Instance.FlagVersion = 1; //ConfigHelper.GetAddonVersion(GlobalsConfig.Instance.AddonName, GlobalsConfig.Instance.Version);

                if (GlobalsConfig.Instance.FlagVersion == 0)
                {
                    UIHelper.LogMessage("Version hiện tại đang cũ, hãy nâng cấp Version mới", UIHelper.MsgType.StatusBar, true);
                    return;
                }

                var ch = GlobalsConfig.Instance.Language;
                Menu MyMenu = new Menu();
                MainApplication.RegisterMenuEventHandler(MyMenu.SBO_Application_MenuEvent);
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
                Application.SBO_Application.ItemEvent += MyMenu.SBO_Application_ItemEvent;
                MainApplication.Run();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        static void LoadDatabaseConfig()
        {
            //DataHelper.LoadConfig();
        }
        static void ConfigDatabase()
        {
            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_PaymentKey"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Payment Key", "OVPM"));
            //    SAPHandler.Instance.CreateUDF("OVPM", "PaymentKey", "Payment Key", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_PaymentKey"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Payment Key", "ORCT"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "PaymentKey", "Payment Key", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_PayType"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Payment Type", "OVPM"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("PT", "Phiếu Thu");
            //    values.Add("PC", "Phiếu Chi");
            //    values.Add("UC", "Ủy Nhiệm Chi");
            //    SAPHandler.Instance.CreateUDF("OVPM", "PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "PT");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_PayType"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Payment Type", "ORCT"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("PT", "Phiếu Thu");
            //    values.Add("PC", "Phiếu Chi");
            //    values.Add("UC", "Ủy Nhiệm Chi");
            //    SAPHandler.Instance.CreateUDF("ORCT", "PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "PT");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_Bank"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Bank", "OVPM"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "Bank", "Bank", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_CashFlow"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Cash Flow", "OVPM"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "CashFlow", "Cash Flow", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_Bank"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Bank", "ORCT"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "Bank", "Bank", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "V_BANK");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_CashFlow"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Cash Flow", "ORCT"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "CashFlow", "Cash Flow", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_Review"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Review", "OVPM"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("Y", "Xác nhận");
            //    values.Add("N", "Chưa xác nhận");
            //    SAPHandler.Instance.CreateUDF("OVPM", "Review", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "N");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_Review"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Review", "ORCT"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("Y", "Xác nhận");
            //    values.Add("N", "Chưa xác nhận");
            //    SAPHandler.Instance.CreateUDF("ORCT", "Review", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "N");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_Status"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Status", "OVPM"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("N", "Yêu cầu thanh toán");
            //    values.Add("R", "Đề xuất thanh toán");
            //    values.Add("A", "Đã phê duyệt");
            //    values.Add("J", "Đã từ chối");
            //    values.Add("S", "Đã tạo");
            //    SAPHandler.Instance.CreateUDF("OVPM", "Status", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "N");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_Status"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Status", "ORCT"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("N", "Yêu cầu thanh toán");
            //    values.Add("R", "Đề xuất thanh toán");
            //    values.Add("A", "Đã phê duyệt");
            //    values.Add("J", "Đã từ chối");
            //    values.Add("S", "Đã tạo");
            //    SAPHandler.Instance.CreateUDF("ORCT", "Status", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "N");
            //}
        }

        static void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            var message = string.Empty;
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    //Exit Add-On
                    SAPUtils.StopAddon(GlobalsConfig.Instance.AddonName, ref message);
                    Application.SBO_Application.MessageBox(message);
                    System.Windows.Forms.Application.Exit();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    SAPUtils.StopAddon(GlobalsConfig.Instance.AddonName, ref message);
                    Application.SBO_Application.MessageBox(message);
                    break;
                default:
                    break;
            }
        }


        
    }
}
