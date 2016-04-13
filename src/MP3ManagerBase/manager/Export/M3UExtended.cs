using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MP3ManagerBase.helpers;

namespace MP3ManagerBase.manager.Export
{
    /// <summary>
    /// Dient zum Export einer Playliste im Extneded M3U-Format
    /// </summary>
    internal class M3UExtended : IExport
    {
        /// <summary>
        /// Generiert einen Eintrag für den Text-Export
        /// </summary>
        /// <param name="artistName">
        /// Nam des Interpreten
        /// </param>
        /// <param name="titleName">
        /// Name des Titels
        /// </param>
        /// <returns>
        /// Umgewandelte Zeichenkette im Text-Format
        /// </returns>
        private string ExportTextFormat(string artistName, string titleName)
        {
            return string.Format("#EXTINF:-1,{0} - {1}", artistName, titleName);
        }

        #region IExport
        public Encoding TextEncoding
        {
            get
            {
                return UTF8Encoding.UTF8;
            }
        }

        public byte[] Export(IEnumerable<WSongInformation> songs)
        {
            if (songs == null || songs.Count() == 0)
            {
                return null;
            }

            StringBuilder result = new StringBuilder();
            result.AppendLine("#EXTM3U");

            foreach (var song in songs)
            {
                result.AppendLine(ExportTextFormat(song.Interpret, song.Title));
            }

            return TextEncoding.GetBytes(result.ToString());
        }
        #endregion

    }
}
