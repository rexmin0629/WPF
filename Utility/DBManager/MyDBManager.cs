using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Data;

namespace MyDBManager
{

    public class DBManager : SingletonBase<DBManager>
    {
        private object obj = new object();
        private string DBSetInfo = string.Empty;
        public Action<Exception> ExceptionHandler;
        public void init(string DBSetInfo)
        {
            this.DBSetInfo = DBSetInfo;
         //   Utility.WriteDBCommandToLog.instance.init();
        }

        public void addNewCommand(DBMailStruct mail)
        {
            //if (!mail.command.StartsWith("select"))
            //{
            //    Utility.WriteDBCommandToLog.instance.writeACommand(mail.command);
            //}
            //直接處理
            Task t1 = Task.Factory.StartNew(() =>
            {
                DBManager.instance.makeCommand(mail);
            });
        }

        private string connectSetting()
        {
            //return SetInfoINI.instance.DBSetInfo();
            return this.DBSetInfo;
        }

        private void makeCommand(DBMailStruct mail)
        {

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectSetting()))
                {
                    conn.Open();
                    //Console.WriteLine("MyDbConn : command = " + mail.command);
                    using (MySqlCommand cmd = new MySqlCommand(mail.command, conn))
                    {
                        if (mail.commandType == CommandType.select)
                        {
                            SQLAdapter(cmd, mail);
                        }
                        else
                        {
                            SQLTransaction(conn, cmd , mail);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                //System.Diagnostics.Debug.WriteLine("SQL error = " + mail.command);
                //Utility.Logging.SqlErrorLog(mail.command, ex);
                System.Diagnostics.Debug.WriteLine(e.Message);
                //throw e;
                if (ExceptionHandler != null)
                {
                    ExceptionHandler(e);
                }
            }
            finally
            {
                //使用完了就回收
                DBMailManager.instance.recycleMail(mail);
            }
        }

        private void SQLTransaction(MySqlConnection conn, MySqlCommand cmd , DBMailStruct mail)
        {
            using (MySqlTransaction trans = conn.BeginTransaction())
            {
                try
                {
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    mail.isSolve = true;
                    if(mail.sendTo != null){
                        mail.sendTo(mail);
                    }
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    if (mail.sendTo != null)
                    {
                        mail.isSolve = false;
                        mail.sendTo(mail);
                    }
                    throw e;
                }
            }
        }

        private void SQLAdapter(MySqlCommand cmd , DBMailStruct mail)
        {
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                mail.dataTable = dataTable;
                mail.isSolve = true;
                if(mail.sendTo != null){
                  mail.sendTo(mail);
                }
            }
        }

    }
}
