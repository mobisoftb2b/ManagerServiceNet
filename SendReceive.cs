using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Timers;
using NLog;
using RabbitMQManager;


namespace ManagerServiceNet
{
    class SendReceive
    {
        RabbitMQManagersApprove m;
        ManagersTimer t;
        Timer startDelayTimer1;
        Timer startDelayTimer2;
        int count;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private int startTimeoutSec;
        private int startPollingTimeoutSec;
        public SendReceive() {
            startTimeoutSec = int.Parse(ConfigurationManager.AppSettings["startTimeoutSec"]);
            startPollingTimeoutSec = int.Parse(ConfigurationManager.AppSettings["startPollingTimeoutSec"]);
            m = new RabbitMQManagersApprove();

            t = new ManagersTimer();
            //start after 1 sec (for imeddiate start stop)
            count = 1;
            startDelayTimer1 = new Timer(startTimeoutSec * 1000);
            startDelayTimer1.Elapsed += new ElapsedEventHandler(this.OnTimer1);
            startDelayTimer1.Start();
        }

        public void OnTimer1(object sender, ElapsedEventArgs args)
        {
            logger.Info("trying to start connection to rabbitmq");
            startDelayTimer1.Stop();
            if (Init())
                return;
            logger.Error("rejected to connect 1nd time");
            //if not succeeded to start immediately wait 1 minute
            startDelayTimer2 = new Timer(startPollingTimeoutSec * 1000);
            startDelayTimer2.Elapsed += new ElapsedEventHandler(this.OnTimer2);
            startDelayTimer2.Start();
            
        }

        //timer 2 never stops
        //it tries to connect each 2 minutes
        public void OnTimer2(object sender, ElapsedEventArgs args) {
            count++;
            logger.Info($"trying to start connection to rabbitmq {count} times");
            if (Init())
                return;
            logger.Error($"rejected to connect {count} times");
        }


        private bool Init() {
            if (m.Init("") == true)
            {
                t.Start();
                m.ReceiveMessage();
                return true;
            }
            else
                return false;
            //m.TestFCMMessage("f8YL3HD6TdKRqC72-2nu5X:APA91bHul5OgswIaLRIWbHeiOIXyAMebyO-oPW81edRTkEzFdoO3yOJ5k055doVp_lyPJI3xWTsua62uMYLOIOHI-lTBDgQESjqt_Ckepo5SmzKTQ9RHnslg1lWLo-UHPvtjpkxknx9H", "from console 2");
        }

        public void Close() {
            m.Close();
            t.Stop();
        }
        

    }

}
