    using System;

namespace PN.SmartLib.CustomAttribute
{
    public class APIResponseAttribute:Attribute
    {
        public APIResponseAttribute(string code, string description)
        {
            Code = code;
            Description = description;
        }
        public APIResponseAttribute(string code)
        {
            Code = code;
            Description = string.Empty;
        }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
