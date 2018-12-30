using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerWinform
{
    internal class FriendInfo
    {
        private int uid;
        private string username;
        private string icon;
        private string nickname;
        private string sex;
        private string school;

        public int Uid { get => uid; set => uid = value; }
        public string Username { get => username; set => username = value; }
        public string Icon { get => icon; set => icon = value; }
        public string Nickname { get => nickname; set => nickname = value; }
        public string Sex { get => sex; set => sex = value; }
        public string School { get => school; set => school = value; }
    }
}