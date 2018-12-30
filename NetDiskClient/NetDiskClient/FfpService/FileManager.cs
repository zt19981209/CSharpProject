using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NetDiskClient
{
    internal class FileManager : INotifyPropertyChanged
    {
        private BitmapImage icon;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _filename;
        public string fileName
        {
            get { return _filename; }
            set
            {
                _filename = value;
                if (PropertyChanged != null)
                {

                    PropertyChanged(this, new PropertyChangedEventArgs("fileName"));
                }
            }
        }

        private string _filesize;
        public string fileSize
        {
            get { return _filesize; }
            set
            {
                _filesize = value;
                //PropertyChanged(this, new PropertyChangedEventArgs("fileSize"));
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("fileSize"));
                }
            }
        }

        private string _fileType;
        public string fileType
        {
            get { return _fileType; }
            set
            {
                _fileType = value;
                //PropertyChanged(this, new PropertyChangedEventArgs("fileType"));
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("fileType"));
                }
            }
        }

        private string _fileDate;
        public string fileDate
        {
            get { return _fileDate; }
            set
            {
                _fileDate = value;
                //PropertyChanged(this, new PropertyChangedEventArgs("fileDate"));
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("fileDate"));
                }
            }
        }

        public BitmapImage Icon { get => icon; set => icon = value; }
        //public string Iconstring { get => iconstring; set => iconstring = value; }

    }
}