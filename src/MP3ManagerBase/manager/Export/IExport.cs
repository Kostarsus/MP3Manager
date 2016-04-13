using MP3ManagerBase.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.manager.Export
{
    /// <summary>
    /// Schnittstelle zur Implementierung der Exportlisten
    /// </summary>
    interface IExport
    {
        /// <summary>
        /// Das Encoding für den Bytearray
        /// </summary>
        Encoding TextEncoding { get; }

        /// <summary>
        /// Exportiert die übergebene Liste in das entsprechende Format
        /// </summary>
        /// <param name="songs">
        /// Liste der zu exportierenden Songs
        /// </param>
        /// <returns>
        /// Ein Byte-Array im exportierten Format
        /// </returns>
        byte[] Export(IEnumerable<WSongInformation> songs);
    }
}
