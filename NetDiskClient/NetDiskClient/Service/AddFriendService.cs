using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using NetDiskClient.utils;
using Newtonsoft.Json.Linq;
using Panuon.UI;

namespace NetDiskClient.Service
{
    internal class AddFriendService
    {
        private Socket addSocket = null;
        private Thread td = null;
        private static AddFriendService addFriendService = new AddFriendService();

        public Socket AddSocket { get => addSocket; }

        public static AddFriendService GetAddFriendService()
        {
            return addFriendService;
        }

        private AddFriendService()
        {
            //创建socket
            addSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //发起建立连接的请求   //云主机外网ip：106.13.44.221 笔记本ip：192.168.137.1 台式电脑ip：172.18.64.161
            IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
            EndPoint point = new IPEndPoint(ipAddress, 10000);
            try
            {
                addSocket.Connect(point);//通过ip：端口号  定位一个要连接的服务器端

                //开启接收来自服务器的线程
                td = new Thread(ReceiveMessageFromServer);
                td.Start();
            }
#pragma warning disable CS0168 // 声明了变量“exception”，但从未使用过
            catch (Exception exception)
#pragma warning restore CS0168 // 声明了变量“exception”，但从未使用过
            {
                Console.WriteLine("未连接到服务器");
                MessageBox.Show("未与注册服务器连接！！", "警告");
            }
        }

        private void ReceiveMessageFromServer()
        {
            Thread.Sleep(1500);

            addSocket.Send(Encoding.UTF8.GetBytes(Config.self_data_json));
            while (true)
            {
                bool isNotConnected = addSocket.Poll(10, SelectMode.SelectRead);
                Thread.Sleep(10);
                if (isNotConnected == true)
                {
                    break;
                }

                byte[] data = new byte[1024 * 3];
                //try
                //{
                int length = addSocket.Receive(data);
                string message = Encoding.UTF8.GetString(data, 0, length);
                JObject jo = null;
                try
                {
                    jo = JObject.Parse(message);
                }
                catch (Exception e)
                {
                    Config.mainWindow.Dispatcher.Invoke(() =>
                    {
                        PUMessageBox.ShowDialog("添加失败，请从新添加！");
                    });

                    continue;
                }

                string type = jo["type"].ToString();
                if (type == "searchFriend")
                {
                    string uid = jo["uid"].ToString();
                    string username = jo["username"].ToString();
                    string nickname = jo["nickname"].ToString();
                    string iconstr = jo["icon"].ToString();
                    string exist = jo["exist"].ToString();
                    if (exist == "1")
                    {
                        Config.mainWindow.addFriendWin.Dispatcher.Invoke(() =>
                        {
                            //new BitmapImage(new Uri("Icon\\" + iconstr + ".jpg", UriKind.RelativeOrAbsolute));
                            Config.mainWindow.addFriendWin.img_addicon.Source = new BitmapImage(new Uri("http://106.13.44.221:80/" + iconstr + ".jpg", UriKind.RelativeOrAbsolute));
                            Config.mainWindow.addFriendWin.lab_addnickname.Content = "昵  称：" + nickname;
                            Config.mainWindow.addFriendWin.lab_addusername.Content = "用户名：" + username;
                            Config.mainWindow.addFriendWin.searcheduid = uid;
                        });
                    }

                    if (exist == "0")
                    {
                        Config.mainWindow.Dispatcher.Invoke(() => { PUMessageBox.ShowDialog("没有找到此用户！"); });
                    }
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e);
                    //    MessageBox.Show("与注册服务器断开！");
                    //}
                }
                else if (type == "add")
                {
                    string username = jo["username"].ToString();
                    string nickname = jo["nickname"].ToString();
                    string iconstr = jo["icon"].ToString();
                    string exist = jo["exist"].ToString();

                    Config.mainWindow.Dispatcher.Invoke(() =>
                    {
                        VerifyFriendWin verifyFriendWin = new VerifyFriendWin();
                        verifyFriendWin.frienduid = jo["uid"].ToString();
                        verifyFriendWin.lab_friendusername.Content = "用户名：" + username;
                        verifyFriendWin.lab_friendnickname.Content = "昵  称：" + nickname;
                        verifyFriendWin.img_friendicon.Source = new BitmapImage(new Uri("http://106.13.44.221:80/" + iconstr + ".jpg", UriKind.RelativeOrAbsolute));
                        verifyFriendWin.ShowDialog();
                    });
                }
            }
        }
    }
}