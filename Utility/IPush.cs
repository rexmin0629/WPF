using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AxIPUSHXLib;

using MLTraderRiskMnter.Model;

namespace Utility
{
    public class IPush
    {
        public delegate void stringDelegate(string msg);
        public stringDelegate showMsg;
        public Action<IPush, string , string> receiveMsg;
        public Action<IPush, string, object> receiveBinMsg;
        public Action<IPush> SubSubjectReady;

        public AxiPushX axiPushX;
        public string ipushSubSubject;
        public string ipushSendSubject;
        public string machine;


        private const string SUB_SUBJECT = "SubSubject";
        private const string SEND_SUBJECT = "SendSubject";
        private const string TAG = "TAG";
        private const string PRODUCT = "Product";
        private const string IP = "IP";
        private const string PORT = "PORT";
        private const string COMPANY = "COMPANY";
        private const string USERNAME = "USERNAME";
        private const string PASSWARD = "PASSWARD";

        public bool isConnect { get;private set;}

        public IPush()
        {
            isConnect = false;
            axiPushX = new AxiPushX();
            axiPushX.CreateControl();
            axiPushX.ConnectReady += new _DiPushXEvents_ConnectReadyEventHandler(axiPushX_ConnectReady);
            axiPushX.ConnectLost += new EventHandler(axiPushX_ConnectLost);
            axiPushX.ConnectFail += new _DiPushXEvents_ConnectFailEventHandler(axiPushX_ConnectFail);
            axiPushX.SubjectReceived += new _DiPushXEvents_SubjectReceivedEventHandler(axiPushX_SubjectReceived);
            axiPushX.SubjectBinReceived += new _DiPushXEvents_SubjectBinReceivedEventHandler(axiPushX_SubjectBinReceived);
           // System.Diagnostics.Debug.WriteLine("axiPushX handle is " + axiPushX.Handle);

            Datas.subjectRealTime =  subSubjectRealTime;
        }

        void axiPushX_SubjectBinReceived(object sender, _DiPushXEvents_SubjectBinReceivedEvent e)
        {
            if (receiveBinMsg != null)
            {
                receiveBinMsg(this, e.subject, e.data);
            }
        }

        public void Setting(string path)
        {
            if (System.IO.File.Exists(path))
            {
                Utility.IniFile setting = new Utility.IniFile(path);
                var dd = setting.GetSectionValues("IPUSH");
                IPushConnectSetting(dd);
            }
        }

        public void IPushConnectSetting(Dictionary<string, string> setting)
        {
           //ipushSubSubject = setting[SUB_SUBJECT];
           //ipushSendSubject = setting[SEND_SUBJECT];
           //machine = setting[TAG];
           axiPushX.product = setting[PRODUCT];
           axiPushX.ipuship = setting[IP];
           axiPushX.ipushport =int.Parse(setting[PORT]);
           axiPushX.company = setting[COMPANY];
           axiPushX.username = setting[USERNAME];
           axiPushX.password = setting[PASSWARD];

       
        }

        private void AddStatus(string msg)
        {
     //       System.Diagnostics.Debug.WriteLine(msg);
            if (showMsg != null)
            {
                showMsg(msg);
            }
        }

        public  void connectToServer()
        {
            int result = 0;
            result = axiPushX.ipushConnect();

            if (result <= 0)
            {
                AddStatus("ipush " + machine + " Cannot connect to iPush server " + axiPushX.ipuship);
                isConnect = false;
            }
            else
            {
                AddStatus("ipush " + machine + " Connect to iPush server succeed " + axiPushX.ipuship);
                isConnect = true;
            }
        }

        public void disConnectToServer()
        {
            axiPushX.ipushDisconnect();
        }

        public void sendSubject(string msg)
        {
            axiPushX.ipushSendSubject(ipushSendSubject,msg);
 //           Utility.Logging.Log.Info(msg);
        }

        void axiPushX_SubjectReceived(object sender, _DiPushXEvents_SubjectReceivedEvent e)
        {
          //  throw new NotImplementedException();
            AddStatus("ipush " + machine + " Received subject : "+e.subject.ToString()+" cmd msg : " + e.data.ToString());
 //           Utility.Logging.Log.Info(e.data);
            if (receiveMsg != null)
            {
                receiveMsg(this ,e.subject , e.data);
            }
        }

        void axiPushX_ConnectFail(object sender, _DiPushXEvents_ConnectFailEvent e)
        {
          //throw new NotImplementedException();
            AddStatus("ipush " + machine + " Connection Fail " + e.nStatus);
        }

        void axiPushX_ConnectLost(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
            AddStatus("ipush " + machine + " Connection lost ");
        }

        void axiPushX_ConnectReady(object sender, _DiPushXEvents_ConnectReadyEvent e)
        {
         //   throw new NotImplementedException();            
       //     axiPushX.ipushSubSubject(ipushSubSubject);
            AddStatus("ipush " + machine + " Connection Ready SubSubject : " + ipushSubSubject);
            if (SubSubjectReady != null)
            {
                SubSubjectReady(this);
            }
        }
        /// <summary>
        /// iFrpt. + 分公司 + 帳號
        /// </summary>
        /// <param name="subjectStr"></param>
        public void subSubjectRealTime(string subjectStr)
        {
            subjectStr = string.Format("{0}.{1}.{2}", Datas.ipushSubSubjectFirstSymbol, Datas.companySymbol, subjectStr);
            axiPushX.ipushSubSubject(subjectStr);
        }
       
    }
}
