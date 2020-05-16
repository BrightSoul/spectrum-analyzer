using NAudio.CoreAudioApi;
using NAudio.Dmo;
using NAudio.Wave;
using NAudio.Wave.Asio;
using Nito.AsyncEx;
using SpectrumAnalyzer.Models;
using SpectrumAnalyzer.Models.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SpectrumAnalyzer.Services.Input
{
    public class WasapiInput : IInput
    {

        private int channels;
        private int bytesPerSample;

        private readonly AsyncCollection<Sample[]> sampleQueue = new AsyncCollection<Sample[]>(new ConcurrentQueue<Sample[]>());

        public int Priority => 30;

        public async IAsyncEnumerable<Sample[]> EnumerateSamplesAsync(InputOption option, [EnumeratorCancellation] CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(option.OptionName))
            {
                throw new ConfigurationNeededException();
            }

            var devices = GetDevices();
            var device = devices.First(d => d.FriendlyName == option.OptionName);

            using var capture = new WasapiLoopbackCapture(device);
            capture.ShareMode = AudioClientShareMode.Shared;
            option.SamplingFrequency = capture.WaveFormat.SampleRate;
            bytesPerSample = capture.WaveFormat.BitsPerSample / 8;
            
            EventHandler<WaveInEventArgs> handler = GetHandlerByEncoding(capture.WaveFormat.Encoding);
            capture.DataAvailable += handler;

            channels = capture.WaveFormat.Channels;
            //capture.WaveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 44100, 1, 44100 * 2, 2, 8 * 2);
            capture.StartRecording();

            while (await sampleQueue.OutputAvailableAsync(token))
            {
                var samples = await sampleQueue.TakeAsync(token);
                yield return samples;
            }
            capture.DataAvailable -= handler;
        }

        private EventHandler<WaveInEventArgs> GetHandlerByEncoding(WaveFormatEncoding encoding)
        {
            switch(encoding)
            {
                case WaveFormatEncoding.IeeeFloat:
                    return HandleIeeeFloatBuffer;
                default:
                    throw new NotImplementedException($"La codifica {encoding} al momento non è supportata");
            }
        }

        private void HandleIeeeFloatBuffer(object sender, WaveInEventArgs e)
        {
            var blockSize = bytesPerSample * channels;

            var samples = new Sample[e.BytesRecorded/blockSize];
            for (var i = 0; i < e.BytesRecorded; i+=blockSize)
            {
                //TODO: Check BitConverter.IsLittleEndian
                var value = BitConverter.ToSingle(e.Buffer, i) * short.MaxValue;
                samples[i/blockSize] = new Sample(new[] { value });
            }
            sampleQueue.Add(samples);
        }

        private MMDevice[] GetDevices()
        {
            using var enumerator = new MMDeviceEnumerator();
            return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToArray();
        }

        public InputOption[] GetInputOptions()
        {
            var devices = GetDevices();
                return devices
                .Select(device => {
                    try
                    {
                        return new InputOption(device.FriendlyName, this, "WASAPI");
                    } catch
                    {
                        return null;
                    }
                    })
                .Where(device => device != null)
                .ToArray();
        }

        public FrameworkElement CreateConfigUI(string driverName)
        {
            return new Button() { Content = driverName };
        }
    }
}
