using Colorspace;
using Nito.AsyncEx;
using Spettro.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Spettro.Components
{
    /// <summary>
    /// Logica di interazione per SpectrumTimeline.xaml
    /// </summary>
    public partial class SpectrumTimeline : UserControl
    {
        public static readonly DependencyProperty FrequencyAmplitudesProperty =
            DependencyProperty.Register(
                "FrequencyAmplitudes", typeof(FrequencyAmplitude[]), typeof(SpectrumTimeline), new PropertyMetadata(Callback)
        );
        public FrequencyAmplitude[] FrequencyAmplitudes
        {
            get { return (FrequencyAmplitude[])GetValue(FrequencyAmplitudesProperty); }
            set { SetValue(FrequencyAmplitudesProperty, value); }
        }

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (underlyingQueue.Count <= 20)
            {
                renderingQueue.Add(e.NewValue as FrequencyAmplitude[]);
            }
            else
            {
                Debug.WriteLine($"{nameof(SpectrumTimeline)} skipped a frame");
            }
        }
        private static ConcurrentQueue<FrequencyAmplitude[]> underlyingQueue = new ConcurrentQueue<FrequencyAmplitude[]>();
        private static AsyncCollection<FrequencyAmplitude[]> renderingQueue = new AsyncCollection<FrequencyAmplitude[]>(underlyingQueue);

        public SpectrumTimeline()
        {
            InitializeComponent();
            Task.Run(RenderFrequencyAmplitudes);
        }

        private async Task RenderFrequencyAmplitudes()
        {
            using var bitmap = new Bitmap(1000, 200);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(System.Drawing.Color.Black);
            }
            var column = 0;
            while (await renderingQueue.OutputAvailableAsync())
            {
                try
                {
                    var frequencyAmplitudes = await renderingQueue.TakeAsync();
                    var actualFrequencyAmplitudes = frequencyAmplitudes.Where(f => f.Frequency.Value <= 15000).Select(a => a.Amplitude.Values[0]).ToArray();
                    actualFrequencyAmplitudes = Resample(actualFrequencyAmplitudes, bitmap.Height);
                    for (var i = 0; i < bitmap.Height; i++)
                    {
                        var color = GetColor(actualFrequencyAmplitudes[i]);
                        bitmap.SetPixel(column, bitmap.Height - i - 1, color);
                    }
                    column++;
                    if (column >= bitmap.Width)
                    {
                        column = 0;
                    }
                    var indicator = System.Drawing.Color.FromArgb(255, 50, 50, 50);
                    for (var i = 0; i < bitmap.Height; i++)
                    {
                        bitmap.SetPixel(column, i, indicator);
                    }
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, ImageFormat.Png);

                    Dispatcher.Invoke(() =>
                    {
                        Spectrogram.Source = Convert(ms);
                    });
                }
                catch (Exception)
                {

                }
            }
        }

        private float[] Resample(float[] source, int n)
        {
            //n destination length
            int m = source.Length; //source length
            float[] destination = new float[n];
            destination[0] = source[0];
            destination[n - 1] = source[m - 1];

            for (int i = 1; i < n - 1; i++)
            {
                float jd = ((float)i * (float)(m - 1) / (float)(n - 1));
                int j = (int)jd;
                destination[i] = source[j] + (source[j + 1] - source[j]) * (jd - (float)j);
            }
            return destination;
        }

        private static System.Drawing.Color GetColor(float amplitude)
        {
            var lowerLimit = -30f;
            var upperLimit = 0f;
            var factor = (Math.Max(lowerLimit, Math.Min(upperLimit, amplitude)) - lowerLimit) / (upperLimit - lowerLimit);
            //TODO: non cablare qui questi valori
            //var factor = Math.Min(1.0, (amplitude + 30) / 30.0);
            var hue = (factor * 0.375) + 0.7917;
            if (hue >= 1.0)
                hue -= 1.0;

            var luminance = factor;

                ColorHSL color = new ColorHSL(hue, 1.0, luminance);
                ColorRGB color2 = new ColorRGB(color);
                return System.Drawing.Color.FromArgb(System.Convert.ToInt32(color2.R * 255), System.Convert.ToInt32(color2.G * 255), System.Convert.ToInt32(color2.B * 255));

        }

        private static BitmapImage Convert(MemoryStream ms)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
