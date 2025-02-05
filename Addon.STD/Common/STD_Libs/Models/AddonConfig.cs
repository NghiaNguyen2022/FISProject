using SAPCore.SAP.DIAPI;
using STD.DataReader;
using System.Collections;
using System.Collections.Generic;

namespace STDLibs.Models
{
    public class AddonConfig
    {
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public AddonConfig(Hashtable data)
        {
            ConfigKey = data["ConfigKey"].ToString();
            ConfigValue = data["ConfigValue"].ToString();
        }
    }

    public class Configs
    {
        //private static List<AddonConfig> addonConfigs;

        //public static List<AddonConfig> AddonConfigs
        //{
        //    get
        //    {
        //        if (addonConfigs == null || addonConfigs.Count == 0)
        //        {
        //            addonConfigs = new List<AddonConfig>();
        //            var query = "SELECT * FROM \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Addon_Config\"";
        //            var datas = dbProvider.QueryList(query);
        //            if (datas != null && datas.Length > 0)
        //            {
        //                foreach (var data in datas)
        //                    addonConfigs.Add(new AddonConfig(data));
        //            }
        //        }
        //        return addonConfigs;
        //    }
        //}

        private static Dictionary<string, string > _addonConfigs;

        public static Dictionary<string, string> AddonConfigurations
        {
            get
            {
                if (_addonConfigs == null || _addonConfigs.Count == 0)
                {
                    _addonConfigs = new Dictionary<string, string>();
                    var query = "SELECT * FROM \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Addon_Config\"";
                    var datas = dbProvider.QueryList(query);
                    if (datas != null && datas.Length > 0)
                    {
                        foreach (var data in datas)
                        {
                            var key = data["ConfigKey"].ToString();
                            if (!_addonConfigs.ContainsKey(key))
                            {
                                var value = data["ConfigValue"];
                                var valstri = value.ToString();
                                //if (value != null)
                                //    valstri = value.ToString();
                                _addonConfigs.Add(data["ConfigKey"].ToString(), valstri);
                            }
                        }
                    }
                }
                return _addonConfigs;
            }
        }
    }
}
