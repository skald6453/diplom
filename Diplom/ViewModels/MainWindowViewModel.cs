using Diplom.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace Diplom.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ShowDialogFFT = new Interaction<FFTWindowViewModel, TestViewModel?>();

            FFTGraphCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fft = new FFTWindowViewModel();

                var result = await ShowDialogFFT.Handle(fft);
            });

            ShowDialogSettings = new Interaction<SettingsWindow, TestViewModel?>();

            SettingsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var settings = new SettingsWindow();
                var result = await ShowDialogSettings.Handle(settings);
            });
        }
        public ICommand FFTGraphCommand { get; }
        public Interaction<FFTWindowViewModel, TestViewModel?> ShowDialogFFT { get; }
        public double Freq { get; set; }

        public ICommand SettingsCommand { get;}

        public Interaction<SettingsWindow, TestViewModel?> ShowDialogSettings { get; } 

        public int ShownInput
        {
            get; set;
        }
    }
}