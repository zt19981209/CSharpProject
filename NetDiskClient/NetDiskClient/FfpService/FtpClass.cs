using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
/*
* 1、无法获得本地路径
*      -->上传文件的路径
*      -->下载的本地路径
* 2、没有图标
* 3、是否实现上传/下载文件夹功能
*/
namespace NetDiskClient
{
    class FtpClass
    {
        private string FtpUserName = "ftp0";
        private string FtpUserPwd = "19981209zt+";
        private string FtpUserPath;
        public static string SSSSize = "100/100MB";

        public FtpClass() { }

        public FtpClass(string ipv4, string nickName, string ftpUserName, string ftpUserPwd)
        {
            this.FtpUserPath = "ftp://" + ipv4 + "/" + nickName;
            this.FtpUserName = ftpUserName;
            this.FtpUserPwd = ftpUserPwd;
        }

        //xinjainyonghu
        //用户名新建
        public void newFold(string nickname)
        {
            string uri = "ftp://" + "106.13.44.221" + "/" + nickname;
            FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
            ftpWebRequest.KeepAlive = false;

            ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
            ftpWebResponse.Close();
        }


        //打开文件夹
        public string[][] openFord(string ftpPath, string fordToOpen)
        {
            ftpPath = ftpPath + "/" + fordToOpen;
            return getFileList(ftpPath);
        }

        //获取文件列表
        public string[][] getFileList(string ftpPath)
        {
            string[][] result = new string[50][];
            try
            {
                string uri;
                if (ftpPath.Length == 0)
                {
                    uri = FtpUserPath;
                }
                else
                {
                    uri = FtpUserPath + "/" + ftpPath;
                }

                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                WebResponse webResponse = ftpWebRequest.GetResponse();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));//GB2312
                string line = reader.ReadLine();

                int i = 0;
                while (line != null)
                {
                    if (i < 50)
                    {
                        string[] lineSplited = getLineSplited(ftpPath, line);
                        result[i] = lineSplited;
                        i++;
                        line = reader.ReadLine();
                    }
                    else
                    {
                        string[][] tmp = new string[result.Length + 20][];
                        tmp = result;
                        result = tmp;   //这样行吗？
                    }

                }

                reader.Close();
                webResponse.Close();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private string[] getLineSplited(string ftpPath, string line)
        {
            string[] lineSplited = new string[4];
            string fileName, fileTime, fileType;

            line = line.TrimEnd(new char[] { ' ' });

            int lastSpace = line.LastIndexOf(' ');

            fileName = line.Substring(lastSpace + 1, line.Length - lastSpace - 1);

            line = line.Remove(lastSpace + 1, line.Length - lastSpace - 1);

            line = line.TrimEnd(new char[] { ' ' });

            lastSpace = line.LastIndexOf(' ');

            fileType = line.Substring(lastSpace + 1, line.Length - lastSpace - 1);

            line = line.Remove(lastSpace + 1, line.Length - lastSpace - 1);

            line = line.TrimEnd(new char[] { ' ' });

            fileTime = line;
            long lenss;
            if (fileType.ToUpper() != "<DIR>")
            {
                lenss = getFileSize(ftpPath, fileName);
                lineSplited[1] = lenss.ToString();

                int lastPoint = fileName.LastIndexOf('.');
                if (lastPoint != -1)
                {
                    fileType = fileName.Substring(lastPoint + 1, fileName.Length - lastPoint - 1) + "文件";
                }
                else
                {
                    fileType = "未知文件";
                }
            }
            else
            {
                lineSplited[1] = null;
                fileType = "文件夹";
            }
            lineSplited[0] = fileName;
            lineSplited[2] = fileType;
            lineSplited[3] = fileTime;

            return lineSplited;
        }


        //获取文件大小
        private long getFileSize(string ftpPath, string fileName)
        {
            long filesize;
            try
            {
                string uri;

                if (ftpPath.Length == 0)
                {
                    uri = FtpUserPath + "/" + fileName;
                }
                else
                {
                    uri = FtpUserPath + "/" + ftpPath + "/" + fileName;
                }
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                ftpWebRequest.KeepAlive = false;

                ftpWebRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                filesize = ftpWebResponse.ContentLength;
                ftpWebResponse.Close();

                return filesize;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return 0;
            }
        }

        //新建文件夹
        public void newFold(string ftpPath, string foldName)
        {
            try
            {
                string uri;

                if (ftpPath.Length == 0)
                {
                    uri = FtpUserPath + "/" + foldName;
                }
                else
                {
                    uri = FtpUserPath + "/" + ftpPath + "/" + foldName;
                }
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                ftpWebRequest.KeepAlive = false;

                ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpWebResponse.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //重命名文件
        public void renameFile(string ftpPath, string oldName, string newName)
        {
            try
            {
                string uri;

                if (ftpPath.Length == 0)
                {
                    uri = FtpUserPath + "/" + oldName;
                }
                else
                {
                    uri = FtpUserPath + "/" + ftpPath + "/" + oldName;
                }
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                ftpWebRequest.KeepAlive = false;

                ftpWebRequest.RenameTo = newName;

                ftpWebRequest.Method = WebRequestMethods.Ftp.Rename;
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpWebResponse.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //重命名文件夹
        public void renameFold(string ftpPath, string oldName, string newName)
        {
            try
            {
                //string uri;

                //if (ftpPath.Length == 0)
                //{
                //    uri = FtpUserPath + "/" + oldName;
                //}
                //else
                //{
                //    uri = FtpUserPath + "/" + ftpPath + "/" + oldName;
                //}
                //FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                //ftpWebRequest.UseBinary = true;
                //ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                //ftpWebRequest.KeepAlive = false;

                newFold(ftpPath, newName);

                delFold(ftpPath, oldName);


                //ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                //FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                //ftpWebResponse.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        //删除文件夹
        //有问题
        public void delFold(string ftpPath, string foldName)
        {
            try
            {
                string uri;

                if (ftpPath.Length == 0)
                {
                    uri = FtpUserPath;
                }
                else
                {
                    uri = FtpUserPath + "/" + ftpPath;
                }
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;

                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpWebResponse.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        ////重命名文件
        //public void renameFile(string ftpPath, string fileName, string newFileName)
        //{
        //    try
        //    {
        //        string uri;

        //        if (ftpPath.Length == 0)
        //        {
        //            uri = FtpUserPath;
        //        }
        //        else
        //        {
        //            uri = FtpUserPath + "/" + ftpPath;
        //        }
        //        FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
        //        ftpWebRequest.UseBinary = true;
        //        ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
        //        ftpWebRequest.KeepAlive = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }
        //}

        //移动文件
        public void cutFile(string oldPath, string newPath, string fileName)
        {
            CopyFile(oldPath, newPath, fileName, 0);
            delFile(oldPath, fileName);
        }

        //复制文件
        //分享文件
        public void CopyFile(string oldPath, string newPath, string fileName, int whichMode)
        {
            try
            {
                string oldUri;
                string newUri;
                if (oldPath.Length == 0)
                {
                    oldUri = FtpUserPath + "/" + fileName;
                }
                else
                {
                    oldUri = FtpUserPath + "/" + oldPath + "/" + fileName;
                }

                if(whichMode == 1)
                {
                    newUri = "ftp://" + "106.13.44.221" + "/" + newPath;
                }
                else
                {
                    if (newPath.Length == 0)
                    {
                        newUri = FtpUserPath + "/" + fileName;
                    }
                    else
                    {
                        newUri = FtpUserPath + "/" + newPath + "/" + fileName;
                    }
                }
                string localPath = "C:\\cocosProject\\HelloWord";

                downFile(oldPath, fileName, localPath);

                Thread.Sleep(2000);

                FileInfo fileInfo = new FileInfo(localPath + "\\"+fileName);

                if(fileInfo.Equals(null)){
                    Console.Write("iiii");
                    return;
                }
                string newFileName;

                if (fileInfo.Name.IndexOf("#") == -1)
                {
                    newFileName = RemoveSpaces(fileInfo.Name);
                }
                else
                {
                    newFileName = fileInfo.Name.Replace("#", "＃");
                    newFileName = RemoveSpaces(newFileName);
                }
                string newUr1 = "ftp://106.13.44.221/zhang";

                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(newUr1));
                ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Method = WebRequestMethods.Ftp.AppendFile;

                ftpWebRequest.ContentLength = fileInfo.Length;
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                FileStream fileStream = fileInfo.OpenRead();
                Stream stream = null;

                ushort[] keys = { 'a', 'b', 'c', 'd' };
                ClassLibrary1.SDesEncrypt fileEncrypt = new ClassLibrary1.SDesEncrypt(keys);

                try
                {
                    stream = ftpWebRequest.GetRequestStream();
                    int contentLen = fileStream.Read(buff, 0, buffLength);

                    while (contentLen != 0)
                    {
                        //stream.Write(Encoding.Default.GetBytes(fileEncrypt.Decode(Encoding.ASCII.GetChars(buff))), 0, contentLen);

                        stream.Write(buff, 0, buffLength);

                        contentLen = fileStream.Read(buff, 0, buffLength);
                        //toolStripProgressBar.Value = startbye;
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Close();
                    if (stream != null)
                        stream.Close();
                }
                return;

                //FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(oldUri);
                //ftpWebRequest.UseBinary = true;
                //ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                //ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                //FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                //Stream ftpStream = ftpWebResponse.GetResponseStream();


                //FtpWebRequest ftpWebRequest2 = (FtpWebRequest)FtpWebRequest.Create(new Uri(newUri));
                //ftpWebRequest2.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                //ftpWebRequest2.KeepAlive = false;
                //ftpWebRequest2.UseBinary = true;
                //ftpWebRequest2.Method = WebRequestMethods.Ftp.AppendFile;
                //Stream stream = ftpWebRequest.GetRequestStream();

                //long cl = ftpWebResponse.ContentLength;
                //int bufferSize = 2048;
                //int readCount;

                //byte[] buffer = new byte[bufferSize];
                //readCount = ftpStream.Read(buffer, 0, bufferSize);

                //while (readCount > 0)
                //{
                //    stream.Write(buffer, 0, bufferSize);
                //    readCount = ftpStream.Read(buffer, 0, bufferSize);
                //}

                //ftpStream.Close();
                //ftpWebResponse.Close();
                //stream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //下载文件
        public void downFile(string ftpPath, string fileName, string localPath)
        {
            try
            {
                string uri;
                string localPathName = localPath + "\\" + fileName;

                if (ftpPath.Length == 0)
                {
                    uri = FtpUserPath + "/" + fileName;
                }
                else
                {
                    uri = FtpUserPath + "/" + ftpPath + "/" + fileName;
                }

                FileStream outStream = new FileStream(localPathName, FileMode.Create);
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                ftpWebRequest.UseBinary = true;

                ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                Stream ftpStream = ftpWebResponse.GetResponseStream();

                long cl = ftpWebResponse.ContentLength;
                int bufferSize = 2048;
                int readCount;

                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);

                while (readCount > 0)
                {
                    outStream.Write(buffer, 0, bufferSize);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outStream.Close();
                ftpWebResponse.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //删除文件
        public void delFile(string ftpPath, string fileName)
        {
            try
            {
                string uri;
                if (ftpPath.Length == 0)
                {
                    uri = FtpUserPath + "/" + fileName;
                }
                else
                {
                    uri = FtpUserPath + "/" + ftpPath + "/" + fileName;
                }

                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
                ftpWebRequest.KeepAlive = false;

                ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpWebResponse.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //上传文件
        public bool upFile(string localPathName, string ftpPath)
        {
            bool ifSuc = true;
            string fileName = localPathName.Substring(localPathName.LastIndexOf('\\') + 1, localPathName.Length - localPathName.LastIndexOf('\\') - 1);

            FileInfo fileInfo = new FileInfo(localPathName);

            int allbye = (int)fileInfo.Length;
            int startbye = 0;
            //toolStripProgressBar.Maximum = allbye;
            //toolStripProgressBar.Minimum = 0;
            string newFileName;

            if (fileInfo.Name.IndexOf("#") == -1)
            {
                newFileName = RemoveSpaces(fileInfo.Name);
            }
            else
            {
                newFileName = fileInfo.Name.Replace("#", "＃");
                newFileName = RemoveSpaces(newFileName);
            }

            string uri;
            if (ftpPath.Length == 0)
            {
                uri = FtpUserPath + "/" + fileName;
            }
            else
            {
                uri = FtpUserPath + "/" + ftpPath + "/" + fileName;
            }


            FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
            ftpWebRequest.KeepAlive = false;
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.Method = WebRequestMethods.Ftp.AppendFile;

            ftpWebRequest.ContentLength = fileInfo.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            FileStream fileStream = fileInfo.OpenRead();
            Stream stream = null;

            ushort[] keys = { 'a', 'b', 'c', 'd' };
            ClassLibrary1.SDesEncrypt fileEncrypt = new ClassLibrary1.SDesEncrypt(keys);

            try
            {
                stream = ftpWebRequest.GetRequestStream();
                int contentLen = fileStream.Read(buff, 0, buffLength);

                while (contentLen != 0)
                {
                    //stream.Write(Encoding.Default.GetBytes(fileEncrypt.Decode(Encoding.ASCII.GetChars(buff))), 0, contentLen);

                    stream.Write(buff, 0, buffLength);

                    contentLen = fileStream.Read(buff, 0, buffLength);
                    startbye += contentLen;
                    //toolStripProgressBar.Value = startbye;
                }
            }
            catch (Exception ex)
            {
                ifSuc = false;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                if (stream != null)
                    stream.Close();
            }
            return ifSuc;
        }

        //去除空格？
        private static string RemoveSpaces(string str)
        {
            string a = "";
            CharEnumerator CEnumerator = str.GetEnumerator();
            while (CEnumerator.MoveNext())
            {
                byte[] array = new byte[1];
                array = System.Text.Encoding.ASCII.GetBytes(CEnumerator.Current.ToString());
                int asciicode = (short)(array[0]);
                if (asciicode != 32)
                {
                    a += CEnumerator.Current.ToString();
                }
            }
            string sdate = System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString()
                + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + System.DateTime.Now.Millisecond.ToString();
            return a.Split('.')[a.Split('.').Length - 2] + "." + a.Split('.')[a.Split('.').Length - 1];
        }

        internal class copyFile
        {
            private string ftpFilePath1;
            private string tagNickName;
            private string fileName;
            private int v;

            public copyFile(string ftpFilePath1, string tagNickName, string fileName, int v)
            {
                this.ftpFilePath1 = ftpFilePath1;
                this.tagNickName = tagNickName;
                this.fileName = fileName;
                this.v = v;
            }
        }
    }
}
