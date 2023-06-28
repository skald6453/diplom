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
using NAudio.Wave;
using NAudio;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using ScottPlot.Avalonia;
using ScottPlot.Styles;
using System.Drawing;
using Avalonia.Media;
using Brushes = Avalonia.Media.Brushes;

namespace Diplom.Views;


public partial class SongPlayWindow : Window
{
    NAudio.Wave.WaveInEvent? Wave;
    public readonly MMDevice[] AudioDevices = new MMDeviceEnumerator()
        .EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
        .ToArray();

    WasapiCapture AudioDevice;
    double[] AudioValues;
    double[] FftValues;
    public FFTWindowViewModel inputNames = new();
    public static TimeSpan framerate = TimeSpan.FromSeconds(1 / 30);
    public DispatcherTimer timer = new DispatcherTimer { Interval = framerate };
    public MusicamDbContext context = new();

    public Dictionary<string, double> noteBaseFreqs = new Dictionary<string, double>()
            {
                { "C2", 65 },
                { "C#2", 69 },
                { "D2", 73 },
                { "D#2", 77 },
                { "E2", 82 },
                { "F2", 87 },
                { "F#2", 92 },
                { "G2", 98 },
                { "G#2", 103 },
                { "A2", 110 },
                { "A#2", 116 },
                { "B2", 123 },
                { "C3", 130 },
                { "C#3", 138 },
                { "D3", 146 },
                { "Eb3", 155 },
                { "E3", 164 },
                { "F3", 174 },
                { "F#3", 185 },
                { "G3", 196 },
                { "G#3", 207 },
                { "A3", 220 },
                { "Bb3", 233 },
                { "B3", 246 },
                { "C4", 261.63f },
                { "C#4", 277.18f },
                { "D4", 293.66f },
                { "Eb4", 311.13f },
                { "E4", 329.63f },
                { "F4", 349.23f },
                { "F#4", 369.12f },
                { "G4", 392 },
                { "G#4", 415 },
                { "A4", 440 },
                { "Bb4", 466 },
                { "B4", 493 },
                { "C5", 523 },
                { "C#5", 554 },
                { "D5", 587 },
                { "D#5", 622 },
                { "E5", 659 },
                { "F5", 698 },
                { "F#5", 739 },
                { "G5", 784 },
                { "G#5", 830 },
                { "A5", 880 },
                { "A#5", 932 },
                { "B5", 987 },


            };

    public static Playback playback;
    public static SongPlayViewModel viewModel = new();
    public static string notenum;
    public static TextBlock textblock;
    public SongPlayWindow()
    {
        
        InitializeComponent();
        WasapiCapture audiodevice = GetSelectedDevice();
        NoteMonitor(audiodevice);
        var playbut = this.Find<Button>("Play");
        var stopbut = this.Find<Button>("Stop");
        textblock = this.Find<TextBlock>("not");
        var output = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
        
        playbut.Click += (s, e) =>
        {
            //var album = listbox.SelectedItem as SongViewModel;
            //string? songname = album.SongName;
            //string name = songname.Replace(" ", "");
            var midifile = MidiFile.Read($"D:\\diplomFull\\Songs\\505\\505voc.mid");
            playback = midifile.GetPlayback(output);
            playback.MoveToStart();
           
            playback.Start();
            playback.NotesPlaybackStarted += OnNotesPlaybackStarted;
            

        };

        stopbut.Click += (s, e) =>
        {
            
            playback.Stop();
        };
    }

    private WasapiCapture GetSelectedDevice()
    {
        string deviceName = "";
        int deviceIndex = 0;
        foreach (Devices2 device in context.Devices2s)
        {
            deviceName = device.Name;
            deviceIndex = (int)device.Id;
        }
        MMDevice selectedDevice = AudioDevices[deviceIndex];
        return selectedDevice.DataFlow == DataFlow.Render
            ? new WasapiLoopbackCapture(selectedDevice)
            : new WasapiCapture(selectedDevice, true, 10);
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

    public void NoteMonitor(WasapiCapture audiodevice)
    {
        AudioDevice = audiodevice;
        WaveFormat fmt = audiodevice.WaveFormat;

        AudioValues = new double[(fmt.SampleRate - 40000)];



        double[] paddedAudio = FftSharp.Pad.ZeroPad(AudioValues);
        double[] fftMag = FftSharp.Transform.FFTpower(paddedAudio);
        FftValues = new double[fftMag.Length];
        double fftPeriod = FftSharp.Transform.FFTfreqPeriod((fmt.SampleRate - 40000), fftMag.Length);
        var window = new FftSharp.Windows.Hanning();
        double[] windowed = window.Apply(FftValues);

        


        double[] filtered = FftSharp.Filter.LowPass(paddedAudio, 8000, maxFrequency: 512);

        AudioDevice.DataAvailable += WaveIn_DataAvailable;
        AudioDevice.StartRecording();
        Updater();
    }

    public void Updater()
    {
        timer.Start();
        timer.Tick += new EventHandler(Timer1_Tick);
    }

    void WaveIn_DataAvailable(object? sender, NAudio.Wave.WaveInEventArgs e)
    {
        int bytesPerSamplePerChannel = AudioDevice.WaveFormat.BitsPerSample / 8;
        int bytesPerSample = bytesPerSamplePerChannel * AudioDevice.WaveFormat.Channels;
        int bufferSampleCount = e.Buffer.Length / bytesPerSample;

        if (bufferSampleCount >= AudioValues.Length)
        {
            bufferSampleCount = AudioValues.Length;
        }

        if (bytesPerSamplePerChannel == 2 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
        {
            for (int i = 0; i < bufferSampleCount; i++)
                AudioValues[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
        }
        else if (bytesPerSamplePerChannel == 4 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
        {
            for (int i = 0; i < bufferSampleCount; i++)
                AudioValues[i] = BitConverter.ToInt32(e.Buffer, i * bytesPerSample);
        }
        else if (bytesPerSamplePerChannel == 4 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
        {
            for (int i = 0; i < bufferSampleCount; i++)
                AudioValues[i] = BitConverter.ToSingle(e.Buffer, i * bytesPerSample);
        }
        else
        {
            throw new NotSupportedException(AudioDevice.WaveFormat.ToString());
        }
    }
    private void Timer1_Tick(object sender, EventArgs e)
    {
        TextBlock label = this.Find<TextBlock>("Label");

   

        double[] paddedAudio = FftSharp.Pad.ZeroPad(AudioValues);

        double[] fftMag = FftSharp.Transform.FFTmagnitude(paddedAudio);
        Array.Copy(fftMag, FftValues, fftMag.Length);
        double[] filtered = FftSharp.Filter.LowPass(paddedAudio, 8000, maxFrequency: 1024);
        // find the frequency peak
        int peakIndex = 0;
        for (int i = 0; i < fftMag.Length; i++)
        {
            if (fftMag[i] > fftMag[peakIndex])
                peakIndex = i;
        }
        double fftPeriod = FftSharp.Transform.FFTfreqPeriod(AudioDevice.WaveFormat.SampleRate, fftMag.Length);
        double peakFrequency = fftPeriod * peakIndex;
        inputNames.Freq = peakFrequency;
        string noti = "";
        foreach (var note in noteBaseFreqs)
        {
            double baseFreq = note.Value;

            //for (int i = 0; i < 9; i++)
            //{
                if ((inputNames.Freq >= baseFreq - 2) && (inputNames.Freq < baseFreq + 2) || (inputNames.Freq == baseFreq))
                {
                    noti = note.Key; break;
                }

                //baseFreq *= 2;
            //}
        }
        label.Text = $"{noti}";
        if (label.Text == textblock.Text)
        {
            label.Foreground = Brushes.Green; 
        }
        else label.Foreground = Brushes.Red;
    }
}