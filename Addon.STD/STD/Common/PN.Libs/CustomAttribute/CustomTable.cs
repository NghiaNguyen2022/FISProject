using System;

namespace PN.SmartLib.CustomAttribute
{
    public class CustomTable : Attribute
    {
        public string Name { get; set; }

        public CustomTable(string name)
        {
            Name = name;
        }
    }
}
