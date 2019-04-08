using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utility
{
    class TimerManager : SingletonBase<TimerManager>
    {
        private object LockObj = new object();
        private bool runCount = false;
        public Action UpdateNotify;
        private object lockTime = new object();

        private int _Hz = 1000;
        public int Hz
        {
            get
            {
                return _Hz;
            }
            set
            {
                _Hz = Math.Max(1, value);
            }
        }

        public void Start()
        {
            lock (LockObj)
            {
                if (runCount == true)
                {
                    return;
                }
                runCount = true;
                Task.Factory.StartNew(count);
            }
        }

        public void Stop()
        {
            runCount = false;
        }

        private void count()
        {
            while (runCount)
            {
                if (UpdateNotify != null)
                {
                    UpdateNotify();
                }
                Thread.Sleep(Hz);
            }
        }

        public int getTime(double diff = 0)
        {
            string DStr = System.DateTime.Now.AddSeconds(diff).ToString("HHmmss");

            return Convert.ToInt32(DStr); ;
        }

        public double getDiffTime(int time)
        {
            string strTime = time.ToString().PadLeft(6, '0');
            TimeSpan ts = System.DateTime.ParseExact(strTime, "HHmmss", null).TimeOfDay;
            TimeSpan now = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            double diffSec = ts.TotalSeconds - now.TotalSeconds;

            return diffSec;
        }

    }
}
