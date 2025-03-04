using APIHandler;
using ERPService.Common;
using ERPService.DataReader;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace ERPService.BackJob
{
    public class BackgroundJob : ServiceBase
    {
        Timer timerClear = new Timer();
        Timer timerInquiry = new Timer();
        Timer timerSettlement = new Timer();
        int Interval
        {
            get
            {
                int timer = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["Interval"], out timer))
                    return timer;
                return 1000;
            }
        }
        protected override void OnStop()
        {

            Utils.WriteToFile($"On Stop");
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Utils.WriteToFile($"On Start");
                System.Threading.Thread thread1 = new System.Threading.Thread(new System.Threading.ThreadStart(runInQuiry));
                System.Threading.Thread thread2 = new System.Threading.Thread(new System.Threading.ThreadStart(runSettlement));
                System.Threading.Thread thread3 = new System.Threading.Thread(new System.Threading.ThreadStart(runClear));

                //Test 
                if (ConfigurationManager.AppSettings["Test"].ToString() == "Y")
                {
                    System.Threading.Thread thread4 = new System.Threading.Thread(new System.Threading.ThreadStart(run));
                    thread4.Start();
                }
                thread1.Start();
                thread2.Start();
                thread3.Start();

                //run();
            }
            catch (FormatException ex)
            {
                Utils.WriteToFile($"Error: {ex.Message}");
                return;
            }
        }

        private void runClear()
        {
            Utils.WriteToFile($"On Start Clear", "Clear");
            timerClear.Elapsed += new ElapsedEventHandler(OnElapsedClear);
            timerClear.Interval = Interval;
            timerClear.Enabled = true;
            timerClear.Start();
        }

        public void OnElapsedClear(object source, ElapsedEventArgs e)
        {
            try
            {

                Utils.WriteToFile($"Clear", "Clear");

                if (GlobalConfig.ClearRunner == null)
                    GlobalConfig.ClearRunner = new RunnerTime();

                var timeRun1String = "12:40";
                var timeRun2String = "17:40";
                var timeRun1 = TimeSpan.Parse(timeRun1String);
                var timeRun2 = TimeSpan.Parse(timeRun2String);

                Utils.WriteToFile($"timeRun1:{timeRun1}", "Clear");
                Utils.WriteToFile($"timeRun2:{timeRun2}", "Clear");

                DateTime now = DateTime.Now;
                TimeSpan currentTime = now.TimeOfDay;

                Utils.WriteToFile($"Clear2", "Clear");

                Utils.WriteToFile($"currentTime:{currentTime}", "Clear");

                var message = string.Empty;
                if (GlobalConfig.ClearRunner != null)
                {
                    if (GlobalConfig.ClearRunner.RunDate.Date != now.Date)
                    {
                        GlobalConfig.ClearRunner.RunDate = now;
                        GlobalConfig.ClearRunner.Timer = 0;
                    }

                    Utils.WriteToFile($"Clear {GlobalConfig.ClearRunner.RunDate } - {GlobalConfig.ClearRunner.Timer}", "Clear");
                    // Match the current time to the configured times
                    if (currentTime.Hours == timeRun1.Hours && currentTime.Minutes >= timeRun1.Minutes
                        && GlobalConfig.ClearRunner.Timer == 0)
                    {          // Today's timeRun1
                        var batch = $"SP{now.ToString("yyMMdd")}01";
                        Utils.WriteToFile($"Run: {now} batch {batch}", "Clear");

                        APIJobHandler.Clear(batch, ref message);
                        Utils.WriteToFile($"{message}", "Clear");
                        GlobalConfig.ClearRunner.Timer = 1;
                    }
                    else if (currentTime.Hours == timeRun2.Hours && currentTime.Minutes >= timeRun2.Minutes
                        && GlobalConfig.ClearRunner.Timer == 1)
                    {

                        var batch = $"SP{now.ToString("yyMMdd")}02";
                        Utils.WriteToFile($"Run: {now} batch {batch}", "Clear");

                        APIJobHandler.Clear(batch, ref message);
                        Utils.WriteToFile($"{message}", "Clear");
                        GlobalConfig.ClearRunner.Timer = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteToFile($"Clear: {ex.Message}", "Clear");
                return;
            }
        }

        private void runInQuiry()
        {
            Utils.WriteToFile($"On Start Inquiry", "InQuiry");
            timerInquiry.Elapsed += new ElapsedEventHandler(OnElapsedInquiry);
            timerInquiry.Interval = Interval;
            timerInquiry.Enabled = true;
            timerInquiry.Start();
        }

        public void OnElapsedInquiry(object source, ElapsedEventArgs e)
        {
            try
            {

                if (GlobalConfig.InquiryRunner == null)
                    GlobalConfig.InquiryRunner = new RunnerTime();

                var timeRun1String = ConfigurationManager.AppSettings["timeRun1"];
                var timeRun2String = ConfigurationManager.AppSettings["timeRun2"];
                var timeRun1 = TimeSpan.Parse(timeRun1String);
                var timeRun2 = TimeSpan.Parse(timeRun2String);

                DateTime now = DateTime.Now;
                TimeSpan currentTime = now.TimeOfDay;
                DateTime fromDate, toDate;

                var message = string.Empty;
                if (GlobalConfig.InquiryRunner != null)
                {
                    if (GlobalConfig.InquiryRunner.RunDate.Date != now.Date)
                    {
                        GlobalConfig.InquiryRunner.RunDate = now;
                        GlobalConfig.InquiryRunner.Timer = 0;
                    }

                    Utils.WriteToFile($"Inquiry {GlobalConfig.InquiryRunner.RunDate} - {GlobalConfig.InquiryRunner.Timer}", "InQuiry");
                    // Match the current time to the configured times
                    if (currentTime.Hours == timeRun1.Hours && currentTime.Minutes >= timeRun1.Minutes
                        && GlobalConfig.InquiryRunner.Timer == 0)
                    {
                        // timeRun1 match: from = timeRun2 of yesterday, to = timeRun1 of today
                        fromDate = DateTime.Today.AddDays(-1).Add(timeRun2); // Yesterday's timeRun2
                        toDate = DateTime.Today.Add(timeRun1);              // Today's timeRun1
                        Utils.WriteToFile($"Matched timeRun1: From {fromDate} to {toDate}", "InQuiry");

                        foreach (var data in VTBAccounts())
                        {
                            Utils.WriteToFile($"Call with {data}", "InQuiry");
                            APIJobHandler.CallVTBAPI(data, fromDate.ToString("yyyyMMdd"), toDate.ToString("yyyyMMdd"), timeRun1String, timeRun2String, ref message);
                            Utils.WriteToFile($"{message}", "InQuiry");
                        }
                        //VTBHandler.CallVTBAPI()

                        GlobalConfig.InquiryRunner.Timer = 1;
                    }
                    else if (currentTime.Hours == timeRun2.Hours && currentTime.Minutes >= timeRun2.Minutes
                        && GlobalConfig.InquiryRunner.Timer == 1)
                    {
                        // timeRun2 match: from = timeRun1 of today, to = timeRun2 of today
                        fromDate = DateTime.Today.Add(timeRun1);            // Today's timeRun1
                        toDate = DateTime.Today.Add(timeRun2);              // Today's timeRun2
                        Utils.WriteToFile($"Matched timeRun2: From {fromDate} to {toDate}", "InQuiry");

                        foreach (var data in VTBAccounts())
                        {
                            Utils.WriteToFile($"Call with {data}", "InQuiry");
                            APIJobHandler.CallVTBAPI(data, fromDate.ToString("yyyyMMdd"), toDate.ToString("yyyyMMdd"), timeRun1String, timeRun2String, ref message);
                            Utils.WriteToFile($"{message}", "InQuiry");
                        }
                        GlobalConfig.InquiryRunner.Timer = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteToFile($"Inquiry: {ex.Message}", "InQuiry");
                return;
            }
        }

        private List<string> VTBAccounts()
        {
            var result = new List<string>();
            var query = "SELECT * FROM  \"" + Constant.Schema + "\".\"vw_Bank_BankAccount\" WHERE \"Key\" = 'VT'";
            var datas = dbProvider.QueryList(query);
            if (datas != null && datas.Length > 0)
            {
                foreach (var data in datas)
                {
                    result.Add(data["Account"].ToString());
                }
            }
            return result;
        }

        private void runSettlement()
        {
            Utils.WriteToFile($"On Start Settlement", "Settlement");
            timerSettlement.Elapsed += new ElapsedEventHandler(OnElapsedSettlement);
            timerSettlement.Interval = Interval;
            timerSettlement.Enabled = true;
            timerSettlement.Start();
        }

        public void OnElapsedSettlement(object source, ElapsedEventArgs e)
        {
            try
            {
                Utils.WriteToFile($"Settlement", "Settlement");
                if (GlobalConfig.PayooRunner == null)
                    GlobalConfig.PayooRunner = new RunnerTime();

                var timeRun1String = ConfigurationManager.AppSettings["timeRun1"];
                var timeRun2String = ConfigurationManager.AppSettings["timeRun2"];
                var timeRun1 = TimeSpan.Parse(timeRun1String);
                var timeRun2 = TimeSpan.Parse(timeRun2String);

                Utils.WriteToFile($"timeRun1:{timeRun1}", "Settlement");
                Utils.WriteToFile($"timeRun2:{timeRun2}", "Settlement");

                DateTime now = DateTime.Now;
                TimeSpan currentTime = now.TimeOfDay;
                var message = string.Empty;
                Utils.WriteToFile($"Settlement2", "Settlement");

                Utils.WriteToFile($"currentTime:{currentTime}", "Settlement");
                if (GlobalConfig.PayooRunner != null)
                {
                    if (GlobalConfig.PayooRunner.RunDate.Date != now.Date)
                    {
                        GlobalConfig.PayooRunner.RunDate = now;
                        GlobalConfig.PayooRunner.Timer = 0;
                    }

                    Utils.WriteToFile($"Settlement {GlobalConfig.PayooRunner.RunDate } - {GlobalConfig.PayooRunner.Timer}", "Settlement");
                    // Match the current time to the configured times
                    if (currentTime.Hours == timeRun1.Hours && currentTime.Minutes >= timeRun1.Minutes
                        && GlobalConfig.PayooRunner.Timer == 0)
                    {
                        APIJobHandler.CallPayooAPI(now.ToString("yyyyMMdd"), 1, ref message);
                        Utils.WriteToFile($"{message}", "Settlement");
                        GlobalConfig.PayooRunner.Timer = 1;
                    }
                    else if (currentTime.Hours == timeRun2.Hours && currentTime.Minutes >= timeRun2.Minutes
                        && GlobalConfig.InquiryRunner.Timer == 1)
                    {
                        APIJobHandler.CallPayooAPI(now.ToString("yyyyMMdd"), 2, ref message);
                        Utils.WriteToFile($"{message}", "Settlement");
                        GlobalConfig.PayooRunner.Timer = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteToFile($"OnElapsedSettlement {ex.Message}", "Settlement");
                return;
            }
        }

        #region Test
        Timer timer = new Timer();
        private void run()
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Start();
        }

        public void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                var docentry = 1893;
                Utils.WriteToFile($"Start Cancle {docentry}", "Cancle");

                //if(DISe)
                var message = string.Empty;
                APIJobHandler.Test(docentry, ref message);
                Utils.WriteToFile($"{message}", "Cancle");

            }
            catch (Exception ex)
            {
                Utils.WriteToFile($"Error catching runTime and generate fromDate toDate: {ex.Message}", "Cancle");
                return;
            }
        }
        #endregion
    }
}
