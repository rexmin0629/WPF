using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;


namespace Utility
{
    class HttpRequestString
    {

        string UTF8_BOM = "\uFEFF";

        private static HttpRequestString _instance;
        public static HttpRequestString instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HttpRequestString();
                }
                return _instance;
            }
        }
        static int Timeout = 30000;


        public void usePost(string targetUrl, string parame, Action<String> callBack)
        {
            byte[] postData = Encoding.UTF8.GetBytes(parame);
            HttpWebRequest request = HttpWebRequest.Create(targetUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = Timeout;
            request.ContentLength = postData.Length;
            request.AllowAutoRedirect = false;  // 禁止重新導向網頁
            // 寫入 Post Body Message 資料流
            using (Stream st = request.GetRequestStream())
            {
                st.Write(postData, 0, postData.Length);
            }

            string result = "";
            // 取得回應資料
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }

            if (callBack != null)
            {
                callBack(result);
            }
        }

        /// <summary>
        /// 單一執行續
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <returns></returns>
        public string useGet(string targetUrl)
        {
            HttpWebRequest request = HttpWebRequest.Create(targetUrl) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = Timeout;

            string result = "";
            // 取得回應資料
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {

                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8,true))
                {
                    result = sr.ReadToEnd();
                }

            }
            //去掉UTF8  bom
            if (result.Contains(UTF8_BOM))
            {
                result = result.Replace(UTF8_BOM, string.Empty);
            }

            return result;
        }



        /// <summary>
        /// 會起一個新執行續
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public void useGet(string targetUrl , Action<String> callBack)
        {
            Task.Factory.StartNew(() =>
            {
                string result = useGet(targetUrl);
                if (callBack != null)
                {
                    callBack(result);
                }
            });         

        }

    }
}
