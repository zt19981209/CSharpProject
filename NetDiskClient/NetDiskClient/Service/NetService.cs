using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NetDiskClient.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetDiskClient.utils
{
    /// <summary>
    /// 与服务器保持连接
    /// </summary>
    internal class NetService
    {
        private NetService()
        {
        }

        private static NetService netService = new NetService();

        public static NetService GetNetService()
        {
            return netService;
        }

        public Socket socket = null;
        private Thread thread = null;
        private bool run = false;
        private LoginIn login;

        /// <summary>
        /// 登录函数
        /// </summary>
        public void Login(LoginIn login)
        {
            this.login = login;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //发起建立连接的请求   ///云主机外网ip：106.13.44.221 笔记本ip：192.168.137.1 台式电脑ip：172.18.64.161 宿舍局域网：192.168.137.1
                IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
                EndPoint point = new IPEndPoint(ipAddress, 8888);
                socket.Connect(point);//通过ip：端口.号  定位一个要连接的服务器端
                string json_str = "{\"username\":\"" + Config.username + "\",\"password\":\"" + Config.password + "\"}";

                //传递登录的用户名和密码
                socket.Send(Encoding.UTF8.GetBytes(json_str));

                //接收来自服务器的登录反馈
                byte[] data = new byte[2048];
                int length = socket.Receive(data);
                string message = Encoding.UTF8.GetString(data, 0, length);
                JObject jo = (JObject)JsonConvert.DeserializeObject(message);
                int state = (int)jo["state"];
                string msg = jo["msg"].ToString().Trim();
                if (state == 1 && msg == "密码正确！")
                {
                    this.login.Visibility = Visibility.Collapsed;//让登录界面消失
                    //开启持续与服务器的通讯线程
                    if (thread != null)
                    {
                        //如何线程已经开启就将其关闭
                        if (thread.ThreadState == ThreadState.Running)
                        {
                            run = false;
                            thread.Abort();
                        }
                    }
                    //开启线程
                    run = true;
                    thread = new Thread(ReceiveMessageFromServer);
                    thread.Start();
                }
                else if (state == -1)
                {
                    MessageBox.Show(msg);
                }
                else if (state == -2)
                {
                    MessageBox.Show(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ReceiveMessageFromServer()
        {
            try
            {
                byte[] bytes = new byte[1024 * 100];
                int len = 0;
                ///////////////////////////////////////////////////获得好友头像
                //socket.Send(Encoding.UTF8.GetBytes("U0004"));
                //len = socket.Receive(bytes);
                //string nstr = Encoding.UTF8.GetString(bytes, 0, len);
                //int num = Int32.Parse(nstr);
                //for (int i = 0; i < num; i++)
                //{
                //    socket.Send(Encoding.UTF8.GetBytes("1"));

                //    len = socket.Receive(bytes);
                //    string filename = Encoding.UTF8.GetString(bytes, 0, len);
                //    FileStream ms = new FileStream("Icon\\" + 00 + ".jpg", FileMode.Create);
                //    ms.Write(bytes, 0, len);
                //    ms.Position = 0;
                //}

                ////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////好友信息获得
                socket.Send(Encoding.UTF8.GetBytes("U0001"));

                len = socket.Receive(bytes);
                string json_str = Encoding.UTF8.GetString(bytes, 0, len);
                Config.friends_json = json_str;
                Console.WriteLine("好友资料：" + json_str);
                //解析好友编号
                Config.FriendsJsonParse(json_str);
                //////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////个人资料的获得
                socket.Send(Encoding.UTF8.GetBytes("U0003"));
                bytes = new byte[2048];
                len = socket.Receive(bytes);
                json_str = Encoding.UTF8.GetString(bytes, 0, len);
                Config.self_data_json = json_str;
                Console.WriteLine("个人资料：" + json_str);
                ///////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////启动连接Udp服务
                //创建udp服务器
                Config.udp_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //启动udp更新数据服务
                new MessageRegService(Config.udp_Socket);
                //启动接收消息服务
                new MessageService(Config.udp_Socket);
                /////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////启动搜索好友服务
                AddFriendService.GetAddFriendService();
                //////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////启动修改资料服务
                ChangeInfoService.GetAddFriendService();
                //////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////启动修改头像服务
                ChangIconService.GetAddFriendService();
                //////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////启动修改密码服务
                ChangePasswordService.GetAddFriendService();
                //////////////////////////////////////////////////////////////
                //打开主界面
                login.Dispatcher.Invoke(delegate ()
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                });

                //好友列表和好友在线的实时更新
                while (run)
                {
                    socket.Send(Encoding.UTF8.GetBytes("U0001"));
                    bytes = new byte[1024 * 10];
                    len = socket.Receive(bytes);
                    json_str = Encoding.UTF8.GetString(bytes, 0, len);
                    Config.friends_json = json_str;
                    Config.FriendsJsonParse(json_str);
                    Console.WriteLine("好友资料：" + json_str);

                    socket.Send(Encoding.UTF8.GetBytes("U0002"));
                    socket.Receive(bytes);
                    Friendlist_json json = new Friendlist_json();
                    json.friendlist = Config.friendslist;
                    string friendslist_json = JObject.FromObject(json).ToString();
                    socket.Send(Encoding.UTF8.GetBytes(friendslist_json));
                    len = socket.Receive(bytes);
                    string msg = Encoding.UTF8.GetString(bytes, 0, len);
                    Config.friendsOnlineList = msg;
                    Console.WriteLine("在线好友：" + msg);
                    Thread.Sleep(5000);
                }
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
            {
                run = false;
            }
        }
    }
}