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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using NetDiskClient.utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Panuon.UI;

namespace NetDiskClient
{
    /// <summary>
    /// LoginIn.xaml 的交互逻辑
    /// </summary>
    public partial class LoginIn : Window
    {
        public LoginIn()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Config.loginIn = this;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            if (btn != null)
            {
                string s = btn.Content.ToString();
                if (s == "关闭")
                {
                    this.Close();
                }
            }
        }

        public void Rotate180()
        {
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(TimeSpan.FromSeconds(1));

            da.To = 180d;

            this.axr.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
        }

        public void Rotate0()
        {
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(TimeSpan.FromSeconds(1));

            da.To = 0d;

            this.axr.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
        }

        public void close()
        {
            Environment.Exit(0);
        }

        private void LoginIn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}