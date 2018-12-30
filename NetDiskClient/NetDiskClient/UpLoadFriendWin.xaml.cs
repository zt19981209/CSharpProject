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
using NetDiskClient.utils;
using Panuon.UI;

namespace NetDiskClient
{
    /// <summary>
    /// UpLoadFriendWin.xaml 的交互逻辑
    /// </summary>
    public partial class UpLoadFriendWin : PUWindow
    {
        public UpLoadFriendWin()
        {
            InitializeComponent();
            listViewFriends.ItemsSource = ManageFriendsList.friendslList;
        }

        private void PUWindow_Closed(object sender, EventArgs e)
        {
            (this.Owner as PUWindow).IsCoverMaskShow = false;
        }

        private void ListViewFriends_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //得到选中好友的信息
            FriendInfo select_friendInfo = listViewFriends.SelectedItem as FriendInfo;
            Config.SelectedName = select_friendInfo.Username;

            this.Close();
        }
    }
}
