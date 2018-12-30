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
    /// FriendInfoWin.xaml 的交互逻辑
    /// </summary>
    public partial class FriendInfoWin : PUWindow
    {
        public FriendInfoWin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                FriendInfo friendInfo = (Config.mainWindow.listViewFriends.SelectedItem as FriendInfo);
                lab_nicknameBig.Content = friendInfo.Nickname;
                lab_uid.Content = friendInfo.Uid;
                lab_username.Content = friendInfo.Username;
                lab_nickname.Content = friendInfo.Nickname;
                lab_sex.Content = friendInfo.Sex;
                lab_school.Content = friendInfo.School;
                SexImg(friendInfo.Sex);
                (img_icon.Content as Image).Source =
                    new BitmapImage(new Uri("http://106.13.44.221:80/" + friendInfo.Iconstring + ".jpg", UriKind.RelativeOrAbsolute));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                PUMessageBox.ShowDialog("加载失败，请从新打开！", "警告");
                this.Close();
            }
        }

        public void SexImg(string sex)
        {
            if (sex == "保密")
            {
                (img_sex.Content as Image).Source = new BitmapImage(new Uri("Image\\" + 10 + ".png", UriKind.Relative));
            }
            else if (sex == "汉子")
            {
                (img_sex.Content as Image).Source = new BitmapImage(new Uri("Image\\" + 8 + ".png", UriKind.Relative));
            }
            else if (sex == "妹子")
            {
                (img_sex.Content as Image).Source = new BitmapImage(new Uri("Image\\" + 9 + ".png", UriKind.Relative));
            }
        }

        private void PUWindow_Closed(object sender, EventArgs e)
        {
            (this.Owner as PUWindow).IsCoverMaskShow = false;
            this.Close();
        }
    }
}