using NAudio.SoundFont;
using SpectrumAnalyzer.Components;
using SpectrumAnalyzer.Helpers;
using SpectrumAnalyzer.Models;
using SpectrumAnalyzer.Models.Exceptions;
using SpectrumAnalyzer.Services.Aggregate;
using SpectrumAnalyzer.Services.Input;
using SpectrumAnalyzer.Services.Transform;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace SpectrumAnalyzer.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {

        private readonly ICollection<IInput> inputs;
        private readonly ITransformer transformer;
        private readonly IResampler resampler;

        public MainViewModel()
        {
            inputs = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IInput).IsAssignableFrom(t) && t.IsClass)
                .Select(t => Activator.CreateInstance(t))
                .Cast<IInput>()
                .OrderBy(input => input.Priority)
                .ToList();

            transformer = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ITransformer).IsAssignableFrom(t) && t.IsClass)
                .Select(t => Activator.CreateInstance(t))
                .Cast<ITransformer>()
                .First();

            resampler = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IResampler).IsAssignableFrom(t) && t.IsClass)
                .Select(t => Activator.CreateInstance(t))
                .Cast<IResampler>()
                .First();

            InputOptions = new ObservableCollection<InputOption>(
                       inputs.SelectMany(input => input.GetInputOptions())
                );

            EventLog = new ObservableCollection<Event>();

            Frequencies = new ObservableCollection<FrequencyViewModel>(
                new [] { 32, 64, 125, 250, 500, 1000, 2000, 4000, 8000, 16000 }
                .Select(value => new FrequencyViewModel(new Frequency(value))));

            SelectedInputOption = InputOptions[0];

            EventLogStart = DateTime.Now;
        }
        public ObservableCollection<FrequencyViewModel> Frequencies { get; }
        public ObservableCollection<InputOption> InputOptions { get; }
        public ObservableCollection<Event> EventLog { get; }

        private FrequencyViewModel selectedFrequency;
        public FrequencyViewModel SelectedFrequency
        {
            get => selectedFrequency;
            private set => Set(ref selectedFrequency, value);
        }

        private InputOption selectedInputOption;
        public InputOption SelectedInputOption
        {
            get => selectedInputOption;
            set
            {
                Set(ref selectedInputOption, value);
                _ = TryStartStreaming(value);
            }
        }

        private FrequencyAmplitude[] frequencyAmplitudes;
        public FrequencyAmplitude[] FrequencyAmplitudes
        {
            get => frequencyAmplitudes;
            set => Set(ref frequencyAmplitudes, value);
        }

        public DateTime EventLogStart
        {
            get; set;
        }

        private CancellationTokenSource cancellationTokenSource = null;
        private async Task TryStartStreaming(InputOption option)
        {
            SuspendAnyPreviousStreamingOption();
            try
            {
                await RenderSamples(cancellationTokenSource.Token);
                BeepItForCompletion();
            }
            catch (ConfigurationNeededException)
            {
                ShowConfigurationForSelectedInputOption();
            }
            catch (OperationCanceledException)
            {
                //Cancellation token was cancelled
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("Si è verificato un errore inatteso: " + exc.Message);
            }            
        }

        private async Task RenderSamples(CancellationToken token)
        {
            var option = selectedInputOption;
            await foreach (var frequencyAmplitues in transformer.EnumerateFrequencyAmplitudesAsync(option, token))
            {
                FrequencyAmplitudes = frequencyAmplitues;
                ResampleAndUpdateFrequencyAmplitudes(frequencyAmplitudes);
                CheckFrequencyAmplitudes();
            }
        }

        private void ResampleAndUpdateFrequencyAmplitudes(FrequencyAmplitude[] frequencyAmplitudes)
        {
            var desiredFrequencyAmplitudes = Frequencies.Select(f => f.Frequency).ToArray();
            var outputFrequencyAmplitudes = resampler.ResampleFrequencyAmplitudes(frequencyAmplitudes, desiredFrequencyAmplitudes).ToArray();
            for (var i = 0; i < outputFrequencyAmplitudes.Length; i++)
            {
                if (Frequencies[i].Frequency.Value != outputFrequencyAmplitudes[i].Frequency.Value)
                {
                    throw new InvalidOperationException("Frequencies values don't match");
                }
                var magnitude = outputFrequencyAmplitudes[i].Amplitude.Values[0];
                //TODO: calcola correttamente i decibel
                var amplitude = ((magnitude / 100) * 30) - 30;

                //var amplitude = Math.Min(0, Math.Max(-30, 20f * (float)Math.Log10(magnitude)));
                Frequencies[i].CurrentAmplitude = amplitude;
            }
        }

        private void ResetEventLog()
        {
            EventLogStart = DateTime.Now;
            EventLog.Clear();
            foreach(var frequency in Frequencies)
            {
                frequency.LastNotified = DateTime.MinValue;
            }
        }

        private void CheckFrequencyAmplitudes()
        {
            Console.WriteLine(Frequencies[0].Threshold);
            //TODO: verifica se superano la soglia
            for (var i = 0; i < Frequencies.Count; i++) {
                var amplitude = Frequencies[i].CurrentAmplitude;
                if (amplitude > Frequencies[i].Threshold)
                {
                    if (DateTime.Now.Subtract(Frequencies[i].LastNotified).Seconds > 5)
                    {
                        Frequencies[i].LastNotified = DateTime.Now;
                        var logEvent = new Event
                        {
                            Instant = DateTime.Now.Subtract(EventLogStart).ToString("hh':'mm':'ss"),
                            Frequency = Frequencies[i].ToString(),
                            Value = $"{amplitude:0.0}db"
                        };
                        EventLog.Add(logEvent);
                        SystemSounds.Asterisk.Play();
                    }
                }
            }
        }

        private void SelectFrequency(int index)
        {
            if (index < Frequencies.Count)
            {
                SelectedFrequency = Frequencies[index];
            }

            //SelectedFrequency = frequency;
        }

        private void ShowConfigurationForSelectedInputOption()
        {
            var option = selectedInputOption;
            var config = option.Owner.CreateConfigUI(option.OptionName);
            var dialog = new NotificationDialog(config);
            dialog.ShowDialog();
        }

        private void BeepItForCompletion()
        {
            SystemSounds.Beep.Play();
        }

        private void Exit()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void SuspendAnyPreviousStreamingOption()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
            cancellationTokenSource = new CancellationTokenSource();
        }
    }
}
