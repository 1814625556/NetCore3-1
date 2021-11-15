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
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        private DispatcherTimer timer = null;

        private bool isPlay = false;

        private string videoPath = "";
        public PlayerControl()
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
                if (timer != null)
                {
                    timer.Tick -= timer_tick;
                }

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                Gmedia.Source = new Uri(value);
                Gmedia.LoadedBehavior = MediaState.Manual;
                Gmedia.Play();
                Gmedia.Pause();
                timer.Tick += timer_tick;
                timer.Start();
            }
        }

        private void timer_tick(object sender, EventArgs e)
        {   
            if (string.IsNullOrEmpty(videoPath)) return;
            DynamicTimeRecordText.Text = $"{TimeConvert(Convert.ToInt32(Gmedia.Position.TotalSeconds))}";
        }

        private string TimeConvert(int seconds)
        {
            var tformatStr = "";
            var mins = seconds / 60;
            tformatStr = mins <= 0 ? "00:" : mins < 10 ? $"0{mins}:" : $"{mins}:";

            seconds -= 60 * mins;
            if (seconds <= 0)
            {
                tformatStr += "00";
            }
            if (seconds > 0 && seconds < 10)
            {
                tformatStr += $"0{seconds}";
            }
            if(seconds >= 10)
            {
                tformatStr += $"{seconds}";
            }

            return tformatStr;
        }

        private void PlayOrPause(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(videoPath)) return;
            if (Gmedia.NaturalDuration.HasTimeSpan)
            {
                TotalTime.Text = $"{TimeConvert(Convert.ToInt32(Gmedia.NaturalDuration.TimeSpan.TotalSeconds))}";
            }
            if (isPlay)
            {
                Gmedia.Pause();
            }
            else
            {
                Gmedia.Play();
            }
            isPlay = !isPlay;
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
                    -1); ;
            this.ActualNeedle.Source = mat.ToWriteableBitmap();
        }

        Scalar ColorToScalar(Color color)
        {
            return new Scalar(color.B, color.G, color.R, color.A);
        }

        private void backClick(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(videoPath)) return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Gmedia.Position = Gmedia.Position - TimeSpan.FromSeconds(10); ;
            }

        }

        private void forwardClick(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(videoPath)) return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Gmedia.Position = Gmedia.Position + TimeSpan.FromSeconds(10); ;
            }

        }
    }
}
