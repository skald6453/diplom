using Avalonia.Controls;
using Avalonia.Media;
using Diplom.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace Diplom.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        MusicamContext context = new();
        public MainWindowViewModel()
        {
            foreach(Song song in context.Songs)
            { 
                SongName = song.Name;
                Artist = song.Author;
                //SearchResults.Add($"Название:{song.Name} --- Автор:{song.Author} --- Жанр:{song.Genre} --- Сложность:{song.Difficulty} --- Длительность:{song.Duration}".Replace(" ", ""));
            }
            ShowDialogFFT = new Interaction<FFTWindowViewModel, SettingsViewModel?>();

            FFTGraphCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fft = new FFTWindowViewModel();

                var result = await ShowDialogFFT.Handle(fft);
            });

            ShowDialogSettings = new Interaction<SettingsWindow, SettingsViewModel?>();

            SettingsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var settings = new SettingsWindow();
                var result = await ShowDialogSettings.Handle(settings);
            });

        }


        public ICommand FFTGraphCommand { get; }
        public Interaction<FFTWindowViewModel, SettingsViewModel?> ShowDialogFFT { get; }
        public double Freq { get; set; }

        public ICommand SettingsCommand { get;}

        public Interaction<SettingsWindow, SettingsViewModel?> ShowDialogSettings { get; } 

        public int ShownInput
        {
            get; set;
        }

        private string? _searchText;

        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }
        
        public string SongName { get; set; }
        public string Artist { get; set; }

        private MainWindowViewModel _selectedAlbum;

        public List<string> SearchResults { get; } = new();

        public MainWindowViewModel? SelectedAlbum
        {
            get => _selectedAlbum;
            set => this.RaiseAndSetIfChanged(ref _selectedAlbum, value);
        }
    }
}