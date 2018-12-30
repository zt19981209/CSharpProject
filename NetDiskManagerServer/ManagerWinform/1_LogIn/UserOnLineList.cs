using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ManagerWinform
{
    /// <summary>
    /// 在线用户列表
    /// </summary>
    internal class UserOnLineList
    {
        private UserOnLineList()
        {
        }

        private static UserOnLineList userOnLineList = new UserOnLineList();

        public static UserOnLineList GetUserOnLineList()
        {
            return userOnLineList;
        }

        /// <summary>
        /// string是用户编号
        /// </summary>
        private Dictionary<int, UserInfo> usersDictionary = new Dictionary<int, UserInfo>();

        //注册在线用户
        public void regOnline(int uid, Socket socket, String username)
        {
            //先检查此账户是否已经在线 在线的话就强行让其下线
            UserInfo userInfo = null;
            try
            {
                userInfo = usersDictionary[uid];
                userInfo.Socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                userInfo = new UserInfo();
                userInfo.Uid = uid;
                userInfo.Socket = socket;
                userInfo.Usename = username;
                usersDictionary.Add(uid, userInfo);//登记在线
            }
        }

        /// <summary>
        /// 更新客户端的udp信息
        /// </summary>
        /// <param name="uid">UDP地址</param>
        /// <param name="ip">UDP端口</param>
        /// <param name="port"></param>
        public void updateOnlineUDP(int uid, string ip, int port)
        {
            UserInfo userInfo = usersDictionary[uid];
            userInfo.Udpip = ip;
            userInfo.Udpport = port;
        }

        /// <summary>
        /// 判断用户是否在线
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool isUserOnline(int uid)
        {
            return usersDictionary.ContainsKey(uid);
        }

        /// <summary>
        /// 获得在线用户信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public UserInfo GetOnlineUserInfo(int uid)
        {
            return usersDictionary[uid];
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="uid"></param>
        public void Logout(int uid)
        {
            usersDictionary.Remove(uid);
        }

        /// <summary>
        /// 获得在线用户的所有键
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, UserInfo>.KeyCollection GetUserInfos()
        {
            return usersDictionary.Keys;
        }
    }
}