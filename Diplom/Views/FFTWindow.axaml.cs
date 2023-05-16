using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ScottPlot.Avalonia;
using FftSharp;
using NAudio.Wave;
using System.Linq;
using System.Reflection.Emit;
using System;
using NAudio.CoreAudioApi;
using System.Runtime.Intrinsics.X86;
using Diplom.ViewModels;
using DynamicData;
using System.Collections.Generic;
using Avalonia.Threading;
using Avalonia.Media;
using ScottPlot;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace Diplom.Views;

public partial class FFTWindow : Avalonia.Controls.Window
{
    NAudio.Wave.WaveInEvent? Wave;
    public readonly MMDevice[] AudioDevices = new MMDeviceEnumerator()
        .EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
        .ToArray();

    WasapiCapture AudioDevice;
    double[] AudioValues;
    double[] FftValues;
    public FFTWindowViewModel inputNames = new();
    public static TimeSpan framerate = TimeSpan.FromSeconds(1 / 60);
    public DispatcherTimer timer = new DispatcherTimer { Interval = framerate };
    public MusicamContext context = new();
    public Dictionary<string, double> noteBaseFreqs = new Dictionary<string, double>()
            {
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
            };
    private static Playback playback;
    public FFTWindow()
    {
        InitializeComponent();


        //надо придумать как вызывать методы ниже при смене значения в комбобоксе, а то так получается
        //что у меня один раз выбирается аудиовход и все, больше ничего не происходит
        WasapiCapture audiodevice = GetSelectedDevice();
        FftMonitor(audiodevice);
    
      
        
        //playback = file.GetPlayback(output);
        //playback.NotesPlaybackStarted += OnNotesPlaybackStarted;
        //playback.Start();
    }
    private static void OnNotesPlaybackStarted(object sender, NotesEventArgs e)
    {
        if (e.Notes.Any(n => n.NoteName == Melanchall.DryWetMidi.MusicTheory.NoteName.B))
            playback.Stop();
    }

    private WasapiCapture GetSelectedDevice()
    {
        string deviceName = "";
        int deviceIndex = 0;
        foreach(Device device in context.Devices)
        {
            deviceName = device.Name;
            deviceIndex = device.Id;
        }
        MMDevice selectedDevice = AudioDevices[deviceIndex];
        return selectedDevice.DataFlow == DataFlow.Render
            ? new WasapiLoopbackCapture(selectedDevice)
            : new WasapiCapture(selectedDevice, true, 10);
    }


    public void FftMonitor(WasapiCapture audiodevice)
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



        AvaPlot plt = this.Find<AvaPlot>("AvaPlot1");
        plt.Refresh();
        plt.Plot.AddSignal(filtered, fftPeriod);
        plt.Plot.YLabel("Spectral Power");
        plt.Plot.XLabel("Frequency (kHz)");
        plt.Plot.SetAxisLimits(0, 6000, 0, .005);
        plt.Refresh();

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
        AvaPlot plt = this.Find<AvaPlot>("AvaPlot1");
        double[] paddedAudio = FftSharp.Pad.ZeroPad(AudioValues);
        double[] fftMag = FftSharp.Transform.FFTmagnitude(paddedAudio);
        Array.Copy(fftMag, FftValues, fftMag.Length);
        double[] filtered = FftSharp.Filter.LowPass(paddedAudio, 8000, maxFrequency: 512);
        // find the frequency peak
        int peakIndex = 0;
        for (int i = 0; i < fftMag.Length; i++)
        {
            if (fftMag[i] > fftMag[peakIndex])
                peakIndex = i;
        }
        double fftPeriod = FftSharp.Transform.FFTfreqPeriod(AudioDevice.WaveFormat.SampleRate, fftMag.Length);
        double peakFrequency = fftPeriod * peakIndex;
        inputNames.Freq = Math.Round(peakFrequency);
        string noti = "";
        foreach(var note in noteBaseFreqs)
        {
           double baseFreq = note.Value;

            for (int i = 0; i < 9; i++)
            {
                if ((inputNames.Freq >= baseFreq - 1) && (inputNames.Freq < baseFreq + 1) || (inputNames.Freq == baseFreq))
                {
                    noti = note.Key; break;
                }

                baseFreq *= 2;
            }
        }
        label.Text = $"{noti}";

        // request a redraw using a non-blocking render queue
        plt.RefreshRequest();
    }
}