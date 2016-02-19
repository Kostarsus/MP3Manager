using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.manager
{
    /// <summary>
    /// Dies ist eine Wrapperklasse zur Vereinigung des Albums mit dem Interpreten
    /// </summary>
    internal class WArtistAlbum
    {

        /// <summary>
        /// Der Name des Interpreten
        /// </summary>
        public string ArtistName { get; set; }

        /// <summary>
        /// Der Primärschlüssel des Interpreten in der Datenbank
        /// </summary>
        public long ArtistId { get; set; }

        /// <summary>
        /// Der Name des Albums
        /// </summary>
        public string AlbumName { get; set; }

        /// <summary>
        /// Der Primärschlüssel des Albums in der Datenbank
        /// </summary>
        public long AlbumId { get; set; }
    }
}
