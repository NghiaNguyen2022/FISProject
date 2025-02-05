using System;

namespace PN.SmartLib.CustomAttribute
{
    public class CustomField : Attribute
    {
        public string Name { get; set; }
        public string DataType { get; set; }

        public CustomField(string name, string dataType)
        {
            Name = name;
            DataType = dataType;
        }
    }
}
