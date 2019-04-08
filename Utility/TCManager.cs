/***
 * 
 * 20170208 Victor
 * 不使用singleton來寫，避免同一支app多重訂閱時難以控管
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTradeWrapperAPILib;
using TCQuoteWrapperAPILib;

namespace Utility
{
    public class TCManager
    {
        public TCQuoteWrapperAPILib.TCQuoteAPIEx QuoteAPI { get; private set; }
        public TCTradeWrapperAPILib.CTCTradeAPIEx TradeAPI { get; private set; }
        /// <summary>
        /// 路徑
        /// </summary>
        public string AppManager_Path = string.Empty;
        /// <summary>
        /// 帳號紀錄
        /// </summary>
        public List<ADIAccount> mapAccount = new List<ADIAccount>(); 
        public bool isConnected = false;
        public Action connectReady = null;

        public void init()
        {
            Task.Factory.StartNew(init_QuoteAPI);
        }

        void initFinish()
        {
            isConnected = true;
            if (connectReady != null)
            {
                connectReady();
            }
        }

        void init_QuoteAPI()
        {
            if (QuoteAPI == null)
            {
                QuoteAPI = TCoreAPI.Quote.GetQuoteAPI();
                QuoteAPI.OnCommandMsg += new _ITCQuoteAPIExEvents_OnCommandMsgEventHandler(QuoteAPI_OnCommandMsg);
                int ret = TCoreAPI.Quote.Connect(QuoteAPI);
                I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Info, this, String.Format("new QuoteAPI: {0}", ret));
            }
        }

        void QuoteAPI_OnCommandMsg(int MsgType, int MsgCode, string MsgString)
        {
            try
            {
                if (MsgType == TCoreAPI.Defines.COMMAND_MSG_SYSTEM_STATUS)
                {
                    switch (MsgCode)
                    {
                        case TCoreAPI.Defines.CONNECTION_STATUS_EXCHANGE_CLEAR:
                            I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Debug, this, "EXCHANGE Clear: " + MsgString);  //MsgString 為清盤交易所代碼                                
                            break;
                        case TCoreAPI.Defines.CONNECT_STATUS_CONNECTED:
                            
                            I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Debug, this, "QuoteAPI CONNECTED:" + MsgString);
                            AppManager_Path = QuoteAPI.GetGeneralService("AppManager_Path");
                            I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Debug, this, "QuoteAPI AppManager_Path:" + AppManager_Path);
                            //取得路徑後才可以啟動TradeAPI
                            init_TradeAPI();
                            break;
                        case TCoreAPI.Defines.CONNECT_STATUS_DISCONNECTED:
                            isConnected = false;
                            I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Debug, this, "QuoteAPI DISCONNECTED:" + MsgString);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Exception, this, "[Exception] QuoteAPI_OnCommandMsg : " + e.Message);
            }
        }

        void init_TradeAPI()
        {
            if (TradeAPI == null)
            {
                TradeAPI = TCoreAPI.Trade.GetTradeAPI();
                TradeAPI.OnAccountUpdate += new _ICTCTradeAPIExEvents_OnAccountUpdateEventHandler(TradeAPI_OnAccountUpdate);
                int ret = TCoreAPI.Trade.Connect(TradeAPI);
                I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Info, this, String.Format("new TradeAPI: {0}", ret));
            }
        }

        void TradeAPI_OnAccountUpdate(int nType, string AcctMask, int nCount)
        {
            try
            {
                I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Info, this, "TradeAPI_OnAccountUpdate Begin-------------------------------");
                TCTradeWrapperAPILib.ADIAccount FADIAccount = new TCTradeWrapperAPILib.ADIAccount();
                if (nType == TCoreAPI.Defines.ACCOUNT_DATA_TYPE_ACCOUNT)
                {
                    I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Info, this, "TradeAPI_OnAccountUpdate nType:" + nType.ToString());
                    // 紀錄所有交易帳號
                    mapAccount.Clear();
                    for (int i = 0; i < nCount; ++i)
                    {
                        TCTradeWrapperAPILib.ADIAccount account = new TCTradeWrapperAPILib.ADIAccount();

                        TradeAPI.GetAccountData(TCoreAPI.Defines.ACCOUNT_DATA_TYPE_ACCOUNT, i, "", account);

                        I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Info, this, "TradeAPI_OnAccountUpdate Get i:" + i.ToString() + " BrokerID:" + account.BrokerID + " account:" + account.Account);
                        if (account.Status != 2)
                            continue;
                        //if (account.BrokerID == "ovs")
                        //    continue;
                        //if (account.BrokerID == "stk")
                        //    continue;
                        mapAccount.Add(account);
                    }
                }
                I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Info, this, "TradeAPI_OnAccountUpdate End-------------------------------");
                
                initFinish();
            }
            catch (Exception ex)
            {
                I4Framework.Func.SystemLogger(I4Framework.Define.LogStatus.Exception, this, "[TradeAPI_OnAccountUpdate] " + ex.Message);
            }
        }

    }
}
