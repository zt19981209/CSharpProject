using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ManagerWinform
{
    internal class DB_Manager
    {
        public static void selcetAll()
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM mygamedb.users";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader.GetInt32("id") + reader.GetString("username") + reader.GetString("password"));
            }
        }

        public static bool Inquiry(string username)
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();
            string sql = "SELECT * FROM netdiskdb.users where username = @para1";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para1", username);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool temp = reader.Read();
            conn.Close();
            return temp;
        }

        public static bool InquiryNickname(string nickname)
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();
            string sql = "SELECT * FROM netdiskdb.users where nickname = @para1";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para1", nickname);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool temp = reader.Read();
            conn.Close();
            return temp;
        }

        public static string InquiryUsername(string username, out string nickname, out string icon, out int uid)
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();
            string sql = "SELECT * FROM netdiskdb.users where username = @para1";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para1", username);
            MySqlDataReader reader = cmd.ExecuteReader();

            string um = null;
            nickname = null;
            icon = null;
            uid = -1;
            if (reader.Read())
            {
                um = reader.GetString("username");
                nickname = reader.GetString("nickname");
                icon = reader.GetString("icon");
                uid = reader.GetInt32("uid");
            }
            conn.Close();

            return um;
        }

        public static string InquiryByNickname(string nickname, out string username, out string icon, out int uid)
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();
            string sql = "SELECT * FROM netdiskdb.users where nickname = @para1";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para1", nickname);
            MySqlDataReader reader = cmd.ExecuteReader();

            string nn = null;
            username = null;
            icon = null;
            uid = -1;
            if (reader.Read())
            {
                username = reader.GetString("username");
                nn = reader.GetString("nickname");
                icon = reader.GetString("icon");
                uid = reader.GetInt32("uid");
            }
            conn.Close();

            return nn;
        }

        public static void InsertUserInfo(string username, string password)
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();

            string sql = "INSERT INTO `netdiskdb`.`users` (`username`, `password`, `maxnumber`) VALUES (@para1, @para2, '100')";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para1", username);
            cmd.Parameters.AddWithValue("para2", password);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public static void AddFriend(int uid, int frienduid)
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();

            string sql = "INSERT INTO `netdiskdb`.`friend` (`uid`, `friendid`) VALUES (@para1, @para2)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para1", uid);
            cmd.Parameters.AddWithValue("para2", frienduid);
            cmd.ExecuteNonQuery();

            sql = "INSERT INTO `netdiskdb`.`friend` (`uid`, `friendid`) VALUES (@para1, @para2)";
            cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para1", frienduid);
            cmd.Parameters.AddWithValue("para2", uid);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public static void DeleteFriend(int uid, int frienduid)
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();
            string sql = "SELECT * FROM netdiskdb.friend where uid = @para1 and friendid = @para2";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para1", uid);
            cmd.Parameters.AddWithValue("para2", frienduid);
            MySqlDataReader reader = cmd.ExecuteReader();
            int logid = 0;
            if (reader.Read())
            {
                logid = reader.GetInt32("logid"); /*DELETE FROM `netdiskdb`.`friend` WHERE(`logid` = '59');*/
            }
            conn.Close();
            conn = DB_utils.GetConnection();
            conn.Open();
            sql = "DELETE FROM `netdiskdb`.`friend` WHERE(`logid` = @para1)";
            MySqlCommand cmd1 = new MySqlCommand(sql, conn);
            cmd1.Parameters.AddWithValue("para1", logid);
            cmd1.ExecuteNonQuery();
            conn.Close();
        }

        public static bool IsPassword(string password)
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();
            string sql = "SELECT * FROM netdiskdb.users where password = @para1";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para1", password);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool temp = reader.Read();
            conn.Close();
            return temp;
        }

        public static void SetPassword(int uid, string password)
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();
            string sql = "UPDATE `netdiskdb`.`users` SET `password` = @para1 WHERE (`uid` = @para4)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("para4", uid);
            cmd.Parameters.AddWithValue("para1", password);
            cmd.ExecuteNonQuery();
        }

        public static void getRowNum()
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();
            string sql = "select count(*) from mygamedb.users";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            Object result = cmd.ExecuteScalar();
            if (result != null)
            {
                Console.WriteLine(int.Parse(result.ToString()));
            }
        }

        public static void delete()
        {
            MySqlConnection conn = DB_utils.GetConnection();
            conn.Open();
            string sql = "DELETE FROM `mygamedb`.`users` WHERE (`id` = '15')";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            int results = cmd.ExecuteNonQuery();
            Console.WriteLine(results);
        }
    }
}