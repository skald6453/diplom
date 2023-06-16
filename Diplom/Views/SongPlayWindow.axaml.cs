using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System.Linq;
using Diplom.ViewModels;
using Melanchall.DryWetMidi.MusicTheory;
using DynamicData.Binding;
using Avalonia.Threading;

namespace Diplom.Views;


public partial class SongPlayWindow : Window
{
    public static Playback playback;
    public static SongPlayViewModel viewModel = new();
    public static string notenum;
    public static TextBlock textblock;
    public SongPlayWindow()
    {
        InitializeComponent();
        bool isstopped = false;
        var playbut = this.Find<Button>("Play");
        var stopbut = this.Find<Button>("Stop");
        textblock = this.Find<TextBlock>("not");
        var output = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
        
        playbut.Click += (s, e) =>
        {
            //var album = listbox.SelectedItem as SongViewModel;
            //string? songname = album.SongName;
            //string name = songname.Replace(" ", "");
            var midifile = MidiFile.Read($"C:\\Users\\samael\\Desktop\\GTP\\505.mid");
            playback = midifile.GetPlayback(output);
            playback.MoveToStart();
           
            playback.Start();
            playback.NotesPlaybackStarted += OnNotesPlaybackStarted;
            

        };

        stopbut.Click += (s, e) =>
        {
            isstopped = true;
            playback.Stop();
        };
    }
 

    public static void OnNotesPlaybackStarted(object sender, NotesEventArgs e)
    {
        foreach (var note in e.Notes)
        {
            notenum = note.ToString();
            viewModel.noteName = notenum;
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                textblock.Text = viewModel.noteName;
            });
        }

        //if (e.Notes.Any(n => n.NoteName == Melanchall.DryWetMidi.MusicTheory.NoteName.B))
        //    viewModel.NoteName.Add(e.Notes.ToString());
    }
}