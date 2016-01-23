using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Tools.database.VersionControl
{
    /// <summary>
    /// Diese Code-First Entität hat den Tabellennamen VersionEntities fest vorgegeben
    /// </summary>
    [Table("VersionEntities")]
    public class DBVersion
    {
        #region Properties

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public int PrimaryVersion { get; set; }

        [DataMember]
        public int SecondaryVersion { get; set; }

        [DataMember]
        public System.DateTime Installdate { get; set; }

        #endregion Properties
    }
}