using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ManagerWinform.UdpServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManagerWinform
{
    internal class Client
    {
        private Socket clientSocket;
        private Thread td;
        private string user_name = "";
        private bool isShow = false;

        public Client(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            td = new Thread(RecieveMessage);
            td.Start();
        }

        public string User_name { get => user_name; }
        public bool IsShow { get => isShow; set => isShow = value; }

        /// <summary>
        /// 处理client与server之间交互的线程
        /// </summary>
        private void RecieveMessage()
        {
            int uid = 0;
            try
            {
                while (true)
                {
                    byte[] data = new byte[2048];
                    int length = clientSocket.Receive(data);
                    string json_str = Encoding.UTF8.GetString(data, 0, length);
                    JObject jo = (JObject)JsonConvert.DeserializeObject(json_str);
                    user_name = jo["username"].ToString().Trim();
                    string username = jo["username"].ToString().Trim();
                    string password = jo["password"].ToString().Trim();

                    //检查用户名是否存在

                    UserService userVerify = new UserService();
                    int regInt = userVerify.loginVertify(username, password);
                    if (regInt > 0)
                    {
                        uid = regInt;
                        UserOnLineList.GetUserOnLineList().regOnline(uid, clientSocket, username);

                        string msg = "{\"state\":1,\"msg\":\"密码正确！\"}";
                        clientSocket.Send(Encoding.UTF8.GetBytes(msg));

                        //一直接受客户端发来的指令
                        byte[] bytes;
                        int len;

                        while (true)
                        {
                            //List<FriendInfo> clientFriendInfos = new UserService().GetFriendList(uid);
                            List<FriendInfo> clientFriendInfos = UserService.GetFriendList(uid);

                            bytes = new byte[2048];

                            len = clientSocket.Receive(bytes);
                            string command = Encoding.UTF8.GetString(bytes, 0, len);

                            if (command.Equals("U0001")) //更新好友列表
                            {
                                //传输好友列表

                                msg = JArray.FromObject(clientFriendInfos).ToString();
                                clientSocket.Send(Encoding.UTF8.GetBytes(msg));
                            }
                            else if (command.Equals("U0002")) //更新好友在线
                            {
                                clientSocket.Send(Encoding.UTF8.GetBytes("1"));
                                len = clientSocket.Receive(bytes);
                                string ids_json = Encoding.UTF8.GetString(bytes, 0, len);
                                string ids = JObject.Parse(ids_json)["friendlist"].ToString();
                                string[] idsArray = ids.Split(',');
                                StringBuilder onlineFriends = new StringBuilder();
                                foreach (string id in idsArray)
                                {
                                    if (id == "")
                                    {
                                        continue;
                                    }
                                    if (UserOnLineList.GetUserOnLineList().isUserOnline(Convert.ToInt32(id)))
                                    {
                                        onlineFriends.Append(id);
                                        onlineFriends.Append(",");
                                    }
                                }

                                if (onlineFriends.Length == 0)
                                {
                                    //无好友在线
                                    clientSocket.Send(Encoding.UTF8.GetBytes("notFound"));
                                }
                                else
                                {
                                    //回执好友在线列表
                                    clientSocket.Send(Encoding.UTF8.GetBytes(onlineFriends.ToString()));
                                }
                            }
                            else if (command.Equals("U0003")) //更新个人资料
                            {
                                UserInfo userInfo = new UserService().GetUserInfo(uid);
                                msg = JObject.FromObject(userInfo).ToString();
                                clientSocket.Send(Encoding.UTF8.GetBytes(msg));
                            }
                            else if (command.Equals("U0004")) //传输好友头像
                            {
                                //clientSocket.Send(Encoding.UTF8.GetBytes(Convert.ToString(clientFriendInfos.Count)));
                                //byte[] da = new byte[1024 * 100];
                                ////传输好友头像
                                //foreach (FriendInfo friendInfo in clientFriendInfos)
                                //{
                                //    clientSocket.Receive(new byte[8]);

                                //    FileStream fs = File.Open("Icon\\" + friendInfo.Icon + ".jpg", FileMode.Open);
                                //    fs.Read(da, 0, da.Length);
                                //    clientSocket.Send(da);
                                //}
                            }
                            else if (command.Equals("Exit")) //退出
                            {
                                UserOnLineList.GetUserOnLineList().Logout(uid);
                                return;
                            }
                        }
                    }
                    else if (regInt == -1)
                    {
                        string msg = "{\"state\":-1,\"msg\":\"用户密码错误！\"}";
                        clientSocket.Send(Encoding.UTF8.GetBytes(msg));
                    }
                    else if (regInt == -2)
                    {
                        string msg = "{\"state\":-2,\"msg\":\"用户名不存在！\"}";
                        clientSocket.Send(Encoding.UTF8.GetBytes(msg));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("与此客户失去链接");
            }
            finally
            {
                UserOnLineList.GetUserOnLineList().Logout(uid);
                clientSocket.Close();
            }
            return;
        }
    }
}