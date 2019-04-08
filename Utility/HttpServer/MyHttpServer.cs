using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utility;

namespace AlgoSrv.HttpServer
{

    enum ColorCircle
    {
        circle_green,
        circle_red,
    }

    class MyHttpServer : Utility.HttpServer
    {
        private const string PATH = "HttpSrv/index.html";
        private string indexStr = string.Empty;

        private DateTime PathLastWriteTime;

        public MyHttpServer(int port) : base(port)
        {
            
        }

        private void checkHttpFile()
        {
            if (!File.Exists(PATH))
            {
                return;
            }

            FileInfo fi = new FileInfo(PATH);
            if (PathLastWriteTime == null)
            {
                PathLastWriteTime = fi.LastWriteTime;
                refleshHttpFile();
            }
            else if (PathLastWriteTime < fi.LastWriteTime)
            {
                PathLastWriteTime = fi.LastWriteTime;
                refleshHttpFile();
            }
        }

        private void refleshHttpFile()
        {
            using (FileStream fs = new FileStream(PATH, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    indexStr = sr.ReadToEnd();
                }
            }
        }


        public override void handleGETRequest(HttpProcessor p)
        {
            checkHttpFile();            
            Console.WriteLine("request: {0}", p.http_url);
            p.writeSuccess();
            p.outputStream.WriteLine(indexStr);

            p.outputStream.WriteLine(CheckList.getBodys());

            //p.outputStream.WriteLine(Body.getSection(ColorCircle.circle_green, "測試1"));
            //p.outputStream.WriteLine(Body.getSection(ColorCircle.circle_red, "測試2"));
            //p.outputStream.WriteLine(Body.getSection(ColorCircle.circle_green, "測試3"));
            //p.outputStream.WriteLine(Body.getSection(ColorCircle.circle_red, "測試4"));
            p.outputStream.WriteLine("</table>");
            p.outputStream.WriteLine("<P>Current Time: " + DateTime.Now.ToString());
            p.outputStream.WriteLine("</body></html>");
            
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            Console.WriteLine("post request: {0}", p.http_url);
        }

    }
}
