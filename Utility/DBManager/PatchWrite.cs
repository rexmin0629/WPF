using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyDBManager
{
    public class PatchWrite : SingletonBase<PatchWrite>
    {
        private object lockobj = new object();
        private int commsCount = 0;
        private int countFin = 0;

        public void WriteToDBMultiThread(List<string> comms, Action OnFinishNotify)
        {
            countFin = 0;
            commsCount = comms.Count;
            if (comms.Count == 0)
            {
                OnFinishNotify();
                return;
            }
            //Utility.Logging.Log.Debug(string.Format("Start countFinish:{0}|commsCount:{1}", countFin, commsCount));
            WriteToDBLarge(comms, OnFinishNotify);
        }

        private void getFinish(Action OnFinishNotify)
        {
            lock (lockobj)
            {
                countFin++;

                // System.Diagnostics.Debug.WriteLine(string.Format("countFin:{0}|commsCount:{1}",countFin,commsCount));
                if (countFin == commsCount)
                {
                    //Utility.Logging.Log.Debug(string.Format("Complete countFinish:{0}|commsCount:{1}", countFin, commsCount));
                    if (OnFinishNotify != null)
                    {
                        OnFinishNotify();
                    }
                }
                else if (countFin > commsCount)
                {
                    //System.Diagnostics.Debug.WriteLine("Count Error");
                    //Utility.Logging.Log.Debug(string.Format("Error countFinish:{0}|commsCount:{1}", countFin, commsCount));
                    throw new Exception(string.Format("Error countFinish:{0}|commsCount:{1}", countFin, commsCount));
                }
            }
        }


        private void WriteToDBLarge(List<string> comms, Action OnFinishNotify)
        {
            for (int i = 0; i < comms.Count; i++)
            {
                MyDBManager.DBMailStruct mail = MyDBManager.DBMailManager.instance.getNewMail();
                mail.command = comms[i];
                mail.commandType = MyDBManager.CommandType.insert;
                mail.sendTo = (MyDBManager.DBMailStruct smail) =>
                {
                    getFinish(OnFinishNotify);
                };

                MyDBManager.DBManager.instance.addNewCommand(mail);
            }
        }
        /// <summary>
        /// write to DB one by one
        /// </summary>
        /// <param name="comms"></param>
        /// <param name="OnFinishNotify"></param>
        public void WriteToDBSingleThread(List<string> comms, Action OnFinishNotify = null)
        {
            string CommStr = string.Empty;
            if (comms.Count > 0)
            {
                CommStr = comms[0];
                comms.RemoveAt(0);
            }
            else
            {
                if (OnFinishNotify != null)
                {
                    OnFinishNotify();
                }
                return;
            }

            MyDBManager.DBMailStruct mail = MyDBManager.DBMailManager.instance.getNewMail();
            mail.command = CommStr;
            mail.commandType = MyDBManager.CommandType.insert;
            mail.sendTo = (MyDBManager.DBMailStruct smail) =>
            {
                WriteToDBSingleThread(comms, OnFinishNotify);
            };
            try
            {
                MyDBManager.DBManager.instance.addNewCommand(mail);
            }
            catch (Exception e)
            {
                //Utility.Logging.Log.Debug(e.Message);
                throw e;
            }
        }

    }
}
