using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerWinform.UdpServer;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace ManagerWinform
{
    internal class UserService
    {
        /// <summary>
        /// 检查登录信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int loginVertify(string username, string password)
        {
            MySqlConnection conn = null;
            try
            {
                conn = DB_utils.GetConnection();
                conn.Open();
                string sql = "SELECT * FROM netdiskdb.users where username = @para1";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("para1", username);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    //询问密码是否一致
                    if (reader.GetString("password").Equals(password))
                    {
                        int uid = reader.GetInt32("uid");
                        return uid;
                    }
                    else
                    {
                        conn.Close();
                        return -1;
                    }
                }
                else
                {
                    conn.Close();
                    return -2;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 获得好友列表
        /// </summary>
        /// <returns></returns>
        public static List<FriendInfo> GetFriendList(int uid)
        {
            MySqlConnection conn = null;
            try
            {
                conn = DB_utils.GetConnection();
                conn.Open();
                string sql =
                    "SELECT users.uid,users.username, users.nickname, users.icon, users.sex, users.school FROM netdiskdb.friend inner join users where users.uid = friend.friendid and friend.uid = @para1";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("para1", uid);
                MySqlDataReader reader = cmd.ExecuteReader();
                List<FriendInfo> friendInfos = new List<FriendInfo>();
                while (reader.Read())
                {
                    FriendInfo friendInfo = new FriendInfo();
                    friendInfo.Uid = reader.GetInt32("uid");
                    friendInfo.Username = reader.GetString("username");
                    friendInfo.Icon = reader.GetString("icon");
                    friendInfo.Nickname = reader.GetString("nickname");
                    friendInfo.Sex = reader.GetString("sex");
                    friendInfo.School = reader.GetString("school");

                    friendInfos.Add(friendInfo);
                }

                return friendInfos;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 个人资料查询和好友资料查询
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>返回UserInfo </returns>
        public UserInfo GetUserInfo(int uid)
        {
            MySqlConnection conn = null;

            try
            {
                conn = DB_utils.GetConnection();
                conn.Open();
                string sql = "SELECT * FROM netdiskdb.users where uid = @para1";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("para1", uid);
                MySqlDataReader reader = cmd.ExecuteReader();
                UserInfo userInfo = new UserInfo();
                if (reader.Read())
                {
                    userInfo.Uid = reader.GetInt32("uid");
                    userInfo.Icon = reader.GetString("icon");
                    userInfo.Nickname = reader.GetString("nickname");
                    userInfo.Usename = reader.GetString("username");
                    userInfo.Sex = reader.GetString("sex");
                    userInfo.School = reader.GetString("school");
                }

                return userInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public void ChangeUserInfo(int uid, string nickname, string sex, string school)
        {
            MySqlConnection conn = null;

            try
            {
                conn = DB_utils.GetConnection();
                conn.Open();
                string sql = "UPDATE `netdiskdb`.`users` SET `nickname` = @para1, `sex` = @para2, `school` = @para3 WHERE (`uid` = @para4)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("para4", uid);
                cmd.Parameters.AddWithValue("para1", nickname);
                cmd.Parameters.AddWithValue("para2", sex);
                cmd.Parameters.AddWithValue("para3", school);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public static void ChangeUserIcon(int uid, string icon)
        {
            MySqlConnection conn = null;

            try
            {
                conn = DB_utils.GetConnection();
                conn.Open();
                string sql = "UPDATE `netdiskdb`.`users` SET `icon` = @para1 WHERE (`uid` = @para4)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("para4", uid);
                cmd.Parameters.AddWithValue("para1", icon);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        //private static void Main()
        //{
        //    new UserService().ChangeUserInfo(1, "1", "1", "1");
        //}
    }
}