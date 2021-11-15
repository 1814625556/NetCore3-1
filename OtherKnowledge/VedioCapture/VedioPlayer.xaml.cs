using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VedioCapture
{
    /// <summary>
    /// Interaction logic for VedioPlayer.xaml
    /// </summary>
    public partial class VedioPlayer : Window
    {
        private double totalTime { get; set; }
        public VedioPlayer()
        {
            InitializeComponent();
        }

        private void LoadMedia(object sender, RoutedEventArgs e)
        {
            this.Gmedia.Source = new Uri(@"D:\WorkSpace\guidance\src\Guidance\bin\x64\Debug\net5.0-windows\3-112824.avi");
            this.Gmedia.LoadedBehavior = MediaState.Manual;
            this.Gmedia.Play();
            this.Gmedia.Pause();

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_tick);
            timer.Start();
        }
        private void timer_tick(object sender, EventArgs e)
        {
            sliderPosition.Value = Gmedia.Position.TotalSeconds;
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            this.Gmedia.Play();
            if (Gmedia.NaturalDuration.HasTimeSpan)
            {
                sliderPosition.Maximum = Gmedia.NaturalDuration.TimeSpan.TotalSeconds;
                sliderPosition.Minimum = 0;
            }
        }

        private void PauseClick(object sender, RoutedEventArgs e)
        {
            this.Gmedia.Pause();
        }

        private void SpeedClick(object sender, RoutedEventArgs e)
        {
            Gmedia.SpeedRatio = 2;
        }

        private void sliderPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //mediaElement.Stop();
            //mediaElement.Position = TimeSpan.FromSeconds(sliderPosition.Value);
            //mediaElement.Play();
        }
    }
}
