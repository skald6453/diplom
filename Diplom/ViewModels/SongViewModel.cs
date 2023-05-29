using Diplom.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diplom.Views;

namespace Diplom.ViewModels
{
    public class SongViewModel : ViewModelBase
    {
        private readonly SongMetadata _album;
        public SongViewModel(SongMetadata album)
        { 
            _album = album;
        }
        public string Cover =>_album.Cover;
        public string SongName => _album.Song;
        public string Artist => _album.Artist;
    }
}
