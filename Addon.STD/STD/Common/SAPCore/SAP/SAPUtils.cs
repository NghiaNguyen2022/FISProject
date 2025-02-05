using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPCore.SAP
{
    public class SAPUtils
    {
        public static void StopAddon(string addonName, ref string message)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(addonName))
                {
                    clsProcess.Kill();
                }
            }
            System.Windows.Forms.Application.Exit();
            message = "CompanyChanged event caught Addon stopped";
        }

    }
}
