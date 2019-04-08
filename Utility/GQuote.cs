using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AxGQUOTEOPRLib;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using MLTraderRiskMnter.Model;
using System.Threading;
using System.Threading.Tasks;

namespace MLTraderRiskMnter
{
    public enum holeMarketSymbo
    {
        TSE_OTC_1=1,
        FUT_OB_2=2,
        OPT_3=3,
        TSE_4=4,
        OTC_5=5,
        FUT_6=6,
        OB_7=7,
    }


    public class GQuote:IDisposable
    {
        public Action<string> showMsg;
        public Action onConnectReady;
        public AxGQuoteOpr axGQuoteOpr;

        private Action<string> getDataNotify;
        /// <summary>
        /// stockno , HighLimit , LowLimit , LastPrice , TotalVolume
        /// 價錢都是100倍的整數
        /// </summary>
        public void registGetDataNotify(Action<string> notify)
        {
            getDataNotify -= notify;
            getDataNotify += notify;
        }

        public void unRegistGetDataNotify(Action<string> notify)
        {
            getDataNotify -= notify;
        }


        public bool isConnect { get; private set; }

        public GQuote()
        {
            isConnect = false;

            axGQuoteOpr = new AxGQuoteOpr();
            //  axGQuoteOpr.CreateControl();  // 這邊要用 WindowsFormsHost
            axGQuoteOpr.ConnectReady += new EventHandler(axGQuoteOpr_ConnectReady);
            axGQuoteOpr.ConnectLost += new EventHandler(axGQuoteOpr_ConnectLost);
            axGQuoteOpr.ConnectFail += new _DGQuoteOprEvents_ConnectFailEventHandler(axGQuoteOpr_ConnectFail);
            axGQuoteOpr.Data += new _DGQuoteOprEvents_DataEventHandler(axGQuoteOpr_Data);
            axGQuoteOpr.WriteLog += new _DGQuoteOprEvents_WriteLogEventHandler(axGQuoteOpr_WriteLog);

            WindowsFormsHost host = new WindowsFormsHost();
            host.Child = axGQuoteOpr;
            axGQuoteOpr.Handle.GetHashCode();//must to add this line
            // System.Diagnostics.Debug.WriteLine("axGQuoteOpr handle is "+axGQuoteOpr.Handle);

        }

        void axGQuoteOpr_WriteLog(object sender, _DGQuoteOprEvents_WriteLogEvent e)
        {
            System.Diagnostics.Debug.WriteLine(e.strLog);
        }

        private void AddStatus(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
            if (showMsg != null)
            {
                showMsg(msg);
            }
        }

        public void DoConnect(Dictionary<string, string> setInfo)
        {
            string path = string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, "configs", setInfo["QuoteConnINI"]);
            try
            {
                if (System.IO.File.Exists(path))
                {
                    DoConnect(path, setInfo["Session"], setInfo["UserName"], setInfo["Password"]);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(path + " is " + System.IO.File.Exists(path));                
                }
                        
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
          //      System.Windows.MessageBox.Show(e.Message);
            }
        }

        public void DoConnect(string strProfilePath , string strSession , string strUserName , string strPassword)
        {
            int result = 0;
            
            result = axGQuoteOpr.DoConnect(strProfilePath, strSession, strUserName, strPassword);
            
            if (result <= 0)
            {
                isConnect = false;
                AddStatus("GQuote  Cannot connect to server ");                
            }
            else
            {
                isConnect = true;
                AddStatus("GQuote  Connect to server succeed ");                
            }
        }

        public string SymboLookUP(string symbo)
        {
            string symbos = axGQuoteOpr.SymbolLookup(symbo);
            //Tools.instance.ProcessLog(symbos);
            return symbos;
        }

        public void DoDisconnect()
        {
            axGQuoteOpr.DoDisconnect();
        }

        public void DoSub(holeMarketSymbo symbo)
        {
            string strSymbol = ((int)symbo).ToString();
            DoSub(strSymbol);            
        }
        public void DoSub(string strSymbol)
        {
            axGQuoteOpr.DoSub(strSymbol);
        }
        public void DoUnsub(holeMarketSymbo symbo)
        {
            string strSymbol = ((int)symbo).ToString();
            DoUnsub(strSymbol);
        }
        public void DoUnsub(string strSymbol)
        {
            axGQuoteOpr.DoUnsub(strSymbol);
        }

        void axGQuoteOpr_Data(object sender, _DGQuoteOprEvents_DataEvent e)
        {
            QuoteData QD;
            string symbo = e.strSymbolName;
            if (Datas.quoteWareHouse.ContainsKey(symbo))
            {
                QD = Datas.quoteWareHouse[symbo];
            }else{
                QD = new QuoteData();
                QD.Symbol = symbo;
                QD.SymboType = (int)axGQuoteOpr.GetSymbolInfoValue(e.nSymbolIndex, 0);
                Datas.quoteWareHouse.Add(symbo, QD);
            }

            QD.TradingPrice = axGQuoteOpr.GetSymbolInfoValue(e.nSymbolIndex, 3);
            QD.ReferencePrice = axGQuoteOpr.GetSymbolInfoValue(e.nSymbolIndex, 12);
            QD.Bid = axGQuoteOpr.GetSymbolInfoValue(e.nSymbolIndex, 30);
            QD.Ask = axGQuoteOpr.GetSymbolInfoValue(e.nSymbolIndex, 50);
            

            if (getDataNotify != null)
            {
                getDataNotify(symbo);
            }
        }

        void axGQuoteOpr_ConnectFail(object sender, _DGQuoteOprEvents_ConnectFailEvent e)
        {
            isConnect = false;
            System.Diagnostics.Debug.WriteLine("連線失敗");
            AddStatus("GQuote  Cannot connect to server "); 
        }

        void axGQuoteOpr_ConnectLost(object sender, EventArgs e)
        {
            isConnect = false;
            System.Diagnostics.Debug.WriteLine("連線遺失");
            AddStatus("GQuote  Cannot connect to server "); 
        }

        void axGQuoteOpr_ConnectReady(object sender, EventArgs e)
        {
            isConnect = true;
            System.Diagnostics.Debug.WriteLine("連線成功");
            AddStatus("GQuote  Connect to server succeed ");    
            if (onConnectReady != null)
            {
                onConnectReady();
            }
        }

        public void Dispose()
        {
            try
            {
                axGQuoteOpr.DoDisconnect();
                axGQuoteOpr.Dispose();
            }
            catch (Exception e)
            {

            }
        }
    }
}
