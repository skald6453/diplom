using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using ReactiveUI;

namespace Diplom.ViewModels
{
    //может быть надо сделать как с окном поиска, при клике в axamlcs на кнопку, "создавать" окно которое будет отображать ноты?
    public class SongPlayViewModel : ViewModelBase
    {

        public string noteName;
        
        public string NoteName
        {
            get => noteName;
            set => this.RaiseAndSetIfChanged(ref noteName, value);
        }

    }
}
