using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetDiskClient.utils;
using Newtonsoft.Json.Linq;

namespace NetDiskClient.Service
{
    /// <summary>
    /// 保持与udp服务器连接并更新数据（ip和端口号）
    /// </summary>
    internal class MessageRegService
    {
        //new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private Socket udpServer = null;

        private Thread td;

        public MessageRegService(Socket udpServer)
        {
            this.udpServer = udpServer;
            td = new Thread(Run);
            td.Start();
        }

        //向服务器更新ip和端口号
        public void Run()
        {
            string uid = JObject.Parse(Config.self_data_json)["Uid"].ToString();
            while (true)
            {
                IPEndPoint serverPoint = Config.serverPoint;
                string jsonStr = "{\"type\":\"reg\",\"myUid\":\"" + uid + "\"}";
                byte[] data = Encoding.UTF8.GetBytes(jsonStr);

                //把更新的服务器发送给服务器
                udpServer.SendTo(data, serverPoint);

                Thread.Sleep(8000);
            }
        }
    }
}