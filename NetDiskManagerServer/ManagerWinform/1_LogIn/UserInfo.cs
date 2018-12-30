using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ManagerWinform
{
    internal class UserInfo
    {
        private int uid;
        private string icon;
        private string nickname;
        private string usename;
        private int maxnumber;
        private string sex;
        private string school;
        private Socket socket;
        private String udpip;
        private int udpport;

        public int Uid { get => uid; set => uid = value; }
        public string Usename { get => usename; set => usename = value; }
        public int Maxnumber { get => maxnumber; set => maxnumber = value; }
        public Socket Socket { get => socket; set => socket = value; }
        public string Udpip { get => udpip; set => udpip = value; }
        public int Udpport { get => udpport; set => udpport = value; }
        public string Icon { get => icon; set => icon = value; }
        public string Nickname { get => nickname; set => nickname = value; }
        public string Sex { get => sex; set => sex = value; }
        public string School { get => school; set => school = value; }
    }
}