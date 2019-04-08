using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace MyDBManager
{
    public enum CommandType
    {
        select,
        insert,
        update,
        delete,
    }

    public class DBMailStruct
    {
        public Action<DBMailStruct> sendTo;
        public string command;
        public CommandType commandType = CommandType.select;
        public bool isSolve;
        public DataTable dataTable;

        //------delete msg use
        public string deletePlkey;
        public string deletePlkeyMachine;

        //monitor
        public string machine;
        public string uid;
        public string app;

        public void init()
        {
            isSolve = false;
            dataTable = null;
            sendTo = null;
            command = null;
            commandType = CommandType.select;

            deletePlkeyMachine = null;
            deletePlkey = null;

            machine = null;
            uid = null;
            app = null;
        }

    }
}
