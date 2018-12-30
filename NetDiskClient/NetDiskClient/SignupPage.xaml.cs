using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetDiskClient.utils;
using Panuon.UI;

namespace NetDiskClient
{
    /// <summary>
    /// SignupPage.xaml 的交互逻辑
    /// </summary>
    public partial class SignupPage : UserControl
    {
        public SignupPage()
        {
            InitializeComponent();
        }
        private string txtUsernameSingupText;       //用户输入的用户名
        private Socket tcpClient;
#pragma warning disable CS0169 // 从不使用字段“SignupPage.tcpClient2”
        private Socket tcpClient2;
#pragma warning restore CS0169 // 从不使用字段“SignupPage.tcpClient2”
        private Thread tdReceiveFromServer;
#pragma warning disable CS0169 // 从不使用字段“SignupPage.tdReceiveFromServerSignIn”
        private Thread tdReceiveFromServerSignIn;
#pragma warning restore CS0169 // 从不使用字段“SignupPage.tdReceiveFromServerSignIn”

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                //创建socket
                tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //发起建立连接的请求   //云主机外网ip：106.13.44.221 笔记本ip：192.168.137.1 台式电脑ip：172.18.64.161
                IPAddress ipAddress = IPAddress.Parse(Config.ipStr);
                EndPoint point = new IPEndPoint(ipAddress, 9999);
                try
                {
                    tcpClient.Connect(point);//通过ip：端口号  定位一个要连接的服务器端

                    //开启接收来自服务器的线程
                    tdReceiveFromServer = new Thread(ReceiveMessageFromServer);
                    tdReceiveFromServer.Start();
                }
#pragma warning disable CS0168 // 声明了变量“exception”，但从未使用过
                catch (Exception exception)
#pragma warning restore CS0168 // 声明了变量“exception”，但从未使用过
                {
                    Console.WriteLine("未连接到服务器");
                    MessageBox.Show("未与注册服务器连接！！", "警告");
                }
            }).Start();
        }

        private void ReceiveMessageFromServer()
        {
            while (true)
            {
                bool isNotConnected = tcpClient.Poll(10, SelectMode.SelectRead);
                Thread.Sleep(10);
                if (isNotConnected == true)
                {
                    break;
                }
                byte[] data = new byte[2048];
                try
                {
                    int length = tcpClient.Receive(data);
                    string message = Encoding.UTF8.GetString(data, 0, length);

                    Config.loginIn.Dispatcher.Invoke(() =>
                    {
                        PUMessageBox.ShowDialog(message);
                        if (message == "注册成功！")
                        {
                            new FtpClass().newFold(txtUsernameSingupText);    //username
                            Config.loginIn.Rotate0();
                        }
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Config.loginIn.Dispatcher.Invoke(() =>
                    {
                        PUMessageBox.ShowDialog("与注册服务器断开！");
                    });
                }
            }
        }

        private void txtPasswordConfirm_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPasswordSignUp.Password != txtPasswordConfirm.Password)
            {
                txtPasswordConfirm.Background = new SolidColorBrush(Color.FromArgb(20, 230, 35, 35));
                labPasswordAlert.Visibility = Visibility.Visible;
            }
            else
            {
                txtPasswordConfirm.Background = new SolidColorBrush(Colors.White);
                labPasswordAlert.Visibility = Visibility.Collapsed;
            }
        }

        private void btnConfirmSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsernameSingup.Text.Trim().Equals("") == false && txtPasswordSignUp.Password.Trim().Equals("") == false && txtPasswordSignUp.Password == txtPasswordConfirm.Password)
            {
                txtUsernameSingupText = txtUsernameSingup.Text;
                string jsonText = "{\"username\":\"" + txtUsernameSingup.Text + "\",\"password\":\" " + txtPasswordSignUp.Password + " \"}";
                byte[] data = Encoding.UTF8.GetBytes(jsonText);
                tcpClient.Send(data);
                txtPasswordConfirm.Password = "";
                txtPasswordSignUp.Password = "";
                txtUsernameSingup.Text = "";
            }
            else if (txtPasswordSignUp.Password != txtPasswordConfirm.Password)
            {
                MessageBox.Show("两次密码输入不相同！");
            }
            else
            {
                MessageBox.Show("信息不完整！");
            }
        }

        private void Btn_giveUp_Click(object sender, RoutedEventArgs e)
        {
            Config.loginIn.Rotate0();
        }
    }
}