using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManagerWinform
{
    internal class SignUpClient
    {
        private Socket clientSocket;
        private Thread td;

        public SignUpClient(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            td = new Thread(RecieveMessage);
            td.Start();
        }

        private void RecieveMessage()
        {
            //bool isNotConnected = clientSocket.Poll(10, SelectMode.SelectRead);
            //Thread.Sleep(10);
            //if (isNotConnected == true)
            //{
            //    break;
            //}
            while (true)
            {
                try
                {
                    byte[] data = new byte[2048];
                    int length = clientSocket.Receive(data);
                    string json_str = Encoding.UTF8.GetString(data, 0, length);

                    JObject jo = (JObject)JsonConvert.DeserializeObject(json_str);
                    string username = jo["username"].ToString().Trim();
                    string password = jo["password"].ToString().Trim();

                    Console.WriteLine("获得：用户名：" + username + "  密码：" + password);

                    //检查用户名是否存在
                    bool usernameIsExist = DB_Manager.Inquiry(username);

                    if (usernameIsExist == true)
                    {
                        Console.WriteLine("用户名存在：" + usernameIsExist);
                        clientSocket.Send(Encoding.UTF8.GetBytes("用户名已存在请重新输入！"));
                    }
                    else if (usernameIsExist == false)
                    {
                        Console.WriteLine("用户名存在：" + usernameIsExist);
                        clientSocket.Send(Encoding.UTF8.GetBytes("注册成功！"));
                        DB_Manager.InsertUserInfo(username, password);
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
        }

        public bool Connected
        {
            get { return clientSocket.Connected; }
        }
    }
}