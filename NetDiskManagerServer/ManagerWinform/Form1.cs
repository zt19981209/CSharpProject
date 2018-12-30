using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagerWinform.UdpServer;
using ManagerWinform._4_AddFriend;
using ManagerWinform._6_ChangeIconServer;
using ManagerWinform._7_ChangePasswordServer;

namespace ManagerWinform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //注册服务器
        private Thread signupTd = null;

        private static List<SignUpClient> signUpClients = new List<SignUpClient>();

        //登录服务器
        private List<Client> clients = new List<Client>();

        private Socket tcpServer;

        //udpServer字段
        private Thread td = null;

        private List<ManagerClient> udpClients = new List<ManagerClient>();

        //搜索好友
        private Thread friendTd = null;

        //修改资料
        private Thread changetd = null;

        //更改头像
        private Thread icontd = null;

        //更改密码
        private Thread passwordtd = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            Config.form1 = this;

            //tcp注册服务

            signupTd = new Thread(SignUpClient);
            signupTd.Start();

            //开启udp获取用户的线程
            td = new Thread(GetClient);
            td.Start();

            //tcp登录服务器
            SignIn();
            //搜索好友服务
            friendTd = new Thread(AddFrienServer);
            friendTd.Start();
            //修改资料服务
            changetd = new Thread(ChangeInfo);
            changetd.Start();
            //修改头像服务
            icontd = new Thread(ChangeIcon);
            icontd.Start();
            //修改密码服务
            passwordtd = new Thread(ChangPassword);
            passwordtd.Start();
        }

        private void ChangPassword()
        {
            Socket iconServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //  云主机内网ip：172.16.0.6 笔记本ip：192.168.137.1
            IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
            EndPoint point = new IPEndPoint(ipAddress, 10004);
            iconServer.Bind(point);
            //监听
            Console.WriteLine("开始监听");
            iconServer.Listen(100); //最多允许100个客户链接

            //获取客户端socket对象
            while (true)
            {
                Socket clientSocket = iconServer.Accept();
                new ChangePasswordClient(clientSocket);
            }
        }

        private void ChangeIcon()
        {
            Socket iconServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //  云主机内网ip：172.16.0.6 笔记本ip：192.168.137.1
            IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
            EndPoint point = new IPEndPoint(ipAddress, 10002);
            iconServer.Bind(point);
            //监听
            Console.WriteLine("开始监听");
            iconServer.Listen(100); //最多允许100个客户链接

            //获取客户端socket对象
            while (true)
            {
                Socket clientSocket = iconServer.Accept();
                new ChangeIconClient(clientSocket);
            }
        }

        private void ChangeInfo()
        {
            Socket changeServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //  云主机内网ip：172.16.0.6 笔记本ip：192.168.137.1
            IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
            EndPoint point = new IPEndPoint(ipAddress, 10001);
            changeServer.Bind(point);
            //监听
            Console.WriteLine("开始监听");
            changeServer.Listen(100); //最多允许100个客户链接

            //获取客户端socket对象
            while (true)
            {
                Socket clientSocket = changeServer.Accept();
                new ChangeClient(clientSocket);
            }
        }

        private void AddFrienServer()
        {
            Socket addServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //  云主机内网ip：172.16.0.6 笔记本ip：192.168.137.1
            IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
            EndPoint point = new IPEndPoint(ipAddress, 10000);
            addServer.Bind(point);
            //监听
            Console.WriteLine("开始监听");
            addServer.Listen(100); //最多允许100个客户链接

            //获取客户端socket对象
            while (true)
            {
                Socket clientSocket = addServer.Accept();
                AddFriendClient client = new AddFriendClient(clientSocket);
                Config.addFriendClients.Add(client);
            }
        }

        private void SignUpClient()
        {
            Socket tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //  云主机内网ip：172.16.0.6 笔记本ip：192.168.137.1
            IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
            EndPoint point = new IPEndPoint(ipAddress, 9999);
            tcpServer.Bind(point);
            //监听
            Console.WriteLine("开始监听");
            tcpServer.Listen(100); //最多允许100个客户链接

            //获取客户端socket对象
            while (true)
            {
                Socket clientSocket = tcpServer.Accept();
                SignUpClient client = new SignUpClient(clientSocket);
                signUpClients.Add(client);
                Console.WriteLine("一个用户正在注册");
            }
        }

        private void GetClient()
        {
            Socket udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpServer.Bind(new IPEndPoint(IPAddress.Parse(Config.ipStr), 6666));
            while (true)
            {
                byte[] data = new byte[1024 * 10];
                EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int len = udpServer.ReceiveFrom(data, ref clientEndPoint);
                string msg = Encoding.UTF8.GetString(data, 0, len);
                Console.WriteLine("客户端udp消息" + msg);
                new ManagerClient(udpServer, msg, clientEndPoint);
            }
        }

        private void SignIn()
        {
            tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 云主机内网ip：172.16.0.6 笔记本ip：192.168.137.1  台式电脑ip：172.18.64.161 宿舍局域网：192.168.137.1
            IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
            EndPoint point = new IPEndPoint(ipAddress, 8888);
            tcpServer.Bind(point);
            //监听
            lab1.Text = "注册服务器开始监听";
            tcpServer.Listen(100); //最多允许100个客户链接

            //获取客户端socket对象
            new Thread(GetClientSocket).Start();

            //获取用户列表
            new Thread(GetClientList).Start();
        }

        private void GetClientSocket()
        {
            while (true)
            {
                Socket clientSocket = tcpServer.Accept();
                Client client = new Client(clientSocket);
                clients.Add(client);
            }
        }

        private void GetClientList()
        {
            while (true)
            {
                foreach (Client client in clients)
                {
                    if (client.User_name.Equals("") == false && client.IsShow == false)
                    {
                        Action deAction = () =>
                        {
                            Label lab = new Label();
                            lab.Text = "用户：" + client.User_name;
                            listView1.Items.Add(lab.Text);
                        };
                        this.Invoke(deAction);
                        client.IsShow = true;
                    }
                }

                Thread.Sleep(500);
            }
        }

        public void GetInfo(string msg)
        {
            Action deAction = () => { this.label1.Text = msg; };
            this.Invoke(deAction);
        }

        public void GetInfo1(string msg)
        {
            Action deAction = () => { this.label2.Text = msg; };
            this.Invoke(deAction);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}