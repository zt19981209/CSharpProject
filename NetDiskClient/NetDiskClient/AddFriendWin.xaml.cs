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
    /// AddFriendWin.xaml 的交互逻辑
    /// </summary>
    public partial class AddFriendWin : PUWindow
    {
        public AddFriendWin()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Owner = Config.mainWindow;
        }

        private void Btn_search_Click(object sender, RoutedEventArgs e)
        {
            AddFriendService addFriendService = AddFriendService.GetAddFriendService();
            string str_json = "{\"type\":\"search\",\"userid\":\"" + txt_username.Text + "\"}";
            addFriendService.AddSocket.Send(Encoding.UTF8.GetBytes(str_json));
        }

        public string searcheduid = null;

        private void Btn_addFriend_Click(object sender, RoutedEventArgs e)
        {
            ManageFriendsList manage = new ManageFriendsList();
            manage.IsFriendExist(searcheduid);
            if (manage.IsFriendExist(searcheduid) == false)
            {
                AddFriendService addFriendService = AddFriendService.GetAddFriendService();
                string str_json = "{\"type\":\"add\",\"uid\":\"" + Config.uid + "\",\"frienduid\":\"\"}";
                addFriendService.AddSocket.Send(Encoding.UTF8.GetBytes(str_json));

                PUMessageBox.ShowDialog("已发送验证请求！");
            }
            else
            {
                PUMessageBox.ShowDialog("该用户已添加!", "警告");
            }
        }

        private void AddFriendWin1_Closed(object sender, EventArgs e)
        {
            Config.mainWindow.IsCoverMaskShow = false;
        }
    }
}