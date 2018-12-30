using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NetDiskClient.utils;
using Newtonsoft.Json.Linq;
using Panuon.UI;

namespace NetDiskClient.Service
{
    /// <summary>
    /// 接收服务器的中转消息
    /// </summary>
    internal class MessageService
    {
        //new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private Socket udpServer = null;

        private Thread td;

        public MessageService(Socket udpServer)
        {
            this.udpServer = udpServer;
            td = new Thread(Run);
            td.Start();
        }

        public void Run()
        {
            while (true)
            {
                EndPoint serverPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = new byte[1024 * 32];
                string ipstr = Config.GetLocalIP();
                Console.WriteLine(ipstr);
                // udpServer.Bind(new IPEndPoint(IPAddress.Parse("10.132.1.50")));
                int len = udpServer.ReceiveFrom(data, ref serverPoint);

                string msg = Encoding.UTF8.GetString(data, 0, len);

                JObject jo = JObject.Parse(msg);
                string str = jo["type"].ToString();
                if (str == "0")
                {
                    MessageBox.Show("消息发送失败！");
                }
                else if (str == "msg")
                {
                    //接收服务器的中转消息
                    MessagePool.GetMessagePool().AddMessage(msg);
                }
            }
        }
    }
}