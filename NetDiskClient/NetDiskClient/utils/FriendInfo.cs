using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NetDiskClient
{
    internal class FriendInfo
    {
        private string uid;
        private BitmapImage icon;
        private string iconstring;
        private string username;
        private string nickname;
        private string sex;
        private string school;

        public string Nickname { get => nickname; set => nickname = value; }
        public BitmapImage Icon { get => icon; set => icon = value; }
        public string Uid { get => uid; set => uid = value; }
        public string Iconstring { get => iconstring; set => iconstring = value; }
        public string Sex { get => sex; set => sex = value; }
        public string School { get => school; set => school = value; }
        public string Username { get => username; set => username = value; }

        public FriendInfo(string uid, string iconstring, string nickname, string sex, string school, string username)
        {
            Uid = uid;
            this.Iconstring = iconstring;

            Icon = new BitmapImage(new Uri("http://106.13.44.221:80/" + this.Iconstring + ".jpg", UriKind.RelativeOrAbsolute));

            Nickname = nickname;
            Sex = sex;
            School = school;
            Username = username;
        }

        //功能方法
        public bool isOnline = false;

        public void setIcon(string iconstring)
        {
            if (iconstring == "def")
            {
                iconstring = "0";
            }

            if (isOnline)
            {
                Icon = new BitmapImage(new Uri("http://106.13.44.221:80/" + Iconstring + ".jpg", UriKind.RelativeOrAbsolute));
            }
            else
            {
                Icon = new BitmapImage(new Uri("Image\\12.png", UriKind.RelativeOrAbsolute));
            }
        }

        public void setOnline(bool isonline)
        {
            this.isOnline = isonline;
            if (isOnline)
            {
                Icon = new BitmapImage(new Uri("http://106.13.44.221:80/" + Iconstring + ".jpg", UriKind.RelativeOrAbsolute));
            }
            else
            {
                Icon = new BitmapImage(new Uri("Image\\12.png", UriKind.RelativeOrAbsolute));
            }
        }

        public int CompareTo(FriendInfo friendInfo)
        {
            if (friendInfo.isOnline == true)
            {
                return -1;
            }
            else if (this.isOnline == true)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}