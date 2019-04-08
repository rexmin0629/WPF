using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoSrv.HttpServer
{
    


    /// <summary>
    /// ipush a
    /// ipush b
    /// monitor ipush a
    /// monoitor ipush b
    /// Database
    /// xTrade
    /// GQuote
    /// </summary>
    class CheckList
    {

        private static StringBuilder strTmp = new StringBuilder();

        public static string getSection(bool open, string title)
        {
            ColorCircle cc = ColorCircle.circle_green;
            if (!open)
            {
                cc = ColorCircle.circle_red;
            }
            return getSection(cc, title);
        }

        public static string getSection(ColorCircle cc, string title)
        {
            string str = string.Format("<tr><td><div class='{0}'/></td><td>{1}</td></tr>", cc.ToString(), title);
            return str;
        }

        public static string getBodys()
        {
            strTmp.Clear();

            for (int i = 0; i < MainForm.instance.iPushList.Count(); i++)
            {
                strTmp.AppendLine(getSection(MainForm.instance.iPushList[i].getNowState(),"IPush " + MainForm.instance.iPushList[i].axiPushX.Tag.ToString()));
            }
            for (int i = 0; i < MainForm.instance.MonitorIPushList.Count(); i++)
            {
                strTmp.AppendLine(getSection(MainForm.instance.MonitorIPushList[i].getNowState(), "MonitorIPush " + MainForm.instance.MonitorIPushList[i].axiPushX.Tag.ToString()));
            }

            strTmp.AppendLine(getSection(Quote.QuoteReceiver.instance.getNowDBState(), "DataBase"));

            strTmp.AppendLine(getSection(xTrade.YFStockOrder.getNowState() , "xTrade"));

            strTmp.AppendLine(getSection(GQuote.isConnect, "GQuote"));

            strTmp.AppendLine(getSection(Order.OrdersController.instance.ControllerStart , "OrderController"));

            return strTmp.ToString();
        }
    }
}
