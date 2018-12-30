using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace ManagerWinform
{
    internal class ManagerClient
    {
        private Socket mySocket = null;
        private string json_msg = null;
        private Thread td;
        private EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

        public ManagerClient(Socket mySocket, string jsonmsg, EndPoint clientEndPoint)
        {
            this.mySocket = mySocket;
            this.json_msg = jsonmsg;
            this.clientEndPoint = clientEndPoint;
            td = new Thread(Run);
            td.Start();
        }

        private void Run()
        {
            JObject jo = JObject.Parse(json_msg);
            string type = jo["type"].ToString();
            if (type == "reg")
            {
                string myUid = jo["myUid"].ToString();
                //更新最新的ip和端口号
                UserOnLineList.GetUserOnLineList().updateOnlineUDP(Int32.Parse(myUid),
                    (clientEndPoint as IPEndPoint).Address.ToString(), (clientEndPoint as IPEndPoint).Port);
            }
            else if (type == "confirm")
            {
                string myUid = jo["myUid"].ToString();
                string toUid = jo["toUid"].ToString();
                string code = jo["code"].ToString();
                //更新最新的ip和端口号
                UserOnLineList.GetUserOnLineList().updateOnlineUDP(Int32.Parse(myUid),
                    (clientEndPoint as IPEndPoint).Address.ToString(), (clientEndPoint as IPEndPoint).Port);
            }
            else if (type == "msg")
            {
                Console.WriteLine("客户端udp消息" + json_msg);
                int myUid = Int32.Parse(jo["myUid"].ToString());
                int toUid = Int32.Parse(jo["toUid"].ToString());
                try
                {
                    //更新最新的ip和端口号
                    UserOnLineList.GetUserOnLineList().updateOnlineUDP(myUid,
                        (clientEndPoint as IPEndPoint).Address.ToString(), (clientEndPoint as IPEndPoint).Port);

                    //获得要接收消息的人
                    UserInfo toUser_Info = UserOnLineList.GetUserOnLineList().GetOnlineUserInfo(toUid);
                    //获得接收消息客户的ip和端口号
                    IPEndPoint toUid_Point = new IPEndPoint(IPAddress.Parse(toUser_Info.Udpip), toUser_Info.Udpport);
                    //准备转发消息到toUid
                    byte[] data = Encoding.UTF8.GetBytes(json_msg);
                    mySocket.SendTo(data, toUid_Point);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    //获得要接收消息的人
                    UserInfo toUser_Info = UserOnLineList.GetUserOnLineList().GetOnlineUserInfo(myUid);
                    //获得接收消息客户的ip和端口号
                    IPEndPoint toUid_Point = new IPEndPoint(IPAddress.Parse(toUser_Info.Udpip), toUser_Info.Udpport);
                    //提示客户端此信息无效
                    mySocket.SendTo(Encoding.UTF8.GetBytes("{\"type\":\"0\"}"), toUid_Point);
                }
            }
        }
    }
}