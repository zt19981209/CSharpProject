using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
using Microsoft.Win32;
using NetDiskClient.Service;
using NetDiskClient.utils;
using Newtonsoft.Json.Linq;
using Panuon.UI;
using Path = System.IO.Path;

namespace NetDiskClient
{
    /// <summary>
    /// ChangeIconWin.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeIconWin : PUWindow
    {
        public ChangeIconWin()
        {
            InitializeComponent();
        }

        private Stream fs = null;
        private long contentLen = 0;
        private string fileSourse = null;

        private void Btn_uploadIcon_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图片资源|*.jpg;*.png;*.bmp;*.jpg;|All Files|*.*";
            if (ofd.ShowDialog() == true)
            {
                fileSourse = ofd.FileName;
                img__icon.Source = new BitmapImage(new Uri(fileSourse, UriKind.Absolute));
                fs = ofd.OpenFile();
                btn_Confirm.IsEnabled = true;
            }
        }

        private void Btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            //文件长度
            contentLen = fs.Length;
            //文件内容

            PUMessageBox.ShowAwait("正在上传...");
            //传文件长度
            ChangIconService.GetAddFriendService().ChangeiconSocket.Send(BitConverter.GetBytes(contentLen));
            //接收文件名并把自己uid发送过去
            byte[] data = new byte[512];
            int len = ChangIconService.GetAddFriendService().ChangeiconSocket.Receive(data);
            string filename = Encoding.UTF8.GetString(data, 0, len);
            ChangIconService.GetAddFriendService().ChangeiconSocket.Send(Encoding.UTF8.GetBytes(Config.uid));
            //循环发送文件内容
            while (true)
            {
                byte[] bits = new byte[1024];
                int r = fs.Read(bits, 0, bits.Length);
                if (r <= 0) break; //如果从流中读取完毕,则break;
                ChangIconService.GetAddFriendService().ChangeiconSocket.Send(bits, r, SocketFlags.None);
            }
            fs.Position = 0;

            //把选好的头像拷贝到程序文件中
            try
            {
                File.Copy(fileSourse, "Icon\\" + filename + ".jpg");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Config.mainWindow.Dispatcher.Invoke(() =>
                {
                    PUMessageBox.ShowDialog("上传失败！");
                });
            }

            img__icon.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Icon\\" + filename + ".jpg", UriKind.Absolute));
            ((this.Owner as PersonalDatawin).img_icon.Content as Image).Source = img__icon.Source;
            Config.mainWindow.img_icon.Source = img__icon.Source;
            JObject jo = JObject.Parse(Config.self_data_json);
            jo["Icon"] = filename;
            Config.self_data_json = JObject.FromObject(jo).ToString();
            PUMessageBox.CloseAwait();
            this.Close();
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