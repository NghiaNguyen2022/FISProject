using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Models
{
    public class ERPDocument
    {
        public string Objtype { get; set; }
        public int DocEntry { get; set; }
        public string DocNum { get; set; }

        public ERPDocument(Hashtable data)
        {
            try
            {
                Objtype = data["ObjType"].ToString();
                DocNum = data["DocNum"].ToString();
                DocEntry = int.Parse(data["DocEntry"].ToString());
            }
            catch (Exception ex)
            {

            }
        }
    }
}
