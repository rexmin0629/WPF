using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyDBManager
{
    public class DBMailManager : SingletonBase<DBMailManager>
    {
        private object obj = new object();
        private List<DBMailStruct> mailList = new List<DBMailStruct>();
        private int count;

        public DBMailStruct getNewMail()
        {
            lock (obj)
            {
                DBMailStruct mail;
                if (mailList.Count > 0)
                {
                    mail = mailList[0];
                    mailList.RemoveAt(0);
                }
                else
                {
                    mail = new DBMailStruct();
                }
                count++;
                mail.init();
                //Console.WriteLine("mail manager get mail times : " + count);
                //   Console.WriteLine("maillist count : " + mailList.Count);
                return mail;
            }
        }

        public void recycleMail(DBMailStruct mail)
        {
            lock (obj)
            {
                mail.init();
                //確保沒有重複回收
                if (!mailList.Contains(mail))
                {
                    mailList.Add(mail);
                }
                else
                {
                    Console.WriteLine("maillist  :  it's already been recycle! " );
                }
                
            }
            //Console.WriteLine("maillist count : " + mailList.Count);
        }

    }
}
