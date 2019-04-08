/*
 * 使用方法
 * ThreadAdapter.Invoke(new SendOrPostCallback(o =>
            {
               label1.Text = sb.ToString();
            }), null);
 * 
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utility
{
    public static class ThreadAdapter
    {
        public static SynchronizationContext Dispacher { get; private set; }
        /// <summary>
        /// 請於UI執行緒呼叫此方法。
        /// </summary>
        public static void Initialize()
        {
            if (ThreadAdapter.Dispacher == null)
                ThreadAdapter.Dispacher = SynchronizationContext.Current;
        }
        /// <summary>
        /// 在 Dispatcher 關聯的執行緒上以同步方式執行指定的委派。
        /// </summary>
        public static void Invoke(SendOrPostCallback d, object state)
        {
            if (ThreadAdapter.Dispacher == null)
            {
                if (state != null)
                {
                    System.Diagnostics.Debug.WriteLine("ThreadAdapter : " + state.ToString());
                }
                return;
            }

            try
            {
                Dispacher.Send(d, state);
            }
            catch (Exception e)
            {
                Console.WriteLine("ThreadAdapter : "+e.Message);
            }
        }
        /// <summary>
        /// 在 Dispatcher 關聯的執行緒上以非同步方式執行指定的委派。
        /// </summary>
        public static void BeginInvoke(SendOrPostCallback d, object state)
        {
            if (ThreadAdapter.Dispacher == null)
            {
                if (state != null)
                {
                    System.Diagnostics.Debug.WriteLine("ThreadAdapter : " + state.ToString());
                }
                return;
            }
            try
            {
                Dispacher.Post(d, state);
            }
            catch (Exception e)
            {
                Console.WriteLine("ThreadAdapter : "+e.Message);
            }
        }

        /// <summary>
        /// UI執行緒關閉時呼叫
        /// </summary>
        public static void Dispose()
        {
            ThreadAdapter.Dispacher = null;
        }
    }
}
