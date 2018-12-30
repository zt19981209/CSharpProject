using System;
using System.Collections.Generic;
using System.Linq;
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
using Newtonsoft.Json.Linq;
using Panuon.UI;

namespace NetDiskClient
{
    /// <summary>
    /// ChangeUserInfoWin.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeUserInfoWin : PUWindow
    {
        public ChangeUserInfoWin()
        {
            InitializeComponent();
        }

        private void ChangeData_WIN_Loaded(object sender, RoutedEventArgs e)
        {
            txt_nickname.Text = JObject.Parse(Config.self_data_json)["Nickname"].ToString();
            string a = JObject.Parse(Config.self_data_json)["Sex"].ToString();
            combox_sex.Text = JObject.Parse(Config.self_data_json)["Sex"].ToString();
            btn_confirm.IsEnabled = false;
        }

        private void ChangeData_WIN_Closed(object sender, EventArgs e)
        {
            (this.Owner as PUWindow).IsCoverMaskShow = false;
        }

        private void Btn_refuse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = new byte[2048];
            UserInfo userInfo = new UserInfo();
            userInfo.Nickname = txt_nickname.Text;
            userInfo.Sex = combox_sex.Text;
            userInfo.School = txt_school.Text;
            userInfo.Uid = Config.uid;

            string str_json = JObject.FromObject(userInfo).ToString();
            ChangeInfoService.GetAddFriendService().ChangeSocket.Send(Encoding.UTF8.GetBytes(str_json));
            int len = ChangeInfoService.GetAddFriendService().ChangeSocket.Receive(data);
            string json_str = Encoding.UTF8.GetString(data, 0, len);
            Config.self_data_json = json_str;
            Console.WriteLine("个人资料：" + json_str);

            JObject jo = JObject.Parse(Config.self_data_json);
            string Nickname = jo["Nickname"].ToString();
            string Sex = jo["Sex"].ToString();
            string School = jo["School"].ToString();

            (this.Owner as PersonalDatawin).lab_nicknameBig.Content = Nickname;
            (this.Owner as PersonalDatawin).lab_nickname.Content = Nickname;
            (this.Owner as PersonalDatawin).lab_sex.Content = Sex;
            (this.Owner as PersonalDatawin).lab_school.Content = School;
            (this.Owner as PersonalDatawin).SexImg(Sex);
            Config.mainWindow.lab_nickname.Content = Nickname;
            this.Close();
        }

        private void Txt_school_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_school.Text == "")
            {
                btn_confirm.IsEnabled = false;
            }
            else
            {
                btn_confirm.IsEnabled = true;
            }
        }
    }
}