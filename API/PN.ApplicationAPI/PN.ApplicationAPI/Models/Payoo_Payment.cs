using SAPCore.Config;
using STD.DataReader;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace PN.ApplicationAPI.Models
{
    public class PaypooRequest
    {
        public string ResponseData { get; set; }
        public string SecureHash { get; set; }
    }

    public class Payoo_Order
    {
        // public string OrderParentNo { get; set; }
        public string Seller { get; set; }
        public decimal Shop { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public string OrderNo { get; set; }
        public object ChildOrders { get; set; }

        public bool InsertData(string parent, ref string message)
        {
            try
            {
                var dbName = ConfigurationManager.AppSettings["Schema"];
                var query = "INSERT INTO \"" + dbName + "\".\"tb_Payoo_PaymentOrder\" VALUES ( ";
                query += $"'{parent}',";
                query += $"'{Seller}',";
                query += $"{Shop},";
                query += $"'{Description}',";
                query += $"{Total},";
                query += $"'{OrderNo}'";
                query += ")";

                var ret1 = dbProvider.ExecuteNonQuery(query);
                if (ret1 == 1)
                {
                    message = "Lưu thành công";

                    query = "UPDATE \"" + dbName + "\".OINV SET \"U_PayooMark\" = 'PAYOOQR' WHERE \"DocNum\" = " + OrderNo;
                    var ret11 = dbProvider.ExecuteNonQuery(query);

                    return true;
                }
                else
                {
                    message = "Lưu thất bại";
                }
                return false;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
    public class Payoo_Payment
    {
        public int PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; }
        public string PurDate { get; set; }
        public string MerchantUserName { get; set; }
        public decimal ShopId { get; set; }
        public decimal MasterShopID { get; set; }
        public string OrderNo { get; set; }
        public decimal OrderCash { get; set; }
        public string BankName { get; set; }
        public string CardNumber { get; set; }
        public int CardIssuanceType { get; set; }
        public int PaymentStatus { get; set; }
        public string MDD1 { get; set; }
        public string MDD2 { get; set; }
        public string PaymentSource { get; set; }
        public decimal VoucherTotalAmount { get; set; }
        public string VoucherID { get; set; }
        public int PaymentMethodSub { get; set; }
        public List<Payoo_Order> Orders { get; set; }

        public bool InsertData(ref string message)
        {
            //using(var trans)
            try
            {
                var dbName = ConfigurationManager.AppSettings["Schema"];
                var query = "INSERT INTO \"" + dbName + "\".\"tb_Payoo_PaymentINT\" VALUES ( ";
                query += $"{PaymentMethod},";
                query += $"'{PaymentMethodName}',";
                query += $"'{PurDate}',";
                query += $"'{MerchantUserName}',";
                query += $"{ShopId},";
                query += $"{MasterShopID},";
                query += $"'{OrderNo}',";
                query += $"{OrderCash},";
                query += $"'{BankName}',";
                query += $"'{CardNumber}',";
                query += $"{CardIssuanceType},";
                query += $"{PaymentStatus},";
                query += $"'{MDD1}',";
                query += $"'{MDD2}',";
                query += $"'{PaymentSource}',";
                query += $"{VoucherTotalAmount},";
                query += $"'{VoucherID}',";
                query += $"'{DateTime.Now.ToString("yyyyMMdd")}',";
                query += $"'{DateTime.Now.ToString("HHmmss")}',";
                query += $"{PaymentMethodSub}";
                query += ")";

                var ret = -1;

                if (ShopId.ToString() == ConfigurationManager.AppSettings["ShopID"].ToString())
                {
                    if (Orders != null)
                    {
                        //using (DBTransaction)
                        var message1 = string.Empty;
                        var subret = true;
                        foreach (var order in Orders)
                        {
                            message1 = string.Empty;
                            subret = order.InsertData(OrderNo, ref message1);
                            if (!subret)
                            {
                                break;
                            }
                        }
                        if (subret)
                        {
                            ret = dbProvider.ExecuteNonQuery(query);
                            message = ret == 1 ? "Lưu thành công" : "Lưu thất bại";
                        }
                        else
                        {
                            message = message1;
                        }
                    }
                }
                else
                {
                    ret = dbProvider.ExecuteNonQuery(query);
                    message = ret == 1 ? "Lưu thành công" : "Lưu thất bại";
                    if (ret == 1)
                    {
                        query = "UPDATE \"" + dbName + "\".OINV SET \"U_PayooMark\" = 'PAYOOQR' WHERE \"DocNum\" = " + OrderNo;
                        var ret1 = dbProvider.ExecuteNonQuery(query);
                    }
                }
                //var ret1 = dbProvider.ExecuteNonQuery(query);
                //if(ret1 == 1)
                //{
                //    message = "Lưu thành công";
                //    query = "UPDATE \"" + dbName + "\".OINV SET \"U_PayooMark\" = 'PAYOOQR' WHERE \"DocNum\" = " + OrderNo;
                //    ret1 = dbProvider.ExecuteNonQuery(query);

                //    if(ShopId.ToString() == ConfigurationManager.AppSettings["ShopID"].ToString())
                //    {

                //    }

                //    return true;
                //}
                //else
                //{
                //    message = "Lưu thất bại";
                //}
                return ret == 1;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}