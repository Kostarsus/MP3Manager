using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.model
{
    internal class InterpretMap
    {
        public void MapEntity(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Interpret>()
                .Property(t => t.Id).HasColumnName("Id");
            modelBuilder.Entity<Interpret>()
                .Property(t => t.Name).HasColumnName("Name").IsUnicode(false);
            modelBuilder.Entity<Interpret>()
                .Property(t => t.Searchname).HasColumnName("Searchname");

            modelBuilder.Entity<Interpret>()
                        .HasMany<Title>(t => t.Titles)
                        .WithRequired(e => e.Interpret)
                        .HasForeignKey(e => e.InterpretId);

        }

    }
}
