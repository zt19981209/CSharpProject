using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NetDiskClient.utils
{
    internal class Config
    {
        public static string uid = "";
        public static string username = "";
        public static string password = "";

        //好友数据json格式
        public static string friends_json = "";

        //好友uid列表
        public static string friendslist = "";

        //取出好友列表值
        public static void FriendsJsonParse(string friends_json)
        {
            JArray mJObj = JArray.Parse(friends_json);
            StringBuilder strBuilder = new StringBuilder();
            foreach (JObject temp in mJObj)
            {
                strBuilder.Append((string)temp["Uid"]);
                strBuilder.Append(",");
            }

            friendslist = strBuilder.ToString();
        }

        //个人资料存储
        public static string self_data_json = "";

        public static string friendsOnlineList = "";

        //udp的socket
        public static Socket udp_Socket = null;

        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //准备发送信息的好友uid和nickname
        public static string toUid = "";

        //得到主窗口和登录窗口的对象
        public static MainWindow mainWindow = null;

        //登录窗口对象
        public static LoginIn loginIn = null;

        //服务器ip字符串
        //192.168.137.1        106.13.44.221
        public static string ipStr = "106.13.44.221";

        //服务器udp的ip和端口号
        public static IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse(ipStr), 6666);

        //要分享的用户名
        public static string SelectedName = null;

        //要分享的文件
        public static string SelectedFiPath = null;

        //要存到的文件夹
        public static string SelectedToSavePath = null;

        //输入的文件名
        public static string NAmenamename = null;

    }
}