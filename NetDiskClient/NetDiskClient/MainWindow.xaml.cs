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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using NetDiskClient.Service;
using NetDiskClient.utils;
using Newtonsoft.Json.Linq;
using Panuon.UI;

namespace NetDiskClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : PUWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //显示ftp站点文件
        private List<FileManager> files = new List<FileManager>();

        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            //Image img = new Image();
            //img.Source = new BitmapImage(new Uri("Image\\11.png", UriKind.Relative));
            //this.Icon = img;

            //把主窗口的对象传进Config
            Config.mainWindow = this;
            UpdataSelfData();//更新个人资料

            //ftp配置
            string ftpFilePath = lab_ftpPath.Content.ToString();
            GetFilesByPath(ftpFilePath);

            //好友列表
            ManageFriendsList manageFriendsList = new ManageFriendsList();
            manageFriendsList.UpdateFriendsList();
            listViewFriends.ItemsSource = ManageFriendsList.friendslList;
            new Thread(() =>
            {
                while (true)
                {
                    this.Dispatcher.Invoke(delegate ()
                    {
                        manageFriendsList.UpdateFriendsList();
                        listViewFriends.ItemsSource = null;
                        listViewFriends.ItemsSource = ManageFriendsList.friendslList;
                    });
                    Thread.Sleep(5000);
                }
            }).Start();
        }

        private void GetFilesByPath(string ftpFilePath)
        {
            files.RemoveRange(0, files.Count);
            //文件信息加载
            string[][] ss = ftpClass.getFileList(ftpFilePath);

            for (int i = 0; i < ss.Length; i++)
            {
                bool aaa = (ss[i] != null);
                if (ss[i] != null)
                {
                    FileManager fm = new FileManager();
                    fm.fileName = ss[i][0];
                    fm.fileSize = ss[i][1];
                    if (ss[i][2] != "文件夹")
                        fm.fileType = ss[i][2] + "文件类型";
                    else
                        fm.fileType = ss[i][2];
                    fm.fileDate = ss[i][3];
                    fm.Icon = new BitmapImage(new Uri("Image\\" + ss[i][2] + ".jpg", UriKind.Relative));

                    if (fm.Icon == null)
                    {
                        fm.Icon = new BitmapImage(new Uri("Image\\未知.jpg", UriKind.Relative));
                    }
                    files.Add(fm);
                }
                else
                {
                    break;
                }
            }

            FileManager fm2 = new FileManager();

            listView.ItemsSource = files;
        }

        /// <summary>
        /// 更新个人资料
        /// </summary>
        private string myUid = "";

        private string stricon = null;

        private void UpdataSelfData()
        {
            string strjson = Config.self_data_json;
            JObject jo = JObject.Parse(strjson);
            myUid = jo["Uid"].ToString().Trim();
            Config.uid = myUid;
            stricon = jo["Icon"].ToString().Trim();
            string nickname = jo["Nickname"].ToString().Trim();
            lab_nickname.Content = nickname;

            img_icon.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Icon\\" + stricon + ".jpg", UriKind.Absolute));
        }

        private void MainWin_Closed(object sender, EventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void LabUploading_MouseEnter(object sender, MouseEventArgs e)
        {
            Label lab = (Label)sender;

            lab.BorderBrush = new SolidColorBrush(Colors.Gray);
            lab.BorderThickness = new Thickness(2);
            lab.Foreground = new SolidColorBrush(Colors.Blue);
        }

        private void LabUploading_MouseLeave(object sender, MouseEventArgs e)
        {
            Label lab = (Label)sender;
            lab.BorderBrush = null;
            lab.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void MenuItem_exit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

        //jhgdssjjiik
        private void ListViewFriends_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //解锁发送键
                btn_sendmsg.IsEnabled = true;
                //得到选中好友的信息
                FriendInfo select_friendInfo = listViewFriends.SelectedItem as FriendInfo;
                labNickname_chatframe.Content = select_friendInfo.Nickname;
                Config.toUid = select_friendInfo.Uid;
                // http://106.13.44.221:80/5.jpg    //Environment.CurrentDirectory + "\\Icon\\" + select_friendInfo.Iconstring + ".jpg"
                imgIcon_chatframe.Source = new BitmapImage(new Uri("http://106.13.44.221:80/" + select_friendInfo.Iconstring + ".jpg", UriKind.RelativeOrAbsolute));
            }
            catch (Exception exception)
            {
            }
        }

        private void Btn_sendmsg_Click(object sender, RoutedEventArgs e)
        {
            //{"type":"msg","toUid":" 17354422654  ","myUid":"17354422653","msg":""}
            //"{\"type\":\"msg\",\"toUid\":\"" + Config.toUid + "\",\"myUid\":\"" + myUid + "\",\"msg\":\"" + msg + "\",\"code\":\"\"}"
            Msg msg = new Msg();
            msg.type = "msg";
            msg.toUid = Config.toUid;
            msg.myUid = myUid;
            msg.icon = stricon;
            msg.nickname = JObject.Parse(Config.self_data_json)["Nickname"].ToString();
            msg.msg = txt_msg.Text;
            msg.code = DateTime.Now.ToString();

            string json_str = JObject.FromObject(msg).ToString();
            byte[] data = Encoding.UTF8.GetBytes(json_str);
            Config.udp_Socket.SendTo(data, Config.serverPoint);

            //发送消息完毕让发送框内的信息消失
            txt_msg.Text = "";
            //自己发送的消息也要呈现出来
            AddMessageS(msg);
        }

        /// <summary>
        /// 添加自己信息在聊天框
        /// </summary>
        /// <param name="msg"></param>
        private void AddMessageS(Msg msg)
        {
            JObject jo = JObject.Parse(Config.self_data_json);
            string nickname = jo["Nickname"].ToString().Trim();
            // string str = "\n" + nickname + "\t" + DateTime.Now.ToString() + "\n" + msg.msg + "\n";
            if (msg.msg == "")
                return;
            stkMain.Children.Add(GetBubbleGrid(msg.msg, stricon));
            //txt_Receive.Text = txt_Receive.Text + str;
        }

        /// <summary>
        /// 添加好友消息在聊天框
        /// </summary>
        /// <param name="msg"></param>
        public void AddMessageF(Msg msg)
        {
            Config.mainWindow.Dispatcher.Invoke(() =>
            {
                string str = "\n" + msg.nickname + "\t" + DateTime.Now.ToString() + "\n" + msg.msg + "\n";

                stkMain.Children.Add(GetBubbleGrid(msg.msg, msg.icon, true, true));
                svMain.ScrollToBottom();
                //txt_Receive.Text = txt_Receive.Text + str;
            });
        }

        /// <summary>
        /// 添加好友
        /// </summary>
        public AddFriendWin addFriendWin = null;  //好友窗口
        public RenameWin renameWin = null; //明明窗口

        private void MenuItem_addFriend_Click(object sender, RoutedEventArgs e)
        {
            if (addFriendWin == null || addFriendWin.IsVisible == false)
            {
                this.IsCoverMaskShow = true;
                addFriendWin = new AddFriendWin();
                addFriendWin.Owner = this;
                addFriendWin.ShowDialog();
            }
            else
            {
                addFriendWin.Activate();
                addFriendWin.WindowState = WindowState.Normal;
            }
        }

        private double _height = 40;

        private Grid GetBubbleGrid(string content, string icon, bool isTopOne = false, bool isSystemMsg = false)
        {
            var grd = new Grid()
            {
                Margin = isTopOne ? new System.Windows.Thickness(0, 20, 0, 0) : new System.Windows.Thickness(0, 10, 0, 0),
                MinHeight = _height,
                Opacity = 0,
            };

            var img = new Image()
            {
                Source = new BitmapImage(new Uri("http://106.13.44.221:80/" + icon + ".jpg", UriKind.RelativeOrAbsolute)),
                Height = _height - 10,
                Width = _height - 10,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = isSystemMsg ? System.Windows.HorizontalAlignment.Left : System.Windows.HorizontalAlignment.Right,
                Margin = isSystemMsg ? new System.Windows.Thickness(5, 0, 0, 0) : new System.Windows.Thickness(0, 0, 10, 0),
            };

            grd.Children.Add(img);

            var text = new TextBlock()
            {
                TextWrapping = System.Windows.TextWrapping.Wrap,
                Text = content,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
            };

            var bubble = new PUBubble()
            {
                Margin = isSystemMsg ? new System.Windows.Thickness(_height + 5, 0, 0, 0) : new System.Windows.Thickness(0, 0, _height + 10, 0),
                AnglePosition = isSystemMsg ? AnglePositions.Left : AnglePositions.Right,
                Content = text,
                MinHeight = _height - 6,
                BorderCornerRadius = new System.Windows.CornerRadius(3),
                HorizontalAlignment = isSystemMsg ? System.Windows.HorizontalAlignment.Left : System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Background = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#FF49A9C0"))),
                CoverBrush = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#CC49A9C0"))),
                Padding = new System.Windows.Thickness(10, 0, 10, 0),
            };
            grd.Children.Add(bubble);

            var anima = new DoubleAnimation()
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(0.4),
            };

            var anima2 = new ThicknessAnimation()
            {
                To = isTopOne ? new System.Windows.Thickness(0, 10, 0, 0) : new System.Windows.Thickness(0, 0, 0, 0),
                Duration = TimeSpan.FromSeconds(0.2),
            };
            grd.BeginAnimation(OpacityProperty, anima);
            grd.BeginAnimation(MarginProperty, anima2);
            return grd;
        }

        private static string ipv4 = Config.ipStr;
        private static string nickName = Config.username;//NiCk?
        private static string ftpUserName = "ftp0";
        private static string ftpUserPwd = "19981209zt+";

        //建立传输实例
        private FtpClass ftpClass = new FtpClass(ipv4, nickName, ftpUserName, ftpUserPwd);
        private string localFilePathName = "";
        //此路径位置，最好显示在界面

        private void menuOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            string ftpFilePath1 = lab_ftpPath.Content.ToString();
            FileManager fileManager1 = listView.SelectedItem as FileManager;
            string ftpPathToOpen = fileManager1.fileName;

            lab_ftpPath.Content = lab_ftpPath.Content.ToString() + "/" + ftpPathToOpen;

            GetFilesByPath(ftpFilePath1 + "/" + ftpPathToOpen);

            listView.ItemsSource = null;
            listView.ItemsSource = files;
        }

        private void menuNewFolder_Click(object sender, RoutedEventArgs e)
        {
            string ftpFilePath1 = lab_ftpPath.Content.ToString();
            if (renameWin == null || renameWin.IsVisible == false)
            {
                this.IsCoverMaskShow = true;
                renameWin = new RenameWin();
                renameWin.Owner = this;
                renameWin.ShowDialog();
            }
            else
            {
                renameWin.Activate();
                renameWin.WindowState = WindowState.Normal;
            }

            string newFolderName = Config.NAmenamename;
            ftpClass.newFold(ftpFilePath1, newFolderName);
            GetFilesByPath(ftpFilePath1);

            listView.ItemsSource = null;
            listView.ItemsSource = files;
        }

        private void MenuDownload_Click(object sender, RoutedEventArgs e)
        {
            string ftpFilePath1 = lab_ftpPath.Content.ToString();
            FileManager fileManager1 = listView.SelectedItem as FileManager;
            string fileName = fileManager1.fileName;
            //下载地址

            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择Txt所在文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    System.Windows.MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                string localPath = dialog.SelectedPath;
                ftpClass.downFile(ftpFilePath1, fileName, localPath);

            }
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            string ftpFilePath1 = lab_ftpPath.Content.ToString();
            FileManager fileManager1 = listView.SelectedItem as FileManager;
            string fileName = fileManager1.fileName;
            if (fileManager1.fileType.ToString() == "文件夹")
            {
                ftpClass.delFold(ftpFilePath1, fileName);
            }
            else
            {
                ftpClass.delFile(ftpFilePath1, fileName);
            }
        }

        private void menuRename_Click(object sender, RoutedEventArgs e)
        {
            if (renameWin == null || renameWin.IsVisible == false)
            {
                this.IsCoverMaskShow = true;
                renameWin = new RenameWin();
                renameWin.Owner = this;
                renameWin.ShowDialog();
            }
            else
            {
                renameWin.Activate();
                renameWin.WindowState = WindowState.Normal;
            }

            string ftpFilePath1 = lab_ftpPath.Content.ToString();
            FileManager fileManager1 = listView.SelectedItem as FileManager;
            string fileName = fileManager1.fileName;

            string newNameToChage = Config.NAmenamename;

            ftpClass.renameFile(ftpFilePath1, fileName, newNameToChage);
        }

        private void LabUploading_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string ftpFilePath1 = lab_ftpPath.Content.ToString();
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "";  //筛选文件没写

            if (ofd.ShowDialog() == true)
            {
                ftpClass.upFile(ofd.FileName, ftpFilePath1);
            }

        }

        private void Lab_backpath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string ftpNowPath = lab_ftpPath.Content.ToString();
            if (ftpNowPath == "" || ftpNowPath == null)
                return;
            string ftpBackPath = ftpNowPath.Remove(ftpNowPath.LastIndexOf("/"),
                ftpNowPath.Length - ftpNowPath.LastIndexOf("/"));
            lab_ftpPath.Content = ftpBackPath;
            GetFilesByPath(ftpBackPath);

            listView.ItemsSource = null;
            listView.ItemsSource = files;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string ftpFilePath1 = lab_ftpPath.Content.ToString();
            FileManager fileManager1 = listView.SelectedItem as FileManager;
            if (fileManager1.fileType.ToString() == "文件夹")
            {
                string ftpPathToOpen = fileManager1.fileName;

                lab_ftpPath.Content = lab_ftpPath.Content.ToString() + "/" + ftpPathToOpen;

                GetFilesByPath(ftpFilePath1 + "/" + ftpPathToOpen);

                listView.ItemsSource = null;
                listView.ItemsSource = files;
            }
        }

        private void MenuItem_personalData_Click(object sender, RoutedEventArgs e)
        {
            PersonalDatawin personalDatawin = new PersonalDatawin();
            personalDatawin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            personalDatawin.Owner = this;
            this.IsCoverMaskShow = true;
            personalDatawin.ShowDialog();
        }

        private void Menuitem_checkdata_Click(object sender, RoutedEventArgs e)
        {
            FriendInfoWin friendInfoWin = new FriendInfoWin();
            friendInfoWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            friendInfoWin.Owner = this;
            this.IsCoverMaskShow = true;
            friendInfoWin.ShowDialog();
        }

        private void MenuItem_changepassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordWin changePasswordWin = new ChangePasswordWin();
            changePasswordWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            changePasswordWin.Owner = this;
            this.IsCoverMaskShow = true;
            changePasswordWin.ShowDialog();
        }

        private void Menuitem_deletefriend_Click(object sender, RoutedEventArgs e)
        {
            AddFriendService addFriendService = AddFriendService.GetAddFriendService();
            string str_json = null;
            try
            {
                str_json = "{\"type\":\"delete\",\"uid\":\"" + Config.uid + "\",\"frienduid\":\"" + (listViewFriends.SelectedItem as FriendInfo).Uid + "\",\"choice\":\"1\"}";
                addFriendService.AddSocket.Send(Encoding.UTF8.GetBytes(str_json));
                ManageFriendsList.friendslList.Remove(listViewFriends.SelectedItem as FriendInfo);
            }
            catch (Exception exception)
            {
                PUMessageBox.ShowDialog("删除失败！");
            }
        }

        private void menuShare_Click(object sender, RoutedEventArgs e)
        {
            UpLoadFriendWin fileLoad = new UpLoadFriendWin();
            fileLoad.Owner = this;
            this.IsCoverMaskShow = true;
            fileLoad.Show();

            string tagNickName = Config.SelectedName;
            string ftpFilePath1 = lab_ftpPath.Content.ToString();
            FileManager fileManager1 = listView.SelectedItem as FileManager;
            string fileName = fileManager1.fileName;

            FtpClass ftpClass = new FtpClass(ipv4, nickName, ftpUserName, ftpUserPwd);
            ftpClass.CopyFile(ftpFilePath1, tagNickName, fileName, 1);

        }
    }
}