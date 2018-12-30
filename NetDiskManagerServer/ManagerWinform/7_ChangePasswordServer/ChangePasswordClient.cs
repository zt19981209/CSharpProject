using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ManagerWinform._7_ChangePasswordServer
{
    internal class ChangePasswordClient
    {
        private Socket clientSocket = null;
        private Thread td = null;

        public ChangePasswordClient(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            td = new Thread(RecieveMessage);
            td.Start();
        }

        private void RecieveMessage()
        {
            byte[] data = new byte[1024];
            int len = 0;
            try
            {
                while (true)
                {
                    try
                    {
                        len = clientSocket.Receive(data);
                        string json_str = Encoding.UTF8.GetString(data, 0, len);
                        string type = JObject.Parse(json_str)["type"].ToString();
                        string password = JObject.Parse(json_str)["password"].ToString();
                        int uid = Int32.Parse(JObject.Parse(json_str)["uid"].ToString());
                        if (type == "old")
                        {
                            if (DB_Manager.IsPassword(password))
                            {
                                clientSocket.Send(Encoding.UTF8.GetBytes("1"));
                            }
                            else
                            {
                                clientSocket.Send(Encoding.UTF8.GetBytes("0"));
                            }
                        }
                        else if (type == "new")
                        {
                            DB_Manager.SetPassword(uid, password);
                        }
                    }
                    catch (Exception e)
                    {
                        throw;
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
    }
}