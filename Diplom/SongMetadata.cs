using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom
{
    public class SongMetadata
    {
        public SongMetadata(string artist, string song, string cover)
        { 
        
            Artist = artist;
            Song = song;
            Cover = cover;

        }
        public string Artist { get; set; }

        public string Song { get; set; }

        public string Cover { get; set; }
    }
}
