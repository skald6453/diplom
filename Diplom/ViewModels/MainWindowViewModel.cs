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
using Diplom.ViewModels;
using SQLitePCL;
using Microsoft.Data.Sqlite;

namespace Diplom.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        MusicamDbContext context = new();
        public MainWindowViewModel()
        {
            string cover = "D:\\diplomFull\\Diplom\\Assets\\songView.png";
            foreach (Songs2 song in context.Songs2s)
            {
                SongMetadata album = new(song.Author, song.Name, cover);
                SearchResults.Add(song.Name);
                SearchSongs.Add(new SongViewModel(album));
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

            ShowDialogPlay = new Interaction<SongPlayWindow, SongPlayViewModel>();

            PlaySongCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var song = new SongPlayWindow();
                var result = await ShowDialogPlay.Handle(song);
            });

            ShowSongView = new Interaction<MainWindowViewModel, SongViewModel>();
            
            this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(400))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(DoSearch!);
        }
        public async void DoSearch(string s)
        {
            SearchSongs.Clear();
            foreach (Songs2 song in context.Songs2s)
            {

                if (s != null && song.Name.Contains(s))
                {

                    string cover = "C:\\Users\\samael\\Desktop\\DungeonSynthProj\\finalcoverEE.jpg";
                    SongMetadata album = new(song.Author, song.Name, cover);
                    SearchSongs.Add(new SongViewModel(album));
                }
            }  
        }

        public ICommand SettingsCommand { get; }
        public ICommand FFTGraphCommand { get; }
        public ICommand PlaySongCommand { get; }
        public Interaction<FFTWindowViewModel, SettingsViewModel?> ShowDialogFFT { get; }
        public double Freq { get; set; }



        public Interaction<SettingsWindow, SettingsViewModel?> ShowDialogSettings { get; } 

        public Interaction<SongPlayWindow, SongPlayViewModel> ShowDialogPlay { get; }

        public Interaction<MainWindowViewModel, SongViewModel> ShowSongView { get; }


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
        
        private SongViewModel _selectedAlbum;

        public ObservableCollection<SongViewModel> SearchSongs { get; } = new();

        public List<string> SearchResults { get; } = new();

        public SongViewModel? SelectedAlbum
        {
            get => _selectedAlbum;
            set => this.RaiseAndSetIfChanged(ref _selectedAlbum, value);
        }
    }
}