using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace VedioCapture
{
    public class CaptureScreenHelper
    {
        public static void Capture(double x, double y, double width, double height)
        {
            int ix = Convert.ToInt32(x);
            int iy = Convert.ToInt32(y);
            int iw = Convert.ToInt32(width);
            int ih = Convert.ToInt32(height);

            Bitmap bitmap = new Bitmap(iw, ih);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih));
                bitmap.Save("cc.png", ImageFormat.Png);
            }
        }

        public static Mat GetScreenMat(double x, double y, double width, double height)
        {
            int ix = Convert.ToInt32(x);
            int iy = Convert.ToInt32(y);
            int iw = Convert.ToInt32(width);
            int ih = Convert.ToInt32(height);

            Bitmap bitmap = new Bitmap(iw, ih);
            using (Graphics graphics = Graphics.FromImage(bitmap))
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
    }
}
