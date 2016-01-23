using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.model
{
    [Serializable]
    [Table("MusicBrainz")]
    internal class MusicBrainzInformation
    {
        #region Properties
        [DataMember]
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Feemdschlüssel auf den Interpreten
        /// </summary>
        [DataMember]
        public long ArtistId { get; set; }

        /// <summary>
        /// Fremdschlüssel auf das Album
        /// </summary>
        [DataMember]
        public long AlbumId { get; set; }

        /// <summary>
        /// ID des Interpreten von MusicBrainz
        /// </summary>
        [DataMember]
        public string ArtistMBId { get; set; }

        /// <summary>
        /// ID des Albums bei MusicBrainz
        /// </summary>
        [DataMember]
        public string AlbumMBId { get; set; }

        /// <summary>
        /// Name des Titels aus der MusicBrainz-Datenabnk 
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// ID des Titels bei MusicBrainz
        /// </summary>
        [DataMember]
        public string TitleMBId { get; set; }

        #endregion

        #region Navigation-Properties
        [DataMember]
        public virtual Album Album { get; set; }

        [DataMember]
        public virtual Interpret Artist { get; set; }
        #endregion
    }
}
