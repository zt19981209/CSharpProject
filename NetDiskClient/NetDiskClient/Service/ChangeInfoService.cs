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
    internal class ChangeInfoService
    {
        private Socket changeSocket = null;
        private Thread td = null;
        private static ChangeInfoService changeInfoService = new ChangeInfoService();

        public Socket ChangeSocket { get => changeSocket; }

        public static ChangeInfoService GetAddFriendService()
        {
            return changeInfoService;
        }

        private ChangeInfoService()
        {
            //创建socket
            changeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //发起建立连接的请求   //云主机外网ip：106.13.44.221 笔记本ip：192.168.137.1 台式电脑ip：172.18.64.161
            IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
            EndPoint point = new IPEndPoint(ipAddress, 10001);
            try
            {
                changeSocket.Connect(point);//通过ip：端口号  定位一个要连接的服务器端

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

            //changeSocket.Send(Encoding.UTF8.GetBytes(Config.self_data_json));
            //while (true)
            //{
            //}
        }
    }
}