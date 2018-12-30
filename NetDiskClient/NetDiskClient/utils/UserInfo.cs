using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDiskClient.utils
{
    internal class UserInfo
    {
        private string icon = null;
        private string nickname = null;
        private string password = null;
        private string sex = null;
        private string school = null;
        private string uid;

        public string Icon { get => icon; set => icon = value; }
        public string Nickname { get => nickname; set => nickname = value; }
        public string Password { get => password; set => password = value; }
        public string Sex { get => sex; set => sex = value; }
        public string School { get => school; set => school = value; }
        public string Uid { get => uid; set => uid = value; }
    }
}