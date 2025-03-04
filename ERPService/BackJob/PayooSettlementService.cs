using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using APIHandler;
using ERPService.Common;

namespace ERPService.BackJob
{
    partial class PayooSettlementService : ServiceBase
    {
        Timer timer = new Timer();
        public PayooSettlementService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Utils.WriteToFile($"On Start Settlement");
                run();
            }
            catch (FormatException ex)
            {
                Utils.WriteToFile($"Error: {ex.Message}");
                return;
            }
        }
        protected override void OnStop()
        {
            Utils.WriteToFile($"On Stop Settlement");
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
        private void run()
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 10 * 60000;
            timer.Enabled = true;
            timer.Start();
        }

        public void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {

                if (GlobalConfig.PayooRunner == null)
                    GlobalConfig.PayooRunner = new RunnerTime();

                var timeRun1String = ConfigurationManager.AppSettings["timeRun1"];
                var timeRun2String = ConfigurationManager.AppSettings["timeRun2"];
                var timeRun1 = TimeSpan.Parse(timeRun1String);
                var timeRun2 = TimeSpan.Parse(timeRun2String);

                DateTime now = DateTime.Now;
                TimeSpan currentTime = now.TimeOfDay;
                var message = string.Empty;
                if (GlobalConfig.PayooRunner != null)
                {
                    if (GlobalConfig.PayooRunner.RunDate.Date != now.Date)
                    {
                        GlobalConfig.PayooRunner.RunDate = now;
                        GlobalConfig.PayooRunner.Timer = 0;
                    }

                    Utils.WriteToFile($"Settlement {GlobalConfig.InquiryRunner.RunDate } - {GlobalConfig.InquiryRunner.Timer}");
                    // Match the current time to the configured times
                    if (currentTime.Hours == timeRun1.Hours && currentTime.Minutes >= timeRun1.Minutes
                        && GlobalConfig.PayooRunner.Timer == 0)
                    {
                        APIJobHandler.CallPayooAPI(now.ToString("yyyyMMdd"), 1, ref message);
                        Utils.WriteToFile($"{message}");
                        GlobalConfig.PayooRunner.Timer = 1;
                    }
                    else if (currentTime.Hours == timeRun2.Hours && currentTime.Minutes >= timeRun2.Minutes
                        && GlobalConfig.InquiryRunner.Timer == 1)
                    {
                        APIJobHandler.CallPayooAPI(now.ToString("yyyyMMdd"), 2, ref message);
                        Utils.WriteToFile($"{message}");
                        GlobalConfig.PayooRunner.Timer = 2;
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
