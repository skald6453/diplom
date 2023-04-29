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
    public FFTWindow()
    {
        InitializeComponent();


        //надо придумать как вызывать методы ниже при смене значения в комбобоксе, а то так получается
        //что у меня один раз выбирается аудиовход и все, больше ничего не происходит
        //WasapiCapture audiodevice = GetSelectedDevice(boxitem);
        //FftMonitor(audiodevice);
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
        plt.Plot.AddSignal(FftValues, fftPeriod);
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
        label.Text = $"Peak Frequency: {inputNames.Freq} Hz";

        // request a redraw using a non-blocking render queue
        plt.RefreshRequest();
    }
}