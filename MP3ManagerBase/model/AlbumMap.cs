using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.model
{
    internal class AlbumMap
    {
        public void MapEntity(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>()
                .Property(t => t.Name).HasColumnName("Name").IsUnicode(false);
            modelBuilder.Entity<Album>()
                .Property(t => t.Searchname).HasColumnName("Searchname");
            modelBuilder.Entity<Album>()
                .Property(t => t.InformationStatus).HasColumnName("AdditionalStatus");

            modelBuilder.Entity<Album>()
                        .HasMany<Title>(t => t.Titles)
                        .WithRequired(e => e.Album)
                        .HasForeignKey(e => e.AlbumId);
        }
    }
}
