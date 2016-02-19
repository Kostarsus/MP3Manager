using System.Data.Entity;

namespace Tools.database
{
    public interface IEntityMapping
    {
        /// <summary>
        /// Diese Methode führt das Mapping der Entität durch
        /// </summary>
        /// <param name="modelBuilder"></param>
        void MapEntity(DbModelBuilder modelBuilder);
    }
}