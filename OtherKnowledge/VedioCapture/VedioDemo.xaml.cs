using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VedioCapture
{
    /// <summary>
    /// Interaction logic for VedioDemo.xaml
    /// </summary>
    public partial class VedioDemo : Window
    {
        public VedioDemo()
        {
            InitializeComponent();
        }

        private void LoadVedio(object sender, RoutedEventArgs e)
        {
            this.vedioPlayer.VideoPath = @"D:\WorkSpace\guidance\src\Guidance\bin\x64\Debug\net5.0-windows\3-112824.avi";
        }
    }
}
