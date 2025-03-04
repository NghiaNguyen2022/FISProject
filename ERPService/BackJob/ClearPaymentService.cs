using APIHandler;
using ERPService.Common;
using ERPService.DataReader;
using ERPService.Models;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text.Json;
using System.Timers;

namespace ERPService.BackJob
{
    public partial class ClearService : ServiceBase
    {
        Timer timer = new Timer();

        public ClearService()
        {
            InitializeComponent();
        }

       

        protected override void OnStop()
        {

            Utils.WriteToFile($"On Stop Clear");
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Utils.WriteToFile($"On Start Clear");
                run();
            }
            catch (FormatException ex)
            {
                Utils.WriteToFile($"Error: {ex.Message}");
                return;
            }
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

                if (GlobalConfig.ClearRunner == null)
                    GlobalConfig.ClearRunner = new RunnerTime();

                var timeRun1String = "12:40";
                var timeRun2String = "17:40";
                var timeRun1 = TimeSpan.Parse(timeRun1String);
                var timeRun2 = TimeSpan.Parse(timeRun2String);

                DateTime now = DateTime.Now;
                TimeSpan currentTime = now.TimeOfDay;

                var message = string.Empty;
                if (GlobalConfig.ClearRunner != null)
                {
                    if (GlobalConfig.ClearRunner.RunDate.Date != now.Date)
                    {
                        GlobalConfig.ClearRunner.RunDate = now;
                        GlobalConfig.ClearRunner.Timer = 0;
                    }

                    Utils.WriteToFile($"Clear {GlobalConfig.InquiryRunner.RunDate } - {GlobalConfig.InquiryRunner.Timer}");
                    // Match the current time to the configured times
                    if (currentTime.Hours == timeRun1.Hours && currentTime.Minutes >= timeRun1.Minutes
                        && GlobalConfig.ClearRunner.Timer == 0)
                    {          // Today's timeRun1
                        var batch = $"SP{now.ToString("yyMMdd")}01";
                        Utils.WriteToFile($"Run: {now} batch {batch}");

                        APIJobHandler.Clear(batch, ref message);
                        Utils.WriteToFile($"{message}");
                        GlobalConfig.ClearRunner.Timer = 1;
                    }
                    else if (currentTime.Hours == timeRun2.Hours && currentTime.Minutes >= timeRun2.Minutes
                        && GlobalConfig.ClearRunner.Timer == 1)
                    {

                        var batch = $"SP{now.ToString("yyMMdd")}02";
                        Utils.WriteToFile($"Run: {now} batch {batch}");
                        
                        APIJobHandler.Clear(batch, ref message);
                        Utils.WriteToFile($"{message}");
                        GlobalConfig.ClearRunner.Timer = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteToFile($"Error catching runTime and generate fromDate toDate: {ex.Message}");
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
    }
}
