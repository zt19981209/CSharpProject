using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ManagerWinform.UdpServer;
using Newtonsoft.Json.Linq;

namespace ManagerWinform._6_ChangeIconServer
{
    internal class ChangeIconClient
    {
        private Socket clientSocket = null;
        private Thread td = null;

        public ChangeIconClient(Socket clientSocket)
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
                    try
                    {
                        byte[] bitLen = new byte[8];
                        clientSocket.Receive(bitLen, bitLen.Length, SocketFlags.None);
                        //第一步接收文件的大小
                        long contentLen = BitConverter.ToInt64(bitLen, 0);

                        int size = 0;
                        string iconname = File.ReadAllText("Icon.txt");
                        //接收用户uid
                        clientSocket.Send(Encoding.UTF8.GetBytes(iconname));
                        int len = clientSocket.Receive(bitLen);
                        int uid = Int32.Parse(Encoding.UTF8.GetString(bitLen, 0, len));

                        FileStream ms = new FileStream("Icon\\" + iconname + ".jpg", FileMode.Create);
                        int num = Int32.Parse(iconname) + 1;
                        File.WriteAllText("Icon.txt", Convert.ToString(num));

                        //把头像名写入数据库
                        UserService.ChangeUserIcon(uid, iconname);
                        //循环接收文件的内容,如果接收完毕,则break;
                        while (size < contentLen)
                        {
                            //分多次接收,每次接收1024个字节,
                            byte[] bits = new byte[1024];
                            int r = clientSocket.Receive(bits, bits.Length, SocketFlags.None);
                            if (r <= 0) break;
                            ms.Write(bits, 0, r);
                            size += r;
                        }

                        ms.Position = 0;
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