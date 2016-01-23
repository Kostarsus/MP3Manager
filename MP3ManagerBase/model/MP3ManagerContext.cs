using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MP3ManagerBase.model
{
    internal class MP3ManagerContext : DBContext
    {
        #region Konstruktoren

        public MP3ManagerContext() : base("Name=MP3ManagerConnection")
        {
        }

        #endregion Konstruktoren

        #region Properties

        public DbSet<Album> Albums { get; set; }

        public DbSet<Interpret> Interprets { get; set; }

        public DbSet<Title> Titles { get; set; }

        public DbSet<MusicBrainzInformation> MusicBrainzInformation { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            new AlbumMap().MapEntity(modelBuilder);
            new InterpretMap().MapEntity(modelBuilder);
            new TitleMap().MapEntity(modelBuilder);
            new MusicBrainzInformationMap().MapEntity(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        #endregion Methods
    }
}
