using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.SAP.DIAPI;
using STDApp.Bank;
using STDApp.Common;
using STDApp.Payment;
using STDApp.Payoo;
using System;
using System.Configuration;

namespace STDApp
{
    class Menu
    {
        BankConfig bankConfig;

        public Menu()
        {
            var dbName = ConfigurationManager.AppSettings["Schema"];
            bankConfig = new BankConfig();
            if (DIConnection.Instance.CompanyDB == dbName)
            {
                UIHelper.AddMenuFolder(GlobalsConfig.Instance.CusPMFolderID, GlobalsConfig.Instance.CusPMFolderDesc, GlobalsConfig.Instance.ParentMenuID);

                bankConfig = new BankConfig();
                bankConfig.LoadMenu(GlobalsConfig.Instance.CusPMFolderID, 0);
            }
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {

                if (pVal.BeforeAction)
                {
                    var dbName = ConfigurationManager.AppSettings["Schema"];
                    if (DIConnection.Instance.CompanyDB == dbName)
                    {
                        if (pVal.MenuUID == bankConfig.InquiryForm.MenuID)
                        {
                            frmInquiry.ShowForm();
                        }
                        else if (pVal.MenuUID == bankConfig.PaymentForm.MenuID)
                        {
                            frmPayment.ShowForm();
                        }
                        else if (pVal.MenuUID == bankConfig.BatchForm.MenuID)
                        {
                            frmBatch.ShowForm();
                        }
                        else if (pVal.MenuUID == bankConfig.RateForm.MenuID)
                        {
                            frmRate.ShowForm();
                        }
                    }
                    //else if (pVal.MenuUID == GlobalsConfig.Instance.PaymentReviewFormInfo.MenuID)
                    //{
                    //    frmPaymentReview.ShowForm();
                    //}
                }

            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
        }
        private void HandleKeyPress(SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            // Allow control keys such as Backspace and Delete
            if (pVal.CharPressed < '0' || pVal.CharPressed > '9')
            {
                if (pVal.CharPressed != (char)8) // Backspace
                {
                    BubbleEvent = false; // Block input
                }
            }
        }
        public void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true; // Always set BubbleEvent to true
            try
            {
                var dbName = ConfigurationManager.AppSettings["Schema"];
                if (DIConnection.Instance.CompanyDB == dbName)
                {
                    // select cardcode and fill card name
                    if (pVal.FormTypeEx == bankConfig.PaymentForm.FormType)
                    {

                        if (!pVal.BeforeAction
                            && pVal.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                            && pVal.ItemUID == "grData")
                        {

                        }
                        else
                        {
                        }
                    }
                    else if (pVal.FormTypeEx == bankConfig.PaymentDetailFormInfo.FormType)
                    {
                        try
                        {
                            if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                            {
                                var oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                                if (oCFLEvento.BeforeAction == false)
                                {
                                    var oDataTable = oCFLEvento.SelectedObjects;
                                    var oForm = Application.SBO_Application.Forms.Item(FormUID);//.Item("Payment_F");
                                    oForm.Freeze(true);
                                    if (pVal.ItemUID == "txtBP")
                                    {
                                        oForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = System.Convert.ToString(oDataTable.GetValue(0, 0));
                                        oForm.DataSources.UserDataSources.Item("UD_Nam").ValueEx = System.Convert.ToString(oDataTable.GetValue(1, 0));
                                    }
                                    oForm.Freeze(false);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    else if (pVal.FormTypeEx == bankConfig.BatchForm.FormType)
                    {

                        if (pVal.BeforeAction
                            && pVal.EventType == SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                            && pVal.ItemUID == "txtPag")
                        {
                            HandleKeyPress(pVal, ref BubbleEvent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.Message);
            }
        }

    }
}
