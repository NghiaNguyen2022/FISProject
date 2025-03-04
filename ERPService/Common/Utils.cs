using APIHandler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ERPService.Common
{
    internal class Utils
    {
        public static void WriteToFile(string Message, string file = "ServiceLog_")
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var filepath = AppDomain.CurrentDomain.BaseDirectory + $"\\Logs\\{file}" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!System.IO.File.Exists(filepath))
            {
                // Create a file to write to.
                using (var sw = System.IO.File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (var sw = System.IO.File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        public static void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                var timeRun1String = ConfigurationManager.AppSettings["timeRun1"];
                var timeRun2String = ConfigurationManager.AppSettings["timeRun2"];
                var timeRun1 = TimeSpan.Parse(timeRun1String);
                var timeRun2 = TimeSpan.Parse(timeRun2String);

                DateTime now = DateTime.Now;
                TimeSpan currentTime = now.TimeOfDay;
                DateTime fromTime, toTime;

                if (GlobalConfig.InquiryRunner != null)
                {
                    if(GlobalConfig.InquiryRunner.RunDate.Date != now.Date)
                    {
                        GlobalConfig.InquiryRunner.RunDate = now;
                        GlobalConfig.InquiryRunner.Timer = 0;                        
                    }
                    
                    // Match the current time to the configured times
                    if (currentTime.Hours == timeRun1.Hours && currentTime.Minutes >= timeRun1.Minutes 
                        && GlobalConfig.InquiryRunner.Timer == 0)
                    {
                        // timeRun1 match: from = timeRun2 of yesterday, to = timeRun1 of today
                        fromTime = DateTime.Today.AddDays(-1).Add(timeRun2); // Yesterday's timeRun2
                        toTime = DateTime.Today.Add(timeRun1);              // Today's timeRun1
                        Console.WriteLine($"Matched timeRun1: From {fromTime} to {toTime}");

                        //VTBHandler.CallVTBAPI()

                        GlobalConfig.InquiryRunner.Timer = 1;
                    }
                    else if (currentTime.Hours == timeRun2.Hours && currentTime.Minutes >= timeRun2.Minutes
                        && GlobalConfig.InquiryRunner.Timer == 1)
                    {
                        // timeRun2 match: from = timeRun1 of today, to = timeRun2 of today
                        fromTime = DateTime.Today.Add(timeRun1);            // Today's timeRun1
                        toTime = DateTime.Today.Add(timeRun2);              // Today's timeRun2
                        Console.WriteLine($"Matched timeRun2: From {fromTime} to {toTime}");
                        GlobalConfig.InquiryRunner.Timer = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteToFile($"Error catching runTime and generate fromDate toDate: {ex.Message}");
                return;
            }
        }

    }
}
