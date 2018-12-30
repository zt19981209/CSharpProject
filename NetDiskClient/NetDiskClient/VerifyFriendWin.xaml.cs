using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using NetDiskClient.Service;
using NetDiskClient.utils;
using Panuon.UI;

namespace NetDiskClient
{
    /// <summary>
    /// VerifyFriendWin.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyFriendWin : PUWindow
    {
        public VerifyFriendWin()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Owner = Config.mainWindow;
        }

        public string frienduid = null;

        private void Btn_agree_Click(object sender, RoutedEventArgs e)
        {
            AddFriendService addFriendService = AddFriendService.GetAddFriendService();
            string str_json = "{\"type\":\"verify\",\"uid\":\"" + Config.uid + "\",\"frienduid\":\"" + frienduid + "\",\"choice\":\"1\"}";
            addFriendService.AddSocket.Send(Encoding.UTF8.GetBytes(str_json));

            this.Close();
        }

        private void Btn_refuse_Click(object sender, RoutedEventArgs e)
        {
            AddFriendService addFriendService = AddFriendService.GetAddFriendService();
            string str_json = "{\"type\":\"verify\",\"uid\":\"" + Config.uid + "\",\"frienduid\":\"" + frienduid + "\",\"choice\":\"0\"}";
            addFriendService.AddSocket.Send(Encoding.UTF8.GetBytes(str_json));
            this.Close();
        }
    }
}