using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.ViewModels
{
    public class FFTWindowViewModel : ViewModelBase
    {
        public List<string> InputName { get; set; } = new List<string>();
        public double Freq { get; set; }

        public WasapiCapture audiodevice { get; set; }
        public int ShownInput
        {
            get; set;
        }
    }
}
