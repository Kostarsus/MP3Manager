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
    [Table("Title")]
    public class Title
    {
        [DataMember]
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Fremschlüssel auf den Interpret des titels
        /// </summary>
        [DataMember]
        public long InterpretId { get; set; }

        /// <summary>
        /// Fremdschlüssel auf das Album
        /// </summary>
        [DataMember]
        public long AlbumId { get; set; }

        /// <summary>
        /// Der Name des Titels
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Die Länge als Text des Titels
        /// </summary>
        [DataMember]
        public string Length { get; set; }

        /// <summary>
        /// Die Bitrate, mit welcher dieser Titel gesamelt wurde
        /// </summary>
        [DataMember]
        public Nullable<int> Bitrate { get; set; }

        /// <summary>
        /// Die verbauchten Bytes 
        /// </summary>
        [DataMember]
        public Nullable<long> Bytes { get; set; }

        /// <summary>
        /// Der Pfad dees Titels
        /// </summary>
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// Der Dateiname des Titels
        /// </summary>
        [DataMember]
        public string Filename { get; set; }

        /// <summary>
        /// Der Suchname des Ttitels
        /// </summary>
        [DataMember]
        public string Searchname { get; set; }

        /// <summary>
        /// Die Tracknummer des Titels
        /// </summary>
        [DataMember]
        public int? Track { get; set; }

        /// <summary>
        /// Das Datum der Erstellung der Entität
        /// </summary>
        [DataMember]
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Das Datum der letzten Bearbeitung der Entität
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// Gibt an, ob dieser Titel in einer besondeeren Reihenfolge steht
        /// </summary>
        [DataMember]
        public bool IsOrdered { get; set; }

        /// <summary>
        /// befindet sich der titel in einer Collection unterschiedlicher Künstler?
        /// </summary>
        [DataMember]
        public bool IsCollection { get; set; }

        /// <summary>
        /// Die Dauer in Sekunden
        /// </summary>
        [DataMember]
        public int DurationInSeconds { get; set; }

        /// <summary>
        /// Das Genre des Titels
        /// </summary>
        [DataMember]
        public string Genre { get; set; }

        /// <summary>
        /// Das Jahr der veröffentlichung
        /// </summary>
        [DataMember]
        public int PublicationYear { get; set; }

        /// <summary>
        /// Die Sampelrate des Titels
        /// </summary>
        [DataMember]
        public int SampleRate { get; set; }

        /// <summary>
        /// Die Anzahl der Kanäle, die bei der Sampelun genutzt wurde
        /// </summary>
        [DataMember]
        public int Channels { get; set; }

        #region Navigation Properties
        [DataMember]
        public virtual Album Album { get; set; }

        [DataMember]
        public virtual Interpret Interpret { get; set; }

        #endregion

    }
}
