using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.model
{
    internal class TitleMap
    {
        public void MapEntity(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Title>()
                .Property(t => t.Name).HasColumnName("Name").IsUnicode(false);
            modelBuilder.Entity<Title>()
                .Property(t => t.Searchname).HasColumnName("Searchname");
            modelBuilder.Entity<Title>()
                .Property(t => t.Bitrate).HasColumnName("Bitrate");
            modelBuilder.Entity<Title>()
                .Property(t => t.Bytes).HasColumnName("Bytes");
            modelBuilder.Entity<Title>()
                .Property(t => t.CreationDate).HasColumnName("CreationDate");
            modelBuilder.Entity<Title>()
                .Property(t => t.EditDate).HasColumnName("EditDate");
            modelBuilder.Entity<Title>()
                .Property(t => t.Filename).HasColumnName("Filename");
            modelBuilder.Entity<Title>()
                 .Property(t => t.IsCollection).HasColumnName("IsCollection");
            modelBuilder.Entity<Title>()
                .Property(t => t.DurationInSeconds).HasColumnName("Duration");
            modelBuilder.Entity<Title>()
                .Property(t => t.IsOrdered).HasColumnName("IsOrdered");
            modelBuilder.Entity<Title>()
                .Property(t => t.Length).HasColumnName("Length");
            modelBuilder.Entity<Title>()
                .Property(t => t.Path).HasColumnName("Path");
            modelBuilder.Entity<Title>()
                .Property(t => t.Track).HasColumnName("Track");
            modelBuilder.Entity<Title>()
                .Property(t => t.AlbumId).HasColumnName("Album_ID");
            modelBuilder.Entity<Title>()
                .Property(t => t.InterpretId).HasColumnName("Interpret_ID");
            modelBuilder.Entity<Title>()
                .Property(t => t.Genre).HasColumnName("Genre");
            modelBuilder.Entity<Title>()
                .Property(t => t.PublicationYear).HasColumnName("Year");
            modelBuilder.Entity<Title>()
                .Property(t => t.SampleRate).HasColumnName("SampleRate");
            modelBuilder.Entity<Title>()
                .Property(t => t.Channels).HasColumnName("Channels");


        }
    }
}
