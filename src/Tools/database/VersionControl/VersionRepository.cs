using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tools.database.VersionControl;

namespace Dargoskop.DataManager.DataAccess.general
{
    public class VersionRepository
    {
        #region Constructors

        public VersionRepository(string nameOrConnectionString)
        {
            this.NameOrConnectionString = nameOrConnectionString;
        }

        #endregion Constructors

        #region Properties

        public string NameOrConnectionString { get; private set; }

        #endregion Properties

        /// <summary>
        /// Liefert alle Gruppen nach den Campagnenname sortiert
        /// </summary>
        /// <returns></returns>
        public List<DBVersion> GetAllEntities()
        {
            using (var ctx = new VersionContext(this.NameOrConnectionString))
            {
                return ctx.DBVersions.ToList();
            }
        }

        /// <summary>
        /// Diesst die Konfiguration mit dem angegebenen Pfad
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DBVersion GetEntityById(long id)
        {
            using (var ctx = new VersionContext(this.NameOrConnectionString))
            {
                return ctx.DBVersions.Where(e => e.Id == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Diese Methode liefert die letzte ausgeführte SQL-Querries
        /// </summary>
        /// <returns></returns>
        public DBVersion LastExecuted()
        {
            using (var ctx = new VersionContext(this.NameOrConnectionString))
            {
                return ctx.DBVersions.OrderByDescending(e => e.Installdate).FirstOrDefault();
            }
        }

        /// <summary>
        /// Diese Methode liefert die höchste installierte DBVersion
        /// </summary>
        /// <returns></returns>
        public DBVersion HighestVersion()
        {
            using (var ctx = new VersionContext(this.NameOrConnectionString))
            {
                return ctx.DBVersions.OrderByDescending(e => e.PrimaryVersion).OrderByDescending(e => e.SecondaryVersion).FirstOrDefault();
            }
        }

        /// <summary>
        /// Diese Methode speichert eine DBVersion-Entität
        /// </summary>
        /// <param name="entity">
        /// Die zu speichernde Entität
        /// </param>
        /// <returns>
        /// Die gespeicherte Entität
        /// </returns>
        public DBVersion SaveUpdateEntity(DBVersion entity)
        {
            using (var context = new VersionContext(this.NameOrConnectionString))
            {
                if (context.GetValidationErrors() != null && context.GetValidationErrors().Count() > 0)
                {
                    //TODO: Erweitern um den Fehler
                    throw new ValidationException();
                }

                if (GetEntityById(entity.Id) == null)
                {
                    context.DBVersions.Add(entity);
                }
                else
                {
                    context.DBVersions.Attach(entity);
                    context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
                return entity;
            }
        }
    }
}