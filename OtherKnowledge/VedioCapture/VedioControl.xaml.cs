using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VedioCapture
{
    /// <summary>
    /// Interaction logic for VedioControl.xaml
    /// </summary>
    public partial class VedioControl : UserControl
    {
        private DispatcherTimer timer = null;
        public VedioControl()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
        }

        
        /// <summary>
        /// need ObsolutePath
        /// </summary>
        public string VideoPath
        {
            get => "";
            set
            {
                timer.Tick -= timer_tick;
                Gmedia.Source = new Uri(value);
                Gmedia.LoadedBehavior = MediaState.Manual;
                Gmedia.Play();
                Gmedia.Pause();
                sliderGmedia.Value = 0;
                GmediaPlayer.Visibility = Visibility.Visible;

                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_tick;
                timer.Start();
            }
        }
        private void timer_tick(object sender, EventArgs e)
        {
            sliderGmedia.Value = Gmedia.Position.TotalSeconds;
        }

        /// <summary>
        /// 点击Play按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play(object sender, RoutedEventArgs e)
        {
            if (sliderGmedia.Value == 0)
            {
                sliderGmedia.Maximum = Gmedia.NaturalDuration.TimeSpan.TotalSeconds;
                VedioTime.Text = Gmedia.NaturalDuration.TimeSpan.TotalSeconds.ToString();
            }
            Gmedia.Play();
            this.GmediaPlayer.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pause(object sender, RoutedEventArgs e)
        {
            Gmedia.Pause();
            GmediaPlayer.Visibility = Visibility.Visible;
        }
    }
}
