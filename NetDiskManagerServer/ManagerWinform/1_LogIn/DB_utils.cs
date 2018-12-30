using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ManagerWinform
{
    internal class DB_utils
    {
        public static MySqlConnection GetConnection()
        {
            string connectStr = "server=127.0.0.1;port=3306;user=root;password=root; database= netdiskdb;";
            return new MySqlConnection(connectStr);
        }
    }
}