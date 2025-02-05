using SAPbouiCOM;
using SAPCore;
using STD.Models;
using STDApp.DataReader;
using System.Collections.Generic;

namespace STDApp
{
    public class ViewHelper
    {
        private static List<Branch> branches;
        private static List<Bank1> banks;
        private static List<CashFlow> cashFlows;
        private static List<Account> accounts;
        public static List<Branch> Branches
        {
            get
            {
                if (branches == null || branches.Count == 0)
                {
                    var datas = DataHelper.LoadBranches();
                    branches = new List<Branch>();
                    foreach (var data in datas)
                    {
                        branches.Add(new Branch()
                        {
                            BPLId = data["BPLId"].ToString(),
                            BPLName = data["BPLName"].ToString(),
                            AliasName = data["AliasName"].ToString()
                        });
                    }
                }
                return branches;
            }
        }

        public static List<Bank1> Banks
        {
            get
            {
                if (banks == null || banks.Count <= 0)
                {
                    var datas = DataHelper.LoadBanks();
                    banks = new List<Bank1>();
                    foreach (var data in datas)
                    {
                        banks.Add(new Bank1()
                        {
                            //RowNumer = data["RowNum"].ToString(),
                            Code = data["Code"].ToString(),
                            Name = data["Name"].ToString(),
                            Account = data["U_Account"].ToString()
                        });
                    }
                }
                return banks;
            }
        }
        public static List<CashFlow> CashFlows
        {
            get
            {
                if (cashFlows == null || cashFlows.Count <= 0)
                {
                    var datas = DataHelper.ListCashFlows;
                    cashFlows = new List<CashFlow>();
                    foreach (var data in datas)
                    {
                        cashFlows.Add(new CashFlow()
                        {
                            Id = data["CFWId"].ToString(),
                            Name = data["CFWName"].ToString()
                        });
                    }
                }
                return cashFlows;
            }
        }

        public static List<Account> Accounts
        {
            get
            {
                if (accounts == null || accounts.Count <= 0)
                {
                    var datas = DataHelper.LoadAccounts();
                    accounts = new List<Account>();
                    foreach (var data in datas)
                    {
                        accounts.Add(new Account()
                        {
                            Code = data["AcctCode"].ToString(),
                            Name = data["AcctName"].ToString()
                        });
                    }
                }
                return accounts;
            }
        }

        public static void LoadBranchesToCombobox(ComboBox comboBox)
        {
            
            UIHelper.ClearSelectValidValues(comboBox);
            //for (var i = comboBox.ValidValues.Count - 1; i >= 0; i--)
            //{
            //    comboBox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
            //}
            if (Branches != null && Branches.Count > 0)
            {
                foreach (var data in Branches)
                {
                    comboBox.ValidValues.Add(data.BPLId, data.BPLName);
                }

                UIHelper.ComboboxSelectDefault(comboBox);
                //comboBox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                //comboBox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
                //comboBox.Item.DisplayDesc = true;
            }
        }

        public static void AddCustomerColumnInGrid(ComboBoxColumn combobox)
        {
            var datas = DataHelper.ListCustomerInSAP;
            //for (var i = combobox.ValidValues.Count - 1; i >= 0; i--)
            //{
            //    combobox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
            //}
            UIHelper.ClearSelectValidValues(combobox);
            if (datas != null && datas.Length > 0)
            {
                foreach (var data in datas)
                {
                    var cardCode = data["CardCode"].ToString();
                    var cardName = data["CardName"].ToString();

                    combobox.ValidValues.Add(cardCode, cardName);
                }

            }
        }
        public static void AddCurrencyColumnInGrid(ComboBoxColumn combobox)
        {
            var datas = DataHelper.ListCurrencyInSAP;
            //for (var i = combobox.ValidValues.Count - 1; i >= 0; i--)
            //{
            //    combobox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
            //}

            UIHelper.ClearSelectValidValues(combobox);
            if (datas != null && datas.Length > 0)
            {
                foreach (var data in datas)
                {
                    var cardCode = data["CurrCode"].ToString();
                    var cardName = data["CurrName"].ToString();

                    combobox.ValidValues.Add(cardCode, cardName);
                }

            }
        }

        //public stat

        public static void ColorGridRows(SAPbouiCOM.Grid grid, int index, bool noselect = false)
        {
            //SAPbouiCOM.RowHeaders rowHeaders = grid.RowHeaders;
            //for (var rowIndex = 0; rowIndex < grid.Rows.Count; rowIndex++)
            //{
            //    var commonSetting = grid.CommonSetting;
            //    var hex = "#e7e7e7";
            //    Color _color = System.Drawing.ColorTranslator.FromHtml(hex);
            //    c.SetRowBackColor(rowIndex + 1, System.Drawing.ColorTranslator.ToOle(_color));

            //    for (var colIndex = 0; colIndex < grid.Columns.Count; colIndex++)
            //    {
            //        if (commonSetting.GetCellEditable(rowIndex + 1, colIndex + 1))
            //        {
            //            commonSetting.SetCellBackColor(rowIndex + 1, colIndex + 1, System.Drawing.Color.White.ToArgb());
            //        }

            //    }// disable E7E7E7
            //    //if(commonSetting.get)
            //}

            //if (!noselect)
            //{
            //    var selectedSetting = grid.CommonSetting;
            //    var hex = "#fcdd82";
            //    Color _color = System.Drawing.ColorTranslator.FromHtml(hex);
            //    selectedSetting.SetRowBackColor(index + 1, System.Drawing.ColorTranslator.ToOle(_color));
            //}
        }
        //public static void LoadBanksToCombobox(ComboBoxColumn comboBox)
        //{
        //    UIHelper.ClearSelectValidValues(comboBox);
        //    //for (var i = comboBox.ValidValues.Count - 1; i >= 0; i--)
        //    //{
        //    //    comboBox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
        //    //}
        //    //comboBox.ValidValues.Add("-", "Tất cả");
        //    if (Banks != null && Banks.Count > 0)
        //    {
        //        foreach (var data in Banks)
        //        {
        //            comboBox.ValidValues.Add(data.RowNumer, data.Name);
        //        }

        //        comboBox.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both;
               
        //    }
        //}
        public static void LoadCashFlowsToCombobox(ComboBoxColumn comboBox)
        {
            UIHelper.ClearSelectValidValues(comboBox);
            //for (var i = comboBox.ValidValues.Count - 1; i >= 0; i--)
            //{
            //    comboBox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
            //}
            //comboBox.ValidValues.Add("-", "Tất cả");
            if (CashFlows != null && CashFlows.Count > 0)
            {
                foreach (var data in CashFlows)
                {
                    comboBox.ValidValues.Add(data.Id, data.Name);
                }

                comboBox.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription;
                comboBox.DisplayType = BoComboDisplayType.cdt_both;
            }
        }
    }
}
