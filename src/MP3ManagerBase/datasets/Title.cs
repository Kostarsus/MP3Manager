//------------------------------------------------------------------------------
// <auto-generated>
//    Dieser Code wurde aus einer Vorlage generiert.
//
//    Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten Ihrer Anwendung.
//    Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MP3ManagerBase.datasets
{
    using System;
    using System.Collections.Generic;
    
    public partial class Title
    {
        public int Interpret_ID { get; set; }
        public int Album_ID { get; set; }
        public string Name { get; set; }
        public string Length { get; set; }
        public Nullable<int> Bitrate { get; set; }
        public Nullable<long> Bytes { get; set; }
        public string Path { get; set; }
        public string Filename { get; set; }
        public string Searchname { get; set; }
        public Nullable<int> Track { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsOrdered { get; set; }
        public Nullable<int> MP3TagStatus { get; set; }
        public int Id { get; set; }
        public bool IsCollection { get; set; }
    
        public virtual Album Album { get; set; }
        public virtual Interpret Interpret { get; set; }
    }
}