using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ManagerWinform
{
    internal class ChangeClient
    {
        private Socket clientSocket = null;
        private Thread td = null;

        public ChangeClient(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            td = new Thread(RecieveMessage);
            td.Start();
        }

        private void RecieveMessage()
        {
            try
            {
                while (true)
                {
                    byte[] by = new byte[1024 * 3];
                    int len = clientSocket.Receive(by);
                    string te = Encoding.UTF8.GetString(by, 0, len);
                    JObject jo1 = JObject.Parse(te);
                    int Uid = Int32.Parse(jo1["Uid"].ToString());
                    string nickname = jo1["Nickname"].ToString();
                    string sex = jo1["Sex"].ToString();
                    string school = jo1["School"].ToString();
                    new UserService().ChangeUserInfo(Uid, nickname, sex, school);

                    UserInfo userInfo = new UserService().GetUserInfo(Uid);
                    string msg = JObject.FromObject(userInfo).ToString();
                    clientSocket.Send(Encoding.UTF8.GetBytes(msg));
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
}