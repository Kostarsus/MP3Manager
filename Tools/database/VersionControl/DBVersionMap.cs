using System.Data.Entity;

namespace Tools.database.VersionControl
{
    internal class DBVersionMap : IEntityMapping
    {
        public void MapEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DBVersion>()
                .Property(t => t.Id).HasColumnName("ID");
            modelBuilder.Entity<DBVersion>()
                .Property(t => t.Installdate).HasColumnName("Installdate");
            modelBuilder.Entity<DBVersion>()
                .Property(t => t.PrimaryVersion).HasColumnName("PrimaryVersion");
            modelBuilder.Entity<DBVersion>()
                .Property(t => t.SecondaryVersion).HasColumnName("SecondaryVersion");
        }
    }
}