using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VedioCapture
{
    /// <summary>
    /// Interaction logic for VedioPlayControl.xaml
    /// </summary>
    public partial class VedioPlayControl : UserControl
    {
        private bool isPlay = false;

        private DispatcherTimer timer = null;

        private string videoPath = "";

        public VedioPlayControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// need ObsolutePath
        /// </summary>
        public string VideoPath
        {
            get => videoPath;
            set
            {
                videoPath = value;
                timer.Tick -= timer_tick;

                Gmedia.Source = new Uri(value);
                Gmedia.LoadedBehavior = MediaState.Manual;
                Gmedia.Play();
                Gmedia.Pause();
                sliderGmedia.Value = 0;

                timer.Tick += timer_tick;
                timer.Start();
            }
        }

        private void timer_tick(object sender, EventArgs e)
        {
            sliderGmedia.Value = Gmedia.Position.TotalSeconds;
            var tStr = $"{Convert.ToInt32(Gmedia.Position.TotalSeconds)}:{Convert.ToInt32(Gmedia.NaturalDuration.TimeSpan.TotalSeconds)}";
            VedioTime.Text = tStr;
        }

        private void PlayOrPause(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(videoPath)) return;
            if (isPlay)
            {
                Gmedia.Pause();
                PlayImg.Visibility = Visibility.Visible;
                PauseImg.Visibility = Visibility.Hidden;
            }
            else
            {
                if (Gmedia.NaturalDuration.HasTimeSpan)
                    sliderGmedia.Maximum = Gmedia.NaturalDuration.TimeSpan.TotalSeconds;
                Gmedia.Play();
                PlayImg.Visibility = Visibility.Hidden;
                PauseImg.Visibility = Visibility.Visible;
            }
            isPlay = !isPlay;
        }

        private void VideoLoad(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
        }

        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetNeedlePosition(object sender, MouseButtonEventArgs e)
        {
            var point = Mouse.GetPosition(ActualNeedle);

            var mat = new Mat(
                (int)ActualNeedle.Height,
                (int)ActualNeedle.Width,
                MatType.CV_8UC4,
                new Scalar(0, 0, 0, 0));

            mat.Circle(
                    (int)point.X,
                    (int)point.Y,
                    5,
                    ColorToScalar(Color.FromArgb(255, 255, 0, 0)),
                    - 1); ;
            this.ActualNeedle.Source = mat.ToWriteableBitmap();
        }

        Scalar ColorToScalar(Color color)
        {
            return new Scalar(color.B, color.G, color.R, color.A);
        }
        private void mouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var val = sliderGmedia.Value;
                var position = (val / 600) * Gmedia.NaturalDuration.TimeSpan.TotalSeconds;
            }

        }

        private void mouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
