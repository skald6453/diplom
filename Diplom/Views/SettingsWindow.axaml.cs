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
    public List<string> inputNames = new();
    public static TimeSpan framerate = TimeSpan.FromSeconds(1 / 60);
    public DispatcherTimer timer = new DispatcherTimer { Interval = framerate };
    public SettingsWindow()
    {
        InitializeComponent();
        var boxitem = this.Find<ComboBox>("Inputs");
        boxitem = ListOfInputs(AudioDevices);
        var button = this.Find<Button>("Accept");
        //здесь можно просто перезаписать активное устройство в бд, а в fftwin брать из бд
        button.Click += (s, e) =>
        {
            WasapiCapture audiodevice = GetSelectedDevice(boxitem);
            this.Close();
        };
        //boxitem.SelectionChanged += new EventHandler<Avalonia.Controls.SelectionChangedEventArgs>();
    }

    public ComboBox ListOfInputs(MMDevice[] AudioDevices)
    {
        var boxitem = this.Find<ComboBox>("Inputs");


        foreach (MMDevice device in AudioDevices)
        {
            string deviceType = device.DataFlow == DataFlow.Capture ? "INPUT" : "OUTPUT";
            
            string deviceLabel = $"{deviceType}: {device.FriendlyName}";
            inputNames.Add(deviceLabel);
        }
      
        boxitem.Items = inputNames;

        return boxitem;
    }

    private WasapiCapture GetSelectedDevice(ComboBox boxitem)
    {

        MMDevice selectedDevice = AudioDevices[boxitem.SelectedIndex];
        return selectedDevice.DataFlow == DataFlow.Render
            ? new WasapiLoopbackCapture(selectedDevice)
            : new WasapiCapture(selectedDevice, true, 10);
    }
}