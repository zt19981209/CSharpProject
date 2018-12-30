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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetDiskClient.utils;

namespace NetDiskClient
{
    /// <summary>
    /// SignInPage.xaml 的交互逻辑
    /// </summary>
    public partial class SignInPage : UserControl
    {
        public SignInPage()
        {
            InitializeComponent();
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtpassword.Password.Trim();
            if (username.Trim() == "" || password.Trim() == "")
            {
                MessageBox.Show("请填写用户名或密码！");
                return;
            }

            Config.username = username;
            Config.password = password;
            try
            {
                NetService.GetNetService().Login(Config.loginIn);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("网络连接失败");
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Config.loginIn.Rotate180();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Config.loginIn.close();
        }
    }
}