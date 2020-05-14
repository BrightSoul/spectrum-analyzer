using NAudio.SoundFont;
using SpectrumAnalyzer.Components;
using SpectrumAnalyzer.Helpers;
using SpectrumAnalyzer.Models;
using SpectrumAnalyzer.Models.Exceptions;
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
    public partial class MainViewModel : INotifyPropertyChanged
    {

        private readonly ICollection<IInput> inputs;
        private readonly ITransformer transformer;

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

            InputOptions = new ObservableCollection<InputOption>(
                       inputs.SelectMany(input => input.GetInputOptions())
                );

            Frequencies = new ObservableCollection<Frequency>(
                new []
                {
                    new Frequency(20),
                    new Frequency(40),
                    new Frequency(60),
                    new Frequency(100)
                });

            SelectedInputOption = InputOptions[0];
        }
        public ObservableCollection<Frequency> Frequencies { get; }
        public ObservableCollection<InputOption> InputOptions { get; }

        private Frequency selectedFrequency;
        public Frequency SelectedFrequency
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
            await foreach (var frequencyAmplitues in transformer.EnumerateFrequencyAmplitudesAsync(option.OptionName, option.Owner, token))
            {
                FrequencyAmplitudes = frequencyAmplitues;
                CheckFrequencyAmplitudes(frequencyAmplitudes);
            }
        }

        private void CheckFrequencyAmplitudes(FrequencyAmplitude[] frequencyAmplitudes)
        {
            //TODO: verifica se superano la soglia
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


        #region INotifyPropertyChanged implementation and helpers

        public event PropertyChangedEventHandler PropertyChanged;
        private void Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return;
            }
            backingField = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
