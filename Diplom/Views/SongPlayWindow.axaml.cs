using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
namespace Diplom.Views;


public partial class SongPlayWindow : Window
{
    public static Playback playback;
    public SongPlayWindow()
    {
        InitializeComponent();

        
        var playbut = this.Find<Button>("Play");
        var shownote = this.Find<Label>("CurrentNote");
        var output = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
        playbut.Click += (s, e) =>
        {
            var album = listbox.SelectedItem as SongViewModel;
            string? songname = album.SongName;
            string name = songname.Replace(" ", "");
            var midifile = MidiFile.Read($"C:\\Users\\samael\\Desktop\\GTP\\{name}.mid");
            playback = midifile.GetPlayback(output);
            playback.MoveToStart();
            playback.Start();
        };
    }
}