using OpenCvSharp;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VedioCapture
{
    /// <summary>
    /// 参考连接：https://blog.csdn.net/diebiao6526/article/details/101435572
    /// </summary>
    public class VedioHelper
    {
        static bool IsStop { get; set; }
        static bool IsCapturing { get; set; }
        static string _vName;

        public VedioHelper()
        {
            _vName = "out.avi";
            IsCapturing = false;
            IsStop = true;
        }
        public static void Start()
        {
            IsStop = false;
        }
        public static void Stop()
        {
            if (IsCapturing)
            {
                IsStop = true;
                IsCapturing = false;
            }
        }

        public static void CaptureVideoX(string fileName = "out.avi", int delayMillisecond = 49,
            int screenWidth=1000, int screenHeight=1000, int fps=20)
        {
            fileName = StandardizationFileName(fileName);
            CaptureVideo(fileName, delayMillisecond, screenWidth, screenHeight, fps);
        }

        public async static Task CaptureVideoAsync(string fileName, int delayMillisecond = 49,
            int screenWidth = 1000, int screenHeight = 1000, int fps = 20)
        {
            fileName = StandardizationFileName(fileName);
            await Task.Run(() =>
            {
                CaptureVideo(fileName, delayMillisecond, screenWidth, screenHeight, fps);
            });
        }

        public static void CaptureVideoXOnNewThread(string fileName, int delayMillisecond = 49,
            int screenWidth = 1000, int screenHeight = 1000, int fps = 20)
        {
            fileName = StandardizationFileName(fileName);
            Task.Run(() =>
            {
                CaptureVideo(fileName,delayMillisecond,screenWidth,screenHeight,fps);
            });
        }

        static void CaptureVideo(string fileName, int delayMillisecond = 49,
            int screenWidth = 1000, int screenHeight = 1000, int fps = 20)
        {
            using VideoWriter videoWriter = new VideoWriter(fileName, FourCC.XVID, fps, new Size(screenWidth, screenHeight), true);
            while (!IsStop)
            {
                var mat = GetScreenMatX(0, 0, screenWidth, screenHeight);
                if (!mat.Empty())
                {
                    videoWriter.Write(mat);
                }
                Thread.Sleep(delayMillisecond);
            }
        }

        static Mat GetScreenMatX(int ix, int iy, int iw, int ih)
        {
            var bitmap = new System.Drawing.Bitmap(iw, ih);
            using var graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih));
            Mat source = null;
            using (var s2_ms = new MemoryStream())
            {
                bitmap.Save(s2_ms, ImageFormat.Bmp);
                byte[] bytes = s2_ms.GetBuffer();
                source = Mat.FromImageData(bytes, ImreadModes.AnyColor);
            }

            return source;
        }

        static string StandardizationFileName(string fileName)
        {
            if (fileName.Substring(fileName.Length - 4) != ".avi")
            {
                throw new Exception("suffix must be .avi");
            }

            IsCapturing = true;
            if (fileName == _vName)
            {
                var dt = DateTimeOffset.Now;
                fileName = $"{dt.Year}{dt.Month}{dt.Day}-{dt.Hour}{dt.Minute}{dt.Second}-{dt.Millisecond}.avi";
            }

            return fileName;
        }

        #region 录屏测试
        static Mat GetScreenMat(int x, int y, int width, int height)
        {
            int ix = Convert.ToInt32(x);
            int iy = Convert.ToInt32(y);
            int iw = Convert.ToInt32(width);
            int ih = Convert.ToInt32(height);

            var bitmap = new System.Drawing.Bitmap(iw, ih);
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih));
                Mat source = null;
                using (var s2_ms = new MemoryStream())
                {
                    bitmap.Save(s2_ms, ImageFormat.Bmp);
                    byte[] bytes = s2_ms.GetBuffer();
                    source = Mat.FromImageData(bytes, ImreadModes.AnyColor);
                }

                return source;
            }
        }
        public static void StartVideo()
        {
            var videoWriter = new VideoWriter(@"out.avi", FourCC.XVID, 20, new Size(1000, 1000), true);
            var count = 0;
            while (count < 100)
            {
                var mat = GetScreenMat(0, 0, 1000, 1000);
                if (!mat.Empty())
                {
                    count++;
                    videoWriter.Write(mat);
                    Thread.Sleep(50);
                }
                else
                {
                    break;
                }
            }
            videoWriter.Dispose();
        }
        #endregion
    }
}
