using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MP3ManagerBase.manager;

namespace MP3ManagerBase.factory
{
    /// <summary>
    /// This class reads the ID3-informations from an MP3-File
    /// </summary>
    public class MP3FileHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int MINUTES_IN_SECONDS = 60;
        private  const int HOURS_IN_SECONDS = MINUTES_IN_SECONDS * 60;
        private  const int DAYS_IN_SECONDS = HOURS_IN_SECONDS * 24;
        public const string ZERO_TIME = "00:00:00:00";

        /// <summary>
        /// This mehod converts the total seconds of a song to the formated string DD:HH:MM:SS
        /// </summary>
        /// <param name="totalSeconds">
        /// The total time of a song
        /// </param>
        /// <returns>
        /// The formated string
        /// </returns>
        private string convertTime(int totalSeconds)
        {
            if (totalSeconds == 0)
                return ZERO_TIME;
            int days = totalSeconds / DAYS_IN_SECONDS;
            int rest = totalSeconds - days * DAYS_IN_SECONDS;
            int hours = rest / HOURS_IN_SECONDS;
            rest -= hours * HOURS_IN_SECONDS;
            int minutes = rest / MINUTES_IN_SECONDS;
            rest -= minutes * MINUTES_IN_SECONDS;
            string returnValue = String.Format("{0:00}:{1:00}:{2:00}:{3:00}", days, hours, minutes, rest);
            return returnValue;

        }
        private string NormaliseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            name = name.Replace("\\", "_");
            name = name.Replace("/", "_");
            name = name.Replace(":", "_");
            name = name.Replace("*", "_");
            name = name.Replace("\"", "_");
            name = name.Replace("?", "_");
            name = name.Replace("<", "_");
            name = name.Replace(">", "_");
            name = name.Replace("|", "_");
            return name;
        }


        /// <summary>
        /// Reads the ID3-Tag Information and merges it with the physical file info
        /// </summary>
        /// <param name="fileInfo">
        /// The physical file info
        /// </param>
        /// <returns>
        /// A wrapper which includes the physical file info and theD3-Tag informations
        /// </returns>
        public WMP3FileInfo readFile(WFileInfo fileInfo)
        {
            if (fileInfo==null)
                throw new ArgumentNullException("fileInfo");
            string fullFilename = fileInfo.Path + fileInfo.Filename;
            TagLib.File file = null;
            try
            {
                file = TagLib.File.Create(new TagLibFileAbstraction(fullFilename));
            }
            catch (TagLib.UnsupportedFormatException)
            {                
                new System.IO.IOException("UNSUPPORTED FILE: " + fullFilename);
            }

            //Fill the return value
            WMP3FileInfo retValue = new WMP3FileInfo(fileInfo);
            foreach(var codec in file.Properties.Codecs)
            {
                TagLib.IAudioCodec acodec = codec as TagLib.IAudioCodec;
                if (acodec != null && (acodec.MediaTypes & TagLib.MediaTypes.Audio) != TagLib.MediaTypes.None)
                {
                    retValue.BitRate = acodec.AudioBitrate;
                    retValue.Songlength = acodec.Duration != null ? convertTime(Convert.ToInt32(acodec.Duration.TotalSeconds)) :  convertTime(0);
                    retValue.DurationInSeconds = acodec.Duration != null ? Convert.ToInt32(acodec.Duration.TotalSeconds) : 0;
                    retValue.Channels = acodec.AudioChannels;
                    retValue.SampleRate = acodec.AudioSampleRate;
                }
            }

            retValue.Genre = (file.Tag.Genres != null && file.Tag.Genres.Length > 0) ? file.Tag.Genres[0] : string.Empty;
            retValue.PublicationYear = (int)file.Tag.Year;
            retValue.Album = NormaliseName(file.Tag.Album);
            retValue.Interpret = NormaliseName(file.Tag.FirstPerformer);
            retValue.Title = file.Tag.Title;
            retValue.Track = (int)file.Tag.Track;
            return retValue;
        }

        /// <summary>
        /// Diese Methode ändert die Informationen im MP3-Tag
        /// </summary>
        /// <param name="filename">
        /// Vollqualifizierter Dateiname
        /// </param>
        /// <param name="interpretName">
        /// Name des Interpreten
        /// </param>
        /// <param name="albumName">
        /// Name des Albums
        /// </param>
        /// <param name="titleName">
        /// Name des Titels
        /// </param>
        public void ChangeMP3Tag(string filename, string interpretName, string albumName, string titleName)
        {
            TagLib.File file = null;
            try
            {
                file = TagLib.File.Create(new TagLibFileAbstraction(filename));
            }
            catch (TagLib.UnsupportedFormatException)
            {
                new System.IO.IOException("UNSUPPORTED FILE: " + filename);
            }

            List<string> performers = new List<string>();
            performers.Add(interpretName);
            file.Tag.Performers = performers.ToArray();

            file.Tag.Album = albumName;
            file.Tag.Title = titleName;
            file.Save();
        }
    }
}
