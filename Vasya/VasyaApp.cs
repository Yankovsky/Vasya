using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace Vasya
{
    public class VasyaApp
    {
//         System.out.println("еденицы измерения чисел матрицы");
//        System.out.println(" реальное расстояние между этими числами, ибо эти числы это высота измеренная в разных точках");
        private const int N = 13;
        const int size = 1024;
        string fileName = "file.txt";
        int[,] LoG = new int[N, N]
                          {
                              {0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0},
                              {0, 0, 0, 1, 1, 2, 2, 2, 1, 1, 0, 0, 0},
                              {0, 0, 2, 2, 3, 3, 4, 3, 3, 2, 2, 0, 0},
                              {0, 1, 2, 3, 3, 3, 2, 3, 3, 3, 2, 1, 0},
                              {0, 1, 3, 3, 1, -4, -6, -4, 1, 3, 3, 1, 0},
                              {1, 2, 3, 3, -4, -14, -19, -14, -4, 3, 3, 2, 1},
                              {1, 2, 4, 2, -6, -19, -24, -19, -6, 2, 4, 2, 1},
                              {1, 2, 3, 3, -4, -14, -19, -14, -4, 3, 3, 2, 1},
                              {0, 1, 3, 3, 1, -4, -6, -4, 1, 3, 3, 1, 0},
                              {0, 1, 2, 3, 3, 3, 2, 3, 3, 3, 2, 1, 0},
                              {0, 0, 2, 2, 3, 3, 4, 3, 3, 2, 2, 0, 0},
                              {0, 0, 0, 1, 1, 2, 2, 2, 1, 1, 0, 0, 0},
                              {0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0}
                          };

        public BitmapImage DoWork()
        {
            var topo = LoadTopoFromFile(fileName);

            var newTopo = new List<IList<double>>(size);
            for (int n = 0; n < topo.Count; n++)
            {
                for (int m = 0; m < topo[n].Count; m++)
                {
                    double piska = 0;
                    for (int i = 0; i < N; i++)
                    {
                        for (int j = 0; j < N; j++)
                        {
                            if (n >= i && m >= j)
                            {
                                piska += LoG[i, j] * topo[n - i][m - j];                                
                            }
                        }
                    }
                    newTopo[n][m] = piska;
                }
            }

            var min = newTopo.Min();
            var max = newTopo.Max();
            return null;
            /*var a = CreateBitmap(newTopo);

            var memoryStream = new MemoryStream();
            a.Save(memoryStream, ImageFormat.Png);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
            bitmapImage.EndInit();

            return bitmapImage;*/
        }

        private static List<List<double>> LoadTopoFromFile(string fileName)
        {
            var topo = new List<List<double>>(size);
            using (var sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    topo.Add(Regex.Split(line, @"\s").Select(number => double.Parse(number, CultureInfo.InvariantCulture)).ToList());
                }
            }
            return topo;
        }

/*
       public Bitmap CreateBitmap(List<IList<double>> newTopo)
       {
           var b = new Bitmap(size, size, PixelFormat.Format8bppIndexed);

           var ncp = b.Palette;
           for (int i = 0; i < 256; i++)
               ncp.Entries[i] = Color.FromArgb(255, i, i, i);
           b.Palette = ncp;

           var BoundsRect = new Rectangle(0, 0, Width, Height);
           BitmapData bmpData = b.LockBits(BoundsRect,
                                           ImageLockMode.WriteOnly,
                                           b.PixelFormat);

           IntPtr ptr = bmpData.Scan0;

           int bytes = bmpData.Stride * b.Height;
           var rgbValues = new byte[bytes];

           // fill in rgbValues, e.g. with a for loop over an input array

           Marshal.Copy(rgbValues, 0, ptr, bytes);
           b.UnlockBits(bmpData);
           return b;
       }
*/
    }
}