using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    public abstract class SingletonBase<T> where T : SingletonBase<T>, new()
    {

        private static object lockObj = new object(); //Thread Safe , double lock
        private static T m_instance = null;
        public static T instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (lockObj)
                    {
                        if (m_instance == null)
                        {
                            m_instance = new T();
                        }
                    }
                }

                return m_instance;
            }
        }

    }
}
