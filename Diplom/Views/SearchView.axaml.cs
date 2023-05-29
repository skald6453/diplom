using Avalonia.Controls;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Diplom.ViewModels;

namespace Diplom.Views
{
    public partial class SearchView : UserControl
    {
        public static Playback playback;
        public SearchView()
        {
            InitializeComponent();

            var listbox = this.Find<ListBox>("Songs");
            var playButton = this.Find<Button>("PlayBut");
            var stopButton = this.Find<Button>("StopBut");
            var output = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
            playButton.Click += (s, e) =>
            {
                var album = listbox.SelectedItem as SongViewModel;
                string? songname = album.SongName;
                string name = songname.Replace(" ", "");
                var midifile = MidiFile.Read($"D:\\diplomFull\\Songs\\{name}\\{name}.mid");
                playback = midifile.GetPlayback(output);
                playback.MoveToStart();
                playback.Start();
            };
            stopButton.Click += (s, e) =>
            {
                playback.Stop();
            };
        }
    }
}
