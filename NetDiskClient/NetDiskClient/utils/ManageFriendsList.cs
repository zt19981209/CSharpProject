using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace NetDiskClient.utils
{
    internal class ManageFriendsList
    {
        public static List<FriendInfo> friendslList = null;

        public void UpdateFriendsList()
        {
            //好友列表
            string friendslistjson = Config.friends_json;
            //加载好友之前先把friendlist设置为空的表
            friendslList = new List<FriendInfo>();
            JArray jArray = JArray.Parse(friendslistjson);
            if (friendslList.Count == 0)//第一次是加载
            {
                List<string> uids = new List<string>();
                List<string> icons = new List<string>();
                List<string> usernames = new List<string>();
                List<string> nicknames = new List<string>();
                List<string> sex = new List<string>();
                List<string> school = new List<string>();

                foreach (JObject jo in jArray)
                {
                    uids.Add(jo["Uid"].ToString().Trim());
                    icons.Add(jo["Icon"].ToString().Trim());
                    nicknames.Add(jo["Nickname"].ToString().Trim());
                    sex.Add(jo["Sex"].ToString().Trim());
                    school.Add(jo["School"].ToString().Trim());
                    usernames.Add(jo["Username"].ToString().Trim());
                }

                for (int i = 0; i < icons.Count; i++)
                {
                    friendslList.Add(new FriendInfo(uids[i], icons[i], nicknames[i], sex[i], school[i], usernames[i]));
                }

                UpdateOnlineFriend();
            }
            else //已经有列表数据了
            {
                List<string> uids = new List<string>();
                List<string> icons = new List<string>();
                List<string> nicknames = new List<string>();
                List<string> sex = new List<string>();
                List<string> school = new List<string>();
                List<string> usernames = new List<string>();

                foreach (JObject jo in jArray)
                {
                    uids.Add(jo["Uid"].ToString().Trim());
                    icons.Add(jo["Icon"].ToString().Trim());
                    nicknames.Add(jo["Nickname"].ToString().Trim());
                    sex.Add(jo["Sex"].ToString().Trim());
                    school.Add(jo["School"].ToString().Trim());
                    usernames.Add(jo["Username"].ToString().Trim());
                }
                //检查是否存在有新用户增减
                for (int i = 0; i < jArray.Count; i++)
                {
                    //判断新的好友列表中的数据在旧的好友列表中是否存在
                    bool isExist = false;
                    FriendInfo friendInfo = null;
                    foreach (var temp in friendslList)
                    {
                        if (uids[i] == temp.Uid)
                        {
                            isExist = true;
                            friendInfo = temp;
                        }
                    }

                    if (isExist == true)   //如果存在就更新信息
                    {
                        friendInfo.Nickname = nicknames[i];
                        friendInfo.setIcon(icons[i]);
                    }
                    else                   //如果不存在就把它加进去
                    {
                        friendslList.Add(new FriendInfo(uids[i], icons[i], nicknames[i], sex[i], school[i], usernames[i]));
                    }
                    UpdateOnlineFriend();
                }
            }
        }

        //好友在线更新
        public void UpdateOnlineFriend()
        {
            string friendOnlinelist = Config.friendsOnlineList;
            string[] onlineUids = friendOnlinelist.Split(',');
            foreach (var temp in friendslList)
            {
                temp.setOnline(false);
            }

            foreach (var onlineUid in onlineUids)
            {
                FriendInfo friendInfo = null;
                foreach (var temp in friendslList)
                {
                    if (temp.Uid == onlineUid)
                    {
                        friendInfo = temp;
                        friendInfo.setOnline(true);
                        Console.WriteLine(friendslList[0].isOnline);
                    }
                }
            }
        }

        public bool IsFriendExist(string uid)
        {
            foreach (FriendInfo friendInfo in friendslList)
            {
                if (friendInfo.Uid == uid)
                {
                    return true;
                }
            }

            return false;
        }
    }
}