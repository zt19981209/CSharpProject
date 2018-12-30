using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerWinform._4_AddFriend;

namespace ManagerWinform.UdpServer
{
    internal class Config
    {
        //172.16.0.6  192.168.137.1   10.132.1.50
        public static string ipStr = "172.16.0.6";

        public static List<AddFriendClient> addFriendClients = new List<AddFriendClient>();

        public static Form1 form1 = null;
    }
}