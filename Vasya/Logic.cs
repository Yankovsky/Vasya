using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BitmapProcessing;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Vasya
{
    public class Logic
    {
        public const int Size = 1024;
        public const int N = 13;
        public int ActualImageSize = Size - N;

        private static readonly int[,] LoG = new[,]
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

        private readonly string _fileName;
        private List<List<double>> _topo;
        private double _topoMin;
        private double _topoMax;

        private double[,] _newTopo;

        public Logic(string fileName)
        {
            _fileName = fileName;
        }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }


        public void DoWork()
        {
            _topo = LoadTopoFromFile(_fileName);
            var a = _topo.SelectMany(x => x).ToList();
            _topoMin = a.Min();
            _topoMax = a.Max();

            _newTopo = NewTopo(_topo);
            var b = _newTopo.Cast<double>().ToList();
            MinValue = b.Min();
            MaxValue = b.Max();
        }

        private List<List<double>> LoadTopoFromFile(string fileName)
        {
            var topo = new List<List<double>>(Size);
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

        private Tuple<double, double> FindActualDataLimits(List<double> sortedData, int size, double delta)
        {
            var lowLimit = sortedData[0];
            for (var i = 1; i < size - 1; i++)
            {
                if (Math.Abs((sortedData[i + 1] - sortedData[i]) / (sortedData[i] - lowLimit)) < delta)
                {
                    break;
                }
                lowLimit = sortedData[i];
            }
            var upLimit = sortedData[size - 1];
            for (var i = size - 2; i >= 1; i--)
            {
                if (Math.Abs((sortedData[i - 1] - sortedData[i]) / (sortedData[i] - upLimit)) < delta)
                {
                    break;
                }
                upLimit = sortedData[i];
            }
            return new Tuple<double, double>(lowLimit, upLimit);
        }

        private double[,] NewTopo(IList<List<double>> topo)
        {
            // Граничные элементы матрицы Topo(i, j) при этом исключаются из рассмотрения
            var newTopo = new double[ActualImageSize, ActualImageSize];
            for (int n = N; n < Size; n++)
            {
                for (int m = N; m < Size; m++)
                {
                    newTopo[n - N, m - N] = NetTopoValue(topo, n, m);
                }
            }
            return newTopo;
        }

        private double NetTopoValue(IList<List<double>> topo, int n, int m)
        {
            double newValue = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (n >= i && m >= j)
                    {
                        newValue += LoG[i, j]*topo[n - i][m - j];
                    }
                }
            }
            return newValue;
        }


        //WHY YOU NOT WORK CORRECTLY BITCH
        public byte[] FilteredImage2(double value)
        {
            var result = new byte[ActualImageSize * ActualImageSize];
            for (int i = 0; i < ActualImageSize; i++)
            {
                for (int j = 0; j < ActualImageSize; j++)
                {
                    result[i*ActualImageSize + j] = _newTopo[i, j] < value ? byte.MaxValue : byte.MinValue;
                }
            }
            return result;
        }

        public BitmapSource FilteredImage(double value)
        {
            using (var bitmap = new Bitmap(ActualImageSize, ActualImageSize))
            {
                var processor = new FastBitmap(bitmap);
                processor.LockImage();
                for (int i = 0; i < ActualImageSize; i++)
                {
                    for (int j = 0; j < ActualImageSize; j++)
                    {
                        processor.SetPixel(i, j, _newTopo[i, j] < value ? Color.White : Color.Black);
                    }
                }

                processor.UnlockImage();
                return ToWpfBitmap(bitmap);
            }
        }

        public BitmapSource OriginalImageWithFilteredPoints(double value)
        {
            using (var bitmap = new Bitmap(ActualImageSize, ActualImageSize))
            {
                var processor = new FastBitmap(bitmap);
                processor.LockImage();
                for (int i = 0; i < ActualImageSize; i++)
                {
                    for (int j = 0; j < ActualImageSize; j++)
                    {
                        processor.SetPixel(i, j, _newTopo[i, j] < value ? Color.Firebrick : MakeColor(i + N / 2, j + N / 2));
                    }
                }

                processor.UnlockImage();
                return ToWpfBitmap(bitmap);
            }
        }

        public static BitmapSource ToWpfBitmap(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public Color MakeColor(int i, int j)
        {
            double value = _topo[i][j];
            if (value < _topoMin || value > _topoMax)
            {
                return Color.Black;
            }
            int x = (int)((value - _topoMin) / (_topoMax - _topoMin) * 255);
            return Color.FromArgb(x, x, x);
        }
    }
}