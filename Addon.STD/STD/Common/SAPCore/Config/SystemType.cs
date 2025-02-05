using System.ComponentModel;

namespace SAPCore.Config
{
    /// <summary>
    /// Enum: Define System type; HANA or SQL with each version
    /// </summary>
    public enum SystemType
    {
        [Description("SAP_SQL_2014")]
        SAP_SQL_2014,

        [Description("SAP_SQL_2016")]
        SAP_SQL_2016,

        [Description("SAP_SQL_2017")]
        SAP_SQL_2017,

        [Description("SAP_SQL_2019")]
        SAP_SQL_2019,

        [Description("SAP_HANA")]
        SAP_HANA
    }

    public enum SystemLang
    {
        [Description("English")]
        English,

        [Description("Vietnamese")]
        Vietnamese,

        [Description("Japanese")]
        Japanese,
    }
}
