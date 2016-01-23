using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MP3ManagerBase.factory
{
    /// <summary>
    /// The objectes of this class stores the information from the MP3-File. The inofrmations are read from the filesystem or the ID3-Tag in the MP3-File.
    /// </summary>
    public class WMP3FileInfo : WFileInfo
    {
        public const string UNKNOWN_INTERPRET = "Unknown";
        public const string UNKNOWN_ALBUM = "Various";

        private string _interpret = UNKNOWN_INTERPRET;
        private string _album = UNKNOWN_ALBUM;
        /// <summary>
        /// The interpret of the song
        /// </summary>
        public string Interpret 
        {
            get { return _interpret; }
            set { _interpret = value; }
        }

        /// <summary>
        /// The album where the song was played
        /// </summary>
        public string Album
        {
            get { return _album; }
            set { _album = value; }
        }

        /// <summary>
        /// The title of the song
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The bit-rate of encoding of the MP3-File (ID3-Information)
        /// </summary>
        public int BitRate { get; set; }
        /// <summary>
        /// The length of the song in the ID3-Information
        /// </summary>
        public string Songlength { get; set; }

        /// <summary>
        /// Die Dauer des Liedes in Sekunden
        /// </summary>
        public int DurationInSeconds { get; set; }

        /// <summary>
        /// The Tracknumber in the album
        /// </summary>
        public int Track { get; set; }

        /// <summary>
        /// Das Genre des Titels
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// Das Jahr der veröffentlichung
        /// </summary>
        public int PublicationYear { get; set; }

        /// <summary>
        /// Die Sampelrate des Titels
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// Die Anzahl der Kanäle, die bei der Sampelun genutzt wurde
        /// </summary>
        public int Channels { get; set; }

        public WMP3FileInfo() { }

        public WMP3FileInfo(WFileInfo source)
        {
            Path = source.Path;
            Filename = source.Filename;
            Bytes = source.Bytes;
            EditDate = source.EditDate;
        }
    }
}
