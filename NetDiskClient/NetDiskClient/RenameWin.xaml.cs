using NetDiskClient.utils;
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
using Panuon.UI;

namespace NetDiskClient
{
    /// <summary>
    /// RenameWin.xaml 的交互逻辑
    /// </summary>
    public partial class RenameWin : Window
    {
        public RenameWin()
        {
            InitializeComponent();
        }

        private void PUButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PUButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PUButtonCopnfirm_Click(object sender, RoutedEventArgs e)
        {
            Config.NAmenamename = txtname11.Text;
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            (this.Owner as PUWindow).IsCoverMaskShow = false;
        }
    }
}
