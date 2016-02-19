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
    [Table("Interpret")]
    public class Interpret
    {
        public Interpret()
        {
            this.Titles = new List<Title>();
            this.Id = 0;
        }

        [DataMember]
        [Key]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Searchname { get; set; }

        [DataMember]
        public virtual ICollection<Title> Titles { get; set; }
    }
}
