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
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public SendReceive() {
            m = new RabbitMQManagersApprove();

            t = new ManagersTimer();
            //wait 30 sec before start
            startDelayTimer1 = new Timer(1000);
            startDelayTimer1.Elapsed += new ElapsedEventHandler(this.OnTimer1);
            startDelayTimer1.Start();
        }

        public void OnTimer1(object sender, ElapsedEventArgs args)
        {
            logger.Info("trying to start connection to rabbitmq");
            startDelayTimer1.Stop();
            if (Init())
                return;
            //if not succeeded to start immediately wait 30 sec
            startDelayTimer2 = new Timer(30000);
            startDelayTimer2.Elapsed += new ElapsedEventHandler(this.OnTimer2);
            startDelayTimer2.Start();
            
        }

        public void OnTimer2(object sender, ElapsedEventArgs args) {
            logger.Info("trying 2 to start connection to rabbitmq");
            startDelayTimer2.Stop();
            if (!Init()) {
                logger.Error("rejected to connect 2nd time, stopping service");
                throw new Exception("rejected to connect 2nd time, stopping service");
            }
                
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
