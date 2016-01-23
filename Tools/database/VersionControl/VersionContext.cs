using System.Data.Entity;

namespace Tools.database.VersionControl
{
    internal class VersionContext : DBContext
    {
        #region Constructors

        /// <summary>
        /// Standard-Konstruktor
        /// </summary>
        /// <param name="nameOrConnectionString">
        /// Der Connection-String in dem die Tabellen fürdie Version angelegt sind
        /// </param>
        public VersionContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        #endregion Constructors

        #region Properties

        public DbSet<DBVersion> DBVersions { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            new DBVersionMap().MapEntity(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        #endregion Methods
    }
}