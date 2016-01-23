using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.model
{
    /// <summary>
    /// Diese Aufzählung gibt den Status für den Download der Zusatzinormationen für das Album an
    /// </summary>
    [DataContract]
    public enum InformationDownloadStatus
    {
        /// <summary>
        /// Die Aktualisierung hat noch nicht begonnen
        /// </summary>
        [EnumMember]
        NotStarted = 0,

        /// <summary>
        /// Die Infomationen zu dem Album werden gelesen
        /// </summary>
        [EnumMember]
        Reading = 1,

        /// <summary>
        /// Die Informationen wurden gelesen
        /// </summary>
        [EnumMember]
        Done = 2
    }
}
