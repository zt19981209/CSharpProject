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
using Newtonsoft.Json.Linq;
using Panuon.UI;

namespace NetDiskClient
{
    /// <summary>
    /// PersonalDatawin.xaml 的交互逻辑
    /// </summary>
    public partial class PersonalDatawin : PUWindow
    {
        public PersonalDatawin()
        {
            InitializeComponent();
        }

        private void PersonnalData_win_Loaded(object sender, RoutedEventArgs e)
        {
            JObject jo = JObject.Parse(Config.self_data_json);
            string Uid = jo["Uid"].ToString();
            string Usename = jo["Usename"].ToString();
            string Icon = jo["Icon"].ToString();
            string Nickname = jo["Nickname"].ToString();
            string Maxnumber = jo["Maxnumber"].ToString();
            string Sex = jo["Sex"].ToString();
            SexImg(Sex);
            string School = jo["School"].ToString();

            lab_nicknameBig.Content = Nickname;
            lab_uid.Content = Uid;
            lab_username.Content = Usename;
            lab_nickname.Content = Nickname;
            lab_capacity.Content = Maxnumber;
            lab_sex.Content = Sex;
            lab_school.Content = School;
            (img_icon.Content as Image).Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Icon\\" + Icon + ".jpg", UriKind.Absolute));

            lab_size.Content = FtpClass.SSSSize;
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

        private void PersonnalData_win_Closed(object sender, EventArgs e)
        {
            Config.mainWindow.IsCoverMaskShow = false;
        }

        private void Btn_changedata_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeUserInfoWin changeUserInfoWin = new ChangeUserInfoWin();
            changeUserInfoWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            changeUserInfoWin.Owner = this;
            this.IsCoverMaskShow = true;
            changeUserInfoWin.ShowDialog();
        }

        private void Btn_ChangeIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeIconWin changeIcon = new ChangeIconWin();
            changeIcon.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            changeIcon.Owner = this;
            this.IsCoverMaskShow = true;
            changeIcon.ShowDialog();
           
        }
    }
}