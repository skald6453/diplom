using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Diplom.ViewModels;
using Melanchall.DryWetMidi.Multimedia;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System.Linq;
using System;
using Avalonia.Controls.Templates;

namespace Diplom.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MusicamDbContext context = new();
        public MainWindowViewModel model = new();
       
        public SearchView searchView = new SearchView();
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowDialogFFT.RegisterHandler(DoShowDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowDialogSettings.RegisterHandler(DoShowDialogSettingsAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowDialogPlay.RegisterHandler(DoShowPlaySongDialogAsync)));
            this.WhenActivated(d=> d(ViewModel!.ShowSongView.RegisterHandler(DoShowSongViewAsync)));
        }
       
        private async Task DoShowDialogAsync(InteractionContext<FFTWindowViewModel, SettingsViewModel?> interaction)
        {
            var dialog = new FFTWindow
            {
                DataContext = new FFTWindowViewModel(),
            };
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<SettingsViewModel?>(this);
            interaction.SetOutput(result);
        }
        private async Task DoShowDialogSettingsAsync(InteractionContext<SettingsWindow, SettingsViewModel?> interaction)
        {
            var dialog = new SettingsWindow
            {
                DataContext = new SettingsViewModel(),
            };
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<SettingsViewModel?>(this);
            interaction.SetOutput(result);
        }

        private async Task DoShowPlaySongDialogAsync(InteractionContext<SongPlayWindow, SongPlayViewModel?> interaction)
        {
            var dialog = new SongPlayWindow
            {
                DataContext = new SongPlayViewModel(),
            };
            dialog.DataContext = interaction.Input;
            
            var result = await dialog.ShowDialog<SongPlayViewModel?>(this);
            interaction.SetOutput(result);
        }

        private async Task DoShowSongViewAsync(InteractionContext<MainWindowViewModel, SongViewModel?> interaction)
        {
            var dialog = new MainWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<SongViewModel?>(this);
            interaction.SetOutput(result);
        }
    }
}