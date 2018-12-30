using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ManagerWinform.UdpServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManagerWinform._4_AddFriend
{
    internal class AddFriendClient
    {
        public string uid = null;
        private string username = null;
        private string nickname = null;
        private string icon = null;

        private int frienduid = -1;
        public Socket clientSocket;
        private Thread td;

        public AddFriendClient(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            td = new Thread(RecieveMessage);
            td.Start();
        }

        private void RecieveMessage()
        {
            try
            {
                //接收来自uid和其他自身信息
                byte[] data = new byte[1024 * 3];
                int length = clientSocket.Receive(data);
                string str_json = Encoding.UTF8.GetString(data, 0, length);
                JObject jo = JObject.Parse(str_json);
                uid = jo["Uid"].ToString();
                username = jo["Usename"].ToString();
                nickname = jo["Nickname"].ToString();
                icon = jo["Icon"].ToString();
                while (true)
                {
                    //准备接搜索好友的信息
                    length = clientSocket.Receive(data);
                    str_json = Encoding.UTF8.GetString(data, 0, length).Trim();
                    jo = JObject.Parse(str_json);
                    string type = jo["type"].ToString();
                    if (type == "search")
                    {
                        //从数据库中搜索用户
                        string str = jo["userid"].ToString();
                        bool usernameIsExist = DB_Manager.Inquiry(str);

                        if (usernameIsExist == true)
                        {
                            UserJsonInfo searchFriendInfo = new UserJsonInfo();
                            searchFriendInfo.type = "searchFriend";
                            searchFriendInfo.username = DB_Manager.InquiryUsername(str, out searchFriendInfo.nickname, out searchFriendInfo.icon, out frienduid);
                            searchFriendInfo.uid = Convert.ToString(frienduid);
                            searchFriendInfo.exist = "1";
                            clientSocket.Send(Encoding.UTF8.GetBytes(JObject.FromObject(searchFriendInfo).ToString()));
                        }
                        else if (usernameIsExist == false)
                        {
                            //用户名未找到，开始寻找昵称
                            bool isExist = DB_Manager.InquiryNickname(str);
                            if (isExist == true)
                            {
                                UserJsonInfo searchFriendInfo = new UserJsonInfo();
                                searchFriendInfo.type = "searchFriend";
                                searchFriendInfo.nickname = DB_Manager.InquiryByNickname(str, out searchFriendInfo.username, out searchFriendInfo.icon, out frienduid);
                                searchFriendInfo.exist = "1";
                                clientSocket.Send(Encoding.UTF8.GetBytes(JObject.FromObject(searchFriendInfo).ToString()));
                            }
                            else
                            {
                                UserJsonInfo searchFriendInfo = new UserJsonInfo();
                                searchFriendInfo.type = "searchFriend";
                                searchFriendInfo.exist = "0";
                                clientSocket.Send(Encoding.UTF8.GetBytes(JObject.FromObject(searchFriendInfo).ToString()));
                            }
                        }
                    }
                    else if (type == "add")
                    {
                        foreach (AddFriendClient addFriendClient in Config.addFriendClients)
                        {
                            string a = "";
                            if (addFriendClient.uid != null)
                            {
                                Config.form1.GetInfo(addFriendClient.uid + a);
                                a = addFriendClient.uid;
                            }
                        }

                        try
                        {
                            UserJsonInfo userJsonInfo = new UserJsonInfo();
                            userJsonInfo.type = "add";
                            userJsonInfo.uid = uid;
                            userJsonInfo.username = username;
                            userJsonInfo.nickname = nickname;
                            userJsonInfo.icon = icon;
                            userJsonInfo.exist = "1";
                            str_json = JObject.FromObject(userJsonInfo).ToString();

                            Dictionary<string, AddFriendClient> clientDictionary = new Dictionary<string, AddFriendClient>();

                            foreach (AddFriendClient addFriendClient in Config.addFriendClients)
                            {
                                if (addFriendClient.uid != null)
                                {
                                    //防止旧的已经断线socket对象也被加入到clientDic中
                                    try
                                    {
                                        clientDictionary.Add(addFriendClient.uid, addFriendClient);
                                    }
                                    catch (Exception e)
                                    {
                                        clientDictionary.Remove(addFriendClient.uid);
                                        clientDictionary.Add(addFriendClient.uid, addFriendClient);
                                        Console.WriteLine(e);
                                    }
                                }
                            }
                            clientDictionary[Convert.ToString(frienduid)].clientSocket.Send(Encoding.UTF8.GetBytes(str_json));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                    else if (type == "verify")
                    {
                        if (jo["choice"].ToString() == "1")
                        {
                            DB_Manager.AddFriend(Int32.Parse(jo["uid"].ToString()), Int32.Parse(jo["frienduid"].ToString()));
                        }
                        else if (jo["choice"].ToString() == "0")
                        {
                        }
                    }
                    else if (type == "delete")
                    {
                        int uid = Int32.Parse(jo["uid"].ToString());
                        int frienduid = Int32.Parse(jo["frienduid"].ToString());
                        DB_Manager.DeleteFriend(uid, frienduid);
                        //双方都要删除
                        DB_Manager.DeleteFriend(frienduid, uid);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                clientSocket.Close();
            }
            return;
        }

        public bool Connected
        {
            get { return clientSocket.Connected; }
        }
    }
}