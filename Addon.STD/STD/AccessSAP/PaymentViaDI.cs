using System;
using System.Collections.Generic;
using System.Linq;
using STDApp.Models;
using PN.SmartLib.Helper;
using SAPbobsCOM;
using SAPCore.SAP.DIAPI;
using STD.DataReader;
using System.Globalization;
using SAPCore;

namespace STDApp.AccessSAP
{
    public class PaymentViaDI
    {
        private static bool ConnectDI(ref string message)
        {
            try
            {
                var retConnect = DIConnection.Instance.Connect(ref message, DIConnection.Instance.CookieConnection);
                if (!retConnect)
                {
                    var mess = DIConnection.Instance.Company.GetLastErrorDescription();
                    message = STRING_CONTRANTS.CanNotConnectDIAPI + " - " + mess;
                }
                return retConnect;
            }
            catch (Exception ex)
            {
                message = STRING_CONTRANTS.CanNotConnectDIAPI + " - " + ex.Message;
                return false;
            }
        }

        private static void DisConnectDI()
        {
            DIConnection.Instance.DIDisconnect();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docentry"></param>
        /// <param name="newVal"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool UpdateAccountPayment(ChangeOfApprove data, ref string message)
        {
            if (!ConnectDI(ref message))
                return false;

            var ret = false;

            try
            {
                var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                if (oPayment.GetByKey(data.DocEntry))
                {
                    if (!string.IsNullOrEmpty(data.OldAccount) && data.OldAccount != data.ChangeAccount)
                    {
                        if (!string.IsNullOrEmpty(data.ChangeAccount))
                        {
                            oPayment.CashAccount = data.ChangeAccount;
                        }
                    }
                    if (!string.IsNullOrEmpty(data.OldBank) && data.OldBank != data.ChangeBank)
                    {
                        if (!string.IsNullOrEmpty(data.ChangeBank))
                        {
                            oPayment.UserFields.Fields.Item("U_BANK").Value = data.ChangeBank;
                            var bankAccount = ViewHelper.Banks.Where(x => x.Code == data.ChangeBank).Select(x => x.Account).FirstOrDefault();
                            oPayment.TransferAccount = bankAccount;
                        }
                    }
                    oPayment.UserFields.Fields.Item("U_Review").Value = "Y";
                    var saveret = oPayment.Update();
                    if (saveret != 0)
                    {
                        message = DIConnection.Instance.Company.GetLastErrorDescription();
                        ret = false;
                    }
                    else
                    {
                        message = STRING_CONTRANTS.Notice_UpdatePaymentSuccess;
                        ret = true;
                    }
                }
                else
                {
                    ret = false;
                    message = STRING_CONTRANTS.Notice_CanNotloadDraft;
                }
            }
            catch (Exception ex)
            {
                ret = false;
                message = ex.Message;
            }
            DisConnectDI();
            return ret;
        }
        public static bool CreateIncommingPayment(string BatchNo, ref string message)
        {            
            var query = string.Format(QueryString.PaymentData, BatchNo);
            var datas = dbProvider.QueryList(query);
            var invoices = new List<InvoicePayment>();
            if (datas != null && datas.Count() > 0)
            {
                foreach (var itm in datas)
                {
                    var inv = new InvoicePayment();
                    inv.CardCode = itm["CardCode"].ToString();
                    inv.DocEntry = itm["DocEntry"].ToString();
                    inv.DocCur = itm["DocCur"].ToString();
                    inv.OrderNo = itm["OrderNo"].ToString();
                    inv.TransferAmount = itm["TransferAmount"].ToString();
                    invoices.Add(inv);
                }
            }
            if (invoices.Count <= 0)
            { 
                message = "Không có dữ liệu";
            }

            if (!ConnectDI(ref message))
                return false;
            int lErrCode = -1 ;
            foreach (var cardcode in invoices.Select(x => x.CardCode).Distinct())
            {
                lErrCode = -1;

                var invs = invoices.Where(x => x.CardCode == cardcode);
                var cardCode = cardcode;
                var currency = invs.Select(x => x.DocCur).Distinct().FirstOrDefault();
            
                SAPbobsCOM.Payments oPayment;
                oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oIncomingPayments);

                oPayment.CardCode = cardCode;
                oPayment.DocCurrency = currency;

                var date = BatchNo.Replace("SP", "20").Remove(BatchNo.Length - 2, 2);
                oPayment.DocDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
                var objectType = BoRcptInvTypes.it_Invoice;
                double total = 0;
                var docEntry = string.Empty;
                var i = 0;
                foreach (var item in invs)
                {
                    oPayment.Invoices.Add();
                    oPayment.Invoices.SetCurrentLine(i);
                    oPayment.Invoices.DocEntry = int.Parse(item.DocEntry);
                    oPayment.Invoices.InvoiceType = objectType;
                    oPayment.Invoices.SumApplied = double.Parse(item.TransferAmount);
                    total += oPayment.Invoices.SumApplied;
                    docEntry += item.DocEntry + ";";
                    i++;
                }
                oPayment.TransferSum = total;
                oPayment.TransferAccount = "112101";

                var name = cardCode;
                var wuer = "SELECT \"CardName\" FROM \"" + DIConnection.Instance.CompanyDB + "\".OCRD WHERE \"CardCode\" = '" + cardCode + "'";
                var hash = dbProvider.QuerySingle(wuer);
                if (hash != null)
                    name = hash["CardName"].ToString();
                oPayment.Remarks = "Khách hàng " + name + " chuyển tiền hóa đơn " + docEntry;
            
                var ret1 = -1;
                ret1 = oPayment.Add();

                var newId = string.Empty;
                if (ret1 != 0)
                {
                    DIConnection.Instance.Company.GetLastError(out lErrCode, out message);

                }
                else
                {
                    DIConnection.Instance.Company.GetNewObjectCode(out newId);
                    message = $"Tạo thành đơn id {newId}";
                }
                UIHelper.LogMessage(message, UIHelper.MsgType.StatusBar, ret1 != 0);

                foreach (var item in invs)
                {
                    query = string.Format(QueryString.UpdateAfterClear,
                                            ret1 == 0 ? "Y": "N", 
                                            "",
                                            message,
                                            newId,
                                            BatchNo, 
                                            item.OrderNo);
                    var retupdate = dbProvider.ExecuteNonQuery(query);
                }
            }
            DisConnectDI();
            return true;
        }
        public static PaymentActionResult CreatePayment(PaymentDetail data, string key, string date, ref string message)
        {
            if (!ConnectDI(ref message))
                return null;
            var result = new PaymentActionResult();
            try
            {
                result.CardCode = data.CardCode;

                int lRetCode = -1, lErrCode;
                SAPbobsCOM.Payments oPayment;
                oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oVendorPayments);

                oPayment.CardCode = data.CardCode;
                oPayment.DocCurrency = data.Currency;

                // oPayment.DocDate = data.DocDate;
                DateTime date1;
                if (DateTime.TryParseExact(date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date1))
                    oPayment.DocDate = date1;
                else
                    oPayment.DocDate = data.DocDate;
                var name = data.CardCode;
                var wuer = "SELECT \"CardName\" FROM \"" + DIConnection.Instance.CompanyDB + "\".OCRD WHERE \"CardCode\" = " + data.CardCode;
                var hash = dbProvider.QuerySingle(wuer);
                if (hash != null)
                    name = hash["CardName"].ToString();
                oPayment.Remarks = "Thanh toán cho " + name;
                var objectType = BoRcptInvTypes.it_PurchaseInvoice;

                if (data.DocNum != string.Empty)
                {
                    oPayment.Invoices.Add();
                    oPayment.Invoices.SetCurrentLine(0);
                    oPayment.Invoices.DocEntry = int.Parse(data.DocEntry);
                    oPayment.Invoices.InvoiceType = objectType;
                    if (data.Currency == GlobalsConfig.Instance.LocalCurrencyDefault)
                        oPayment.Invoices.SumApplied = (double)data.Amount;
                    else
                        oPayment.Invoices.AppliedFC = (double)data.Amount;
                }
                oPayment.TransferAccount = "112101";// d;ata.BankAccount;112101
                oPayment.TransferSum = (double)data.Amount;
                oPayment.UserFields.Fields.Item("U_PaymentKey").Value = key;
                if (data.Cashflow != "-")
                {

                    oPayment.PrimaryFormItems.Add();
                    oPayment.PrimaryFormItems.SetCurrentLine(0);
                    oPayment.PrimaryFormItems.CashFlowLineItemID = int.Parse(data.Cashflow);
                    if (data.Currency != GlobalsConfig.Instance.LocalCurrencyDefault)
                        oPayment.PrimaryFormItems.AmountFC = (double)data.Amount;
                    else
                        oPayment.PrimaryFormItems.AmountLC = (double)data.Amount;
                    oPayment.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;

                }

                var ret1 = -1;
                ret1 = oPayment.Add();
                if (ret1 != 0)
                {
                    DIConnection.Instance.Company.GetLastError(out lErrCode, out message);
                    result.Flag = false;
                    result.Message = message;// + " " + DIConnection.Instance.Company.GetLastErrorDescription(); ;
                    //results.Add(result);
                }
                else
                {
                    string newId = string.Empty;
                    DIConnection.Instance.Company.GetNewObjectCode(out newId);
                    result.Flag = true;
                    result.Message = $"Tạo thành đơn id {newId}";
                    //results.Add(result);
                }

            }
            catch (Exception ex)
            {
                message = string.Format("Error {0}", ex.Message);
                result = null;
            }
            DisConnectDI();
            return result;
        }



        //public static List<PaymentApproveResult> ApprovePayment(List<int> datas, ApprovalStatus status, string remark, ref string message)
        //{
        //    if (!ConnectDI(ref message))
        //        return null;

        //    var results = new List<PaymentApproveResult>();
        //    var notice = string.Empty;
        //    if (status == ApprovalStatus.Generate)
        //        notice = "Tạo";
        //    else if (status == ApprovalStatus.Approve)
        //        notice = "Duyệt";
        //    else if (status == ApprovalStatus.Reject)
        //        notice = "Từ chối";
        //    else
        //        notice = "Chuyển về chờ duyệt";

        //    try
        //    {
        //        foreach (var docentry in datas)
        //        {
        //            PaymentApproveResult result = new PaymentApproveResult();

        //            result.DocNum = docentry.ToString();
        //            ApprovalRequestsService oApprovalRequestsService = (ApprovalRequestsService)DIConnection.Instance.Company.GetCompanyService().GetBusinessService(ServiceTypes.ApprovalRequestsService);

        //            var approvalRequestParams = (SAPbobsCOM.ApprovalRequestParams)oApprovalRequestsService.GetDataInterface(SAPbobsCOM.ApprovalRequestsServiceDataInterfaces.arsApprovalRequestParams);

        //            var code = string.Empty;
        //            Hashtable data;
        //            using (var connection = CoreSetting.DataConnection)
        //            {
        //                data = connection.ExecQueryToHashtable(string.Format(QueryString.GetApproveRequestCodeQuery, docentry.ToString()));
        //                connection.Dispose();
        //            }

        //            if (data != null)
        //            {
        //                code = data["WddCode"].ToString();
        //            }

        //            if (string.IsNullOrEmpty(code))
        //            {
        //                result.Flag = false;
        //                result.Message = $"Không tìm thấy mã chứng từ nháp để {notice}";
        //                results.Add(result);
        //                continue;
        //            }
        //            var intCode = int.Parse(code);
        //            approvalRequestParams.Code = intCode;
        //            var approvalRequest = oApprovalRequestsService.GetApprovalRequest(approvalRequestParams);

        //            if (status != ApprovalStatus.Generate && status != ApprovalStatus.Pending)
        //            {
        //                try
        //                {
        //                    approvalRequest.ApprovalRequestDecisions.Add();
        //                    approvalRequest.ApprovalRequestDecisions.Item(0).Remarks = remark;
        //                    var statusApp = BoApprovalRequestDecisionEnum.ardPending;
        //                    if (status == ApprovalStatus.Approve)
        //                        statusApp = BoApprovalRequestDecisionEnum.ardApproved;
        //                    else if (status == ApprovalStatus.Reject)
        //                        statusApp = BoApprovalRequestDecisionEnum.ardNotApproved;
        //                    approvalRequest.ApprovalRequestDecisions.Item(0).Status = statusApp;
        //                    oApprovalRequestsService.UpdateRequest(approvalRequest);
        //                    result.Flag = true;
        //                    result.Message = $"{notice} thành công";
        //                    results.Add(result);
        //                }
        //                catch (Exception ex)
        //                {
        //                    result.Flag = false;
        //                    result.Message = $"{notice} lỗi {ex.Message}";
        //                    results.Add(result);
        //                }
        //            }
        //            else if (status == ApprovalStatus.Generate)
        //            {
        //                try
        //                {
        //                    var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
        //                    if (oPayment.GetByKey(approvalRequest.DraftEntry))
        //                    {
        //                        var saveret = oPayment.SaveDraftToDocument();
        //                        if (saveret != 0)
        //                        {
        //                            var mess = DIConnection.Instance.Company.GetLastErrorDescription();
        //                            result.Flag = false;
        //                            result.Message = $"{notice} lỗi {mess}";
        //                            results.Add(result);
        //                        }
        //                        else
        //                        {
        //                            result.Flag = true;
        //                            result.Message = $"{notice} thành công";
        //                            results.Add(result);
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    result.Flag = false;
        //                    result.Message = $"{notice} lỗi {ex.Message}";
        //                    results.Add(result);
        //                }
        //            }
        //            else if (status == ApprovalStatus.Pending)
        //            {
        //                try
        //                {
        //                    approvalRequest.ApprovalRequestDecisions.Add();
        //                    approvalRequest.ApprovalRequestDecisions.Item(0).Remarks = remark;
        //                    var statusApp = BoApprovalRequestDecisionEnum.ardPending;

        //                    approvalRequest.ApprovalRequestDecisions.Item(0).Status = statusApp;
        //                    oApprovalRequestsService.UpdateRequest(approvalRequest);
        //                    result.Flag = true;
        //                    result.Message = $"{notice} thành công";
        //                    results.Add(result);

        //                    var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
        //                    if (oPayment.GetByKey(approvalRequest.DraftEntry))
        //                    {
        //                        oPayment.UserFields.Fields.Item("U_Review").Value = "Y";
        //                        var saveret = oPayment.Update();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    result.Flag = false;
        //                    result.Message = $"{notice} lỗi {ex.Message}";
        //                    results.Add(result);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        message = string.Format("Error {0}", ex.Message);
        //        results = null;
        //    }
        //    DisConnectDI();
        //    return results;
        //}
        public static List<PaymentApproveResult> ApprovePayment1(List<int> datas, ApprovalStatus status, string remark, ref string message)
        {
            if (!ConnectDI(ref message))
                return null;

            var results = new List<PaymentApproveResult>();
            var notice = string.Empty;
            if (status == ApprovalStatus.Generate)
                notice = "Tạo";
            else if (status == ApprovalStatus.Approve)
                notice = "Duyệt";
            else if (status == ApprovalStatus.Reject)
                notice = "Từ chối";
            else
                notice = "Chuyển về chờ duyệt";

            try
            {
                foreach (var docentry in datas)
                {
                    PaymentApproveResult result = new PaymentApproveResult();

                    result.DocNum = docentry.ToString();
                    if (status != ApprovalStatus.Generate && status != ApprovalStatus.Pending)
                    {
                        try
                        {
                            var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                            if (oPayment.GetByKey(docentry))
                            {
                                oPayment.UserFields.Fields.Item("U_Status").Value = status == ApprovalStatus.Approve ? "A" : "J";
                                var saveret = oPayment.Update();

                                result.Flag = true;
                                result.Message = $"{notice} thành công";
                                results.Add(result);

                            }
                        }
                        catch (Exception ex)
                        {
                            result.Flag = false;
                            result.Message = $"{notice} lỗi {ex.Message}";
                            results.Add(result);
                        }
                    }
                    else if (status == ApprovalStatus.Generate)
                    {
                        try
                        {
                            var oPaymntDraf = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                            var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oVendorPayments);
                            if (oPaymntDraf.GetByKey(docentry))
                            {
                                oPayment.CardCode = oPaymntDraf.CardCode;
                                oPayment.DocCurrency = oPaymntDraf.DocCurrency;
                                oPayment.DocRate = oPaymntDraf.DocRate;
                                oPayment.DocDate = oPaymntDraf.DocDate;
                                oPayment.BPLID = oPaymntDraf.BPLID;

                                for (int i = 0; i < oPaymntDraf.Invoices.Count; i++)
                                {
                                    oPaymntDraf.Invoices.SetCurrentLine(i);

                                    if (i != 0)
                                    {
                                        oPayment.Invoices.Add();
                                        oPayment.Invoices.SetCurrentLine(i);
                                    }
                                    oPayment.Invoices.DocEntry = oPaymntDraf.Invoices.DocEntry;
                                    oPayment.Invoices.InvoiceType = oPaymntDraf.Invoices.InvoiceType;
                                    oPayment.Invoices.SumApplied = oPaymntDraf.Invoices.SumApplied;
                                    oPayment.Invoices.AppliedFC = oPaymntDraf.Invoices.AppliedFC;
                                }
                                oPayment.CashSum = oPaymntDraf.CashSum;
                                oPayment.CashAccount = oPaymntDraf.CashAccount;
                                oPayment.TransferSum = oPaymntDraf.TransferSum;
                                oPayment.TransferAccount = oPaymntDraf.TransferAccount;
                                oPayment.UserFields.Fields.Item("U_BANK").Value = oPaymntDraf.UserFields.Fields.Item("U_BANK").Value;

                                if (oPayment.PrimaryFormItems.Count > 0)
                                {
                                    for (int i = 0; i < oPaymntDraf.PrimaryFormItems.Count; i++)
                                    {
                                        oPaymntDraf.PrimaryFormItems.SetCurrentLine(i);
                                        if (i != 0)
                                        {
                                            oPayment.PrimaryFormItems.Add();
                                        }
                                        oPayment.PrimaryFormItems.SetCurrentLine(i);
                                        oPayment.PrimaryFormItems.CashFlowLineItemID = oPaymntDraf.PrimaryFormItems.CashFlowLineItemID;
                                        oPayment.PrimaryFormItems.AmountFC = oPaymntDraf.PrimaryFormItems.AmountFC;
                                        oPayment.PrimaryFormItems.AmountLC = oPaymntDraf.PrimaryFormItems.AmountLC;
                                        oPayment.PrimaryFormItems.PaymentMeans = oPaymntDraf.PrimaryFormItems.PaymentMeans;
                                    }
                                }

                                oPayment.UserFields.Fields.Item("U_PayType").Value = oPaymntDraf.UserFields.Fields.Item("U_PayType").Value;
                                oPayment.UserFields.Fields.Item("U_Review").Value = "Y";
                                oPayment.UserFields.Fields.Item("U_PaymentKey").Value = oPaymntDraf.UserFields.Fields.Item("U_PaymentKey").Value;

                                oPayment.UserFields.Fields.Item("U_Status").Value = "S";

                                var befStatus = oPaymntDraf.UserFields.Fields.Item("U_Status").Value;
                                oPaymntDraf.UserFields.Fields.Item("U_Status").Value = "S";
                                var ret1 = oPaymntDraf.Update();
                                //var ret1 = -1;
                                if (ret1 == 0)
                                {
                                    var saveret = oPayment.Add();

                                    // var saveret = oPaymntDraf.SaveDraftToDocument();
                                    if (saveret != 0)
                                    {
                                        var mess = DIConnection.Instance.Company.GetLastErrorDescription();
                                        result.Flag = false;
                                        result.Message = $"{notice} lỗi {mess}";
                                        results.Add(result);

                                        oPaymntDraf.UserFields.Fields.Item("U_Status").Value = befStatus;
                                        ret1 = oPaymntDraf.Update();
                                    }
                                    else
                                    {
                                        var code = string.Empty;
                                        DIConnection.Instance.Company.GetNewObjectCode(out code);
                                        result.Flag = true;
                                        result.Message = $"{notice} thành công với mã {code}";
                                        results.Add(result);

                                        //if (oPaymntDraf.GetByKey(docentry))
                                        //{
                                        //    oPaymntDraf.UserFields.Fields.Item("U_Status").Value = "S";
                                        //   // saveret = oPaymntDraf.Cancel();
                                        //    saveret = oPaymntDraf.Update();
                                        //}
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result.Flag = false;
                            result.Message = $"{notice} lỗi {ex.Message}";
                            results.Add(result);
                        }
                    }
                    else if (status == ApprovalStatus.Pending)
                    {
                        try
                        {
                            var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                            if (oPayment.GetByKey(docentry))
                            {
                                oPayment.UserFields.Fields.Item("U_Status").Value = "N";
                                var saveret = oPayment.Update();
                                result.Flag = true;
                                result.Message = $"{notice} thành công với ";
                                results.Add(result);

                            }
                        }
                        catch (Exception ex)
                        {
                            result.Flag = false;
                            result.Message = $"{notice} lỗi {ex.Message}";
                            results.Add(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("Error {0}", ex.Message);
                results = null;
            }
            DisConnectDI();
            return results;
        }
        public static bool ReviewPayment(int data, ref string message)
        {
            if (!ConnectDI(ref message))
                return false;

            var ret = false;

            try
            {
                var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                if (oPayment.GetByKey(data))
                {
                    //oPayment.UserFields.Fields.Item("U_Review").Value = "Y";
                    oPayment.UserFields.Fields.Item("U_Status").Value = "R";

                    var saveret = oPayment.Update();
                    if (saveret != 0)
                    {
                        message = DIConnection.Instance.Company.GetLastErrorDescription();
                        ret = false;
                    }
                    else
                    {
                        message = STRING_CONTRANTS.Notice_UpdatePaymentSuccess;
                        ret = true;
                    }
                }
                else
                {
                    ret = false;
                    message = STRING_CONTRANTS.Notice_CanNotloadDraft;
                }

            }
            catch (Exception ex)
            {
                ret = false;
                message = ex.Message;
            }
            DisConnectDI();
            return ret;
        }
    }
}
