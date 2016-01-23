using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.model
{
    internal class MusicBrainzInformationMap
    {
        public void MapEntity(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MusicBrainzInformation>()
                .Property(t => t.Id).HasColumnName("Id");
            modelBuilder.Entity<MusicBrainzInformation>()
                .Property(t => t.AlbumId).HasColumnName("AlbumId");
            modelBuilder.Entity<MusicBrainzInformation>()
                .Property(t => t.ArtistId).HasColumnName("ArtistId");
            modelBuilder.Entity<MusicBrainzInformation>()
                .Property(t => t.ArtistMBId).HasColumnName("ArtistMBId").IsUnicode(false);
            modelBuilder.Entity<MusicBrainzInformation>()
                .Property(t => t.AlbumMBId).HasColumnName("AlbumMBId").IsUnicode(false);
            modelBuilder.Entity<MusicBrainzInformation>()
                .Property(t => t.Title).HasColumnName("Title").IsUnicode(false);
            modelBuilder.Entity<MusicBrainzInformation>()
                .Property(t => t.TitleMBId).HasColumnName("TitleMBId").IsUnicode(false);

            modelBuilder.Entity<MusicBrainzInformation>()
                        .HasRequired<Album>(t => t.Album)
                        .WithMany()
                        .HasForeignKey(e => e.AlbumId);
            modelBuilder.Entity<MusicBrainzInformation>()
                        .HasRequired<Interpret>(t => t.Artist)
                        .WithMany()
                        .HasForeignKey(e => e.ArtistId);
        }

    }
}
