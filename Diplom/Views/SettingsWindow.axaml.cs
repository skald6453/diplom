using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Diplom.ViewModels;
using NAudio.CoreAudioApi;
using System;
using ScottPlot.Avalonia;
using FftSharp;
using NAudio.Wave;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.X86;
using DynamicData;
using System.Collections.Generic;
using Avalonia.Media;
using ScottPlot;
using System.Data;
using Avalonia.Input;
using System.Reflection;

namespace Diplom.Views;

public partial class SettingsWindow : Avalonia.Controls.Window
{
    NAudio.Wave.WaveInEvent? Wave;
    public readonly MMDevice[] AudioDevices = new MMDeviceEnumerator()
        .EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
        .ToArray();

    WasapiCapture AudioDevice;
    double[] AudioValues;
    double[] FftValues;
    public SettingsViewModel viewModel = new();
    //public static TimeSpan framerate = TimeSpan.FromSeconds(1 / 60);
    //public DispatcherTimer timer = new DispatcherTimer { Interval = framerate };
    public MusicamContext context = new();
    public SettingsWindow()
    {
        InitializeComponent();
        var boxitem = this.Find<ComboBox>("Inputs");
        boxitem = ListOfInputs(AudioDevices);
        var button = this.Find<Button>("Accept");
        context.Devices.RemoveRange(context.Devices);
        context.SaveChanges();
        //здесь можно просто перезаписать активное устройство в бд, а в fftwin брать из бд
        button.Click += (s, e) =>
        {
            Device inputDevice = new()
            {
                Id = boxitem.SelectedIndex,
                Name = AudioDevices[boxitem.SelectedIndex].DeviceFriendlyName,
            };
            context.Devices.Add(inputDevice);
            context.SaveChanges();
            this.Close();
        };
        //boxitem.SelectionChanged += new EventHandler<Avalonia.Controls.SelectionChangedEventArgs>();
    }

    public ComboBox ListOfInputs(MMDevice[] AudioDevices)
    {
        var boxitem = this.Find<ComboBox>("Inputs");
        List<string> inp = new();

        foreach (MMDevice device in AudioDevices)
        {
            string deviceType = device.DataFlow == DataFlow.Capture ? "INPUT" : "OUTPUT";
            string deviceLabel = $"{deviceType}: {device.FriendlyName}";
            inp.Add(deviceLabel);
        }
        viewModel.Inputs = inp;
        boxitem.Items = inp;
        return boxitem;
    }

   
}