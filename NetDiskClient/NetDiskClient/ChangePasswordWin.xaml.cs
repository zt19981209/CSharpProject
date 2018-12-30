using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NetDiskClient.Service;
using NetDiskClient.utils;
using Panuon.UI;

namespace NetDiskClient
{
    /// <summary>
    /// ChangePasswordWin.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePasswordWin : PUWindow
    {
        public ChangePasswordWin()
        {
            InitializeComponent();
        }

        private void Btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = new byte[1024];
            ChangePasswordService.GetAddFriendService().ChangeSocket.Send(Encoding.UTF8.GetBytes("{\"type\":\"old\",\"uid\":\"" + Config.uid + "\",\"password\":\"" + pasOld.Password + "\"}"));
            int len = ChangePasswordService.GetAddFriendService().ChangeSocket.Receive(data);
            string command = Encoding.UTF8.GetString(data, 0, len);
            if (command == "1")
            {
                ChangePasswordService.GetAddFriendService().ChangeSocket.Send(Encoding.UTF8.GetBytes("{\"type\":\"new\",\"uid\":\"" + Config.uid + "\",\"password\":\"" + pasNew.Password + "\"}"));
                PUMessageBox.ShowDialog("密码修改成功！");
            }
            else if (command == "0")
            {
                PUMessageBox.ShowDialog("您输入的密码有误！请重新输入！", "警告");
            }
        }

        private void PUWindow_Closed(object sender, EventArgs e)
        {
            (this.Owner as PUWindow).IsCoverMaskShow = false;
        }

        private void Btn_refuse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}