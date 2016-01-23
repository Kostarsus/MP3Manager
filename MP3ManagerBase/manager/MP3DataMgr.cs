using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MP3ManagerBase.model;

using MP3ManagerBase.factory;
using MP3ManagerBase.helpers;
using System.Data.SqlServerCe;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using System.Windows.Forms;
using System.Data.Entity.Core;
using System.Transactions;

namespace MP3ManagerBase.manager
{
    /// <summary>
    /// This class implements methods to interact with the database to arrange the entities of the MP3-Files
    /// </summary>
    public class MP3DataMgr
    {
        public delegate void DBEventHandler(object sender, string title);

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event DBEventHandler FileInserted;

        private readonly static MP3DataMgr _instance = new MP3DataMgr();

        #region Properties
        public static MP3DataMgr Instance { get { return _instance; } }
        #endregion Properties

        #region Constructors
        private MP3DataMgr()
        {
        }
        #endregion Constructors

        /// <summary>
        /// This method checks if a file is inserted in the database. The check isn't case-sensitive.
        /// </summary>
        /// <param name="path">
        /// A searchcriteria. It's the filepath of the MP3-File
        /// </param>
        /// <param name="filename">
        /// A searchcriteria. It's the filename of the file
        /// </param>
        /// <returns>
        /// True if the file is in the database, false else
        /// </returns>
        public bool ExistFileInDatabase(String path, String filename)
        {
            log.DebugFormat("ExistFileInDatabase: path={0}, filename={1}", path, filename);
            if (path == null)
                throw new ArgumentNullException("path");
            if (filename == null)
                throw new ArgumentNullException("filename");

            using (var context = new MP3ManagerContext())
            {
                var titles = context.Titles;
                var check = from e in titles
                            where e.Path.ToUpper() == @path.ToUpper()
                            && e.Filename.ToUpper() == @filename.ToUpper()
                            select e.Id;
                int count = check.Count();
                log.DebugFormat("count={0}", count);
                return count > 0;
            }
        }
        /// <summary>
        /// Diese Methode filtert die Elemente aus, die in die Datenbank nicht vorhanden sind
        /// </summary>
        /// <param name="fileInfos">
        /// Eine Liste von Dateien, die  überprüft werden müssen
        /// </param>
        /// <returns></returns>
        public IEnumerable<WFileInfo> ExtractDatabaseFromList(IEnumerable<WFileInfo> fileInfos)
        {
            List<WFileInfo> returnValues = new List<WFileInfo>();
            if (fileInfos == null)
            {
                log.Warn("ExtractDatabaseFromList: fileInfos == null");
                return returnValues;
            }

            using (var context = new MP3ManagerContext())
            {
                var titles = context.Titles;
                var pathFilename = from title in titles
                                   select new { Filename = title.Filename, Path = title.Path };
                var pathFilenameList = pathFilename.ToList();
                foreach (var fi in fileInfos)
                {
                    log.DebugFormat("ExtractDatabaseFromList: Überprüfe Datei path={0}, filename={1}", fi.Path, fi.Filename);
                    bool doInsert = true;
                    foreach (var listElement in pathFilenameList)
                    {
                        if (listElement.Filename == fi.Filename && listElement.Path == fi.Path)
                        {
                            doInsert = false;
                            break;
                        }
                    }
                    if (doInsert)
                    {
                        log.DebugFormat("ExtractDatabaseFromList: Füge Datei in Datenbank hinzu path={0}, filename={1}", fi.Path, fi.Filename);
                        WFileInfo newInfo = new WFileInfo { Filename = fi.Filename, Path = fi.Path, Bytes = fi.Bytes, EditDate = fi.EditDate };
                        returnValues.Add(newInfo);
                    }
                }
            }
            return returnValues;
        }
        /// <summary>
        /// This method checks if a file is inserted in the database. The check isn't case-sensitive.
        /// </summary>
        /// <param name="fileInfo">
        /// A searchcriteria to  extract the informations of the file location
        /// </param>
        /// <returns></returns>
        public bool ExistFileInDatabase(WFileInfo fileInfo)
        {
            return ExistFileInDatabase(fileInfo.Path, fileInfo.Filename);
        }
        /// <summary>
        /// This method checks, if an artist is stored in the database
        /// </summary>
        /// <param name="artist">
        /// The searchcriteria of the artist
        /// </param>
        /// <returns>
        /// True if the artist ist stored, false else
        /// </returns>
        public bool ExistArtist(string artist)
        {
            if (String.IsNullOrWhiteSpace(artist))
            {
                log.Warn("ExistArtist: artist == null or empty");
            }
            log.DebugFormat("ExistArtist:artist={0}", artist);

            using (var context = new MP3ManagerContext())
            {
                var interprets = context.Interprets;
                var check = from e in interprets
                            where e.Searchname.ToUpper().Trim() == @artist.ToUpper().Trim()
                            select e.Name;
                int count = check.Count();
                log.DebugFormat("ExistArtist: Count={0}", count);
                return check.Count() > 0;
            }
        }
        /// <summary>
        /// This method searches for the first occurance  of an artist
        /// </summary>
        /// <param name="artist">
        /// Searchcriteria of the artistname
        /// </param>
        /// <returns>
        /// Returns the first occurance of the artist, null else
        /// </returns>
        public Interpret FindArtist(string artist)
        {
            log.DebugFormat("FindArtist:  artist={0}", artist);
            if (artist == null)
            {
                throw new ArgumentException("Artist == null");
            }

            using (var context = new MP3ManagerContext())
            {
                var interprets = context.Interprets;
                var retVal = from e in interprets
                             where e.Searchname.ToUpper().Trim() == @artist.ToUpper().Trim()
                             select e;
                if (retVal.Count() == 0)
                {
                    log.Debug("FindArtist: Interpret nicht gefunden");
                    return null;
                }
                return retVal.First();
            }
        }
        /// <summary>
        /// This method searches for the album of  an artist
        /// </summary>
        /// <param name="albumname">
        /// Searchcriterianame of the album
        /// </param>
        /// <param name="artist">
        /// Searchcriteria Artist of the album
        /// </param>
        /// <returns>
        /// Returns the first occurance of the albu, null else
        /// </returns>
        public Album FindAlbum(string albumname)
        {

            using (var context = new MP3ManagerContext())
            {
                var albums = context.Albums;
                var retVal = from album in albums
                             where album.Searchname.ToUpper().Trim() == @albumname.ToUpper().Trim()
                             select album;
                if (retVal.Count() == 0)
                {
                    log.DebugFormat("FindAlbum: albumname={0}", albumname);
                    return null;
                }
                return retVal.First();
            }
        }

        /// <summary>
        /// This method inserts a new interpret-row
        /// </summary>
        /// <param name="mp3Info">
        /// The data of  the interpeet-row
        /// </param>
        /// <returns>
        /// The new Row if the insertion was successfull, null else
        /// </returns>
        private Interpret insertInterpret(WMP3FileInfo mp3Info)
        {
            if (mp3Info == null)
            {
                throw new ArgumentException("mp3Info == null");
            }
            if (String.IsNullOrWhiteSpace(mp3Info.Interpret))
            {
                mp3Info.Interpret = WMP3FileInfo.UNKNOWN_INTERPRET;
            }

            log.DebugFormat("InsertInterpret: interpret={0}", mp3Info.Interpret);
            string normalizedName = CreateSearchname(mp3Info.Interpret);
            var interpretRow = FindArtist(normalizedName);
            if (interpretRow == null)
            {
                log.Debug("Insert new Interpret");
                using (var context= new MP3ManagerContext())
                {
                    interpretRow = context.Interprets.Add(new Interpret()
                        {
                            Name=mp3Info.Interpret,
                            Searchname = normalizedName
                        });
                    context.SaveChanges();
                }
            }
            return interpretRow;
        }

        /// <summary>
        /// This method inserts a new row in the album table
        /// </summary>
        /// <param name="mp3Info">
        /// The datato insert
        /// </param>
        /// <param name="interpretRow">
        /// The referenced interpretRow
        /// </param>
        /// <returns>
        /// The new albumrrow if the innsertion was successfull, null else
        /// </returns>
        private Album InsertAlbum(WMP3FileInfo mp3Info)
        {
            if (mp3Info == null)
            {
                throw new ArgumentException("mp3Info == null");
            }
            if (String.IsNullOrWhiteSpace(mp3Info.Album))
            {
                mp3Info.Album = WMP3FileInfo.UNKNOWN_ALBUM;
            }

            log.DebugFormat("InsertAlbum: albumname={0}", mp3Info.Album);
            string normalizedName = CreateSearchname(mp3Info.Album);
            var albumRow = FindAlbum(normalizedName);
            if (albumRow == null)
            {
                log.Debug("Insert new Album");
                using (var context = new MP3ManagerContext())
                {
                        albumRow = context.Albums.Add(new Album()
                        {
                            Name = mp3Info.Album, 
                            Searchname = normalizedName, 
                            InformationStatus = InformationDownloadStatus.NotStarted
                        });
                        context.SaveChanges();
                 }
            }
            return albumRow;
        }

        /// <summary>
        /// This method inserts a new tile row
        /// </summary>
        /// <param name="mp3Info">
        /// The data to insert
        /// </param>
        /// <param name="interpretRow">
        /// The referenced interpret row
        /// </param>
        /// <param name="albumRow">
        /// The referenced album row
        /// </param>
        /// <returns>
        /// The inserted title row if the insertion was successfull, null else
        /// </returns>
        private Title InsertTitle(WMP3FileInfo mp3Info, Interpret interpretRow, Album albumRow)
        {
            if (mp3Info == null)
            {
                throw new ArgumentException("mp3Info == null");
            }
            if (interpretRow == null)
            {
                throw new ArgumentException("interpretRow == null");
            }
            if (albumRow == null)
            {
                throw new ArgumentException("albumRow == null");
            }
            if (String.IsNullOrWhiteSpace(mp3Info.Title))
            {
                throw new ArgumentException("Title null or empty");
            }
            log.DebugFormat("InsertTitle: tile={0}", mp3Info.Title);
            Title titleRow = null;
            long start = DateTime.Now.Ticks;
            using (var context = new MP3ManagerContext())
            {
                string normalizedName = CreateSearchname(mp3Info.Title);
                titleRow = context.Titles.Add(new Title()
                 {
                     Interpret = interpretRow,
                     Album = albumRow,
                     Name = mp3Info.Title,
                     Length = mp3Info.Songlength,
                     Bitrate = mp3Info.BitRate,
                     Bytes = mp3Info.Bytes,
                     Path = mp3Info.Path,
                     Filename = mp3Info.Filename,
                     Searchname = normalizedName,
                     Track = mp3Info.Track,
                     CreationDate = DateTime.Now,
                     EditDate = mp3Info.EditDate,
                     IsOrdered = false,
                     IsCollection = false,
                     DurationInSeconds = mp3Info.DurationInSeconds,
                     Genre = mp3Info.Genre,
                     PublicationYear = mp3Info.PublicationYear,
                     SampleRate = mp3Info.SampleRate,
                     Channels = mp3Info.Channels
                 });
                context.SaveChanges();
                return titleRow;
            }
        }

        /// <summary>
        /// This  method read the ID3-Tag of the file and insert the values in the database
        /// </summary>
        /// <param name="fileInfo">
        /// The informations of the file
        /// </param>
        public List<WSynchronizeError> InsertFilesInDatabase(IEnumerable<WFileInfo> fileInfos)
        {
            if (fileInfos == null)
            {
                throw new ArgumentException("fileInfo == null");
            }
            if (fileInfos.Count() == 0)
            {
                return new List<WSynchronizeError>();
            }
            List<WSynchronizeError> errors = new List<WSynchronizeError>();
            MP3FileHandler mp3Reader = new MP3FileHandler();
            List<WMP3FileInfo> mp3Infos = new List<WMP3FileInfo>();
            var mp3InfoReadertasks = new Queue<Task>();
            ThreadPool.SetMinThreads(50, 50);
            var fileInfosArray=fileInfos.ToArray();
            var rangeFileInfo=Partitioner.Create(0, fileInfosArray.Length,5);
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = -1;
            Parallel.ForEach(rangeFileInfo, (range, loopstate) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    WFileInfo fileInfoParameter = fileInfosArray[i];
                    try
                    {
                        log.DebugFormat("ReadMP3_Info: path={0}, filename={1}", fileInfoParameter.Path, fileInfoParameter.Filename);
                        WMP3FileInfo newInfo = mp3Reader.readFile(fileInfoParameter);
                        mp3Infos.Add(newInfo);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new WSynchronizeError() { Path = fileInfoParameter.Path, Filename = fileInfoParameter.Filename, Message = ex.Message });
                    }
                }
            });
            var dbInsertTasks = new Queue<Task>();
            foreach (WMP3FileInfo mp3Info in mp3Infos)
            {
                if (mp3Info == null)
                    continue;

                    Interpret interpretRow = insertInterpret(mp3Info); 
                    Album albumRow = InsertAlbum(mp3Info);
                    InsertTitle(mp3Info, interpretRow, albumRow);
                    FileInserted(this, mp3Info.Title);
            }
            
            return errors;
        }


        /// <summary>
        /// This method searches in title.name, interpret.name and album.name for the searchstring
        /// </summary>
        /// <param name="searchString">
        /// SearchCriteria
        /// </param>
        /// <returns>
        /// Matched result
        /// </returns>
        public IEnumerable<WSearchItem> Search(string searchString)
        {
            log.DebugFormat("Search in Database: searchString={0}", searchString);
            string normalizeSearchString = CreateSearchname(searchString);
            using (var context = new MP3ManagerContext())
            {
                var result = context.Titles
                                    .Include("Interpret")
                                    .Include("Album")
                                    .Where(e => e.Searchname.ToUpper().Trim().Contains(normalizeSearchString) 
                                            || e.Album.Searchname.ToUpper().Trim().Contains(normalizeSearchString) 
                                            || e.Interpret.Searchname.ToUpper().Trim().Contains(normalizeSearchString))
                                    .OrderBy(e => e.Interpret.Name)
                                    .ThenBy(e => e.Album.Name)
                                    .ThenBy(e => e.Name)
                                    .Select(e => new WSearchItem { Interpret = e.Interpret, Album = e.Album, Title = e });
                return result.AsParallel().ToList();
            }
        }

        /// <summary>
        /// This method searches for tablerowsof the given paameters
        /// </summary>
        /// <param name="interpret">
        /// Searchparameter for interpret
        /// </param>
        /// <param name="album">
        /// Searchparameer for album
        /// </param>
        /// <param name="title">
        /// Searchparameter for title
        /// </param>
        /// <returns></returns>
        public IEnumerable<WSearchItem> Search(string interpret, string album, string title)
        {
            if (String.IsNullOrWhiteSpace(interpret) && String.IsNullOrWhiteSpace(album) && String.IsNullOrWhiteSpace(title))
            {
                List<WSearchItem> noSearchPara = new List<WSearchItem>();
                return noSearchPara;
            }
            string interpretSearchString = CreateSearchname(interpret);
            string albumSearchString = CreateSearchname(album);
            string titleSearchString = CreateSearchname(title);

            using (var context = new MP3ManagerContext())
            {

                var result = from titleRow in context.Titles
                             join albumRow in context.Albums on titleRow.AlbumId equals albumRow.Id
                             join artistRow in context.Interprets on titleRow.InterpretId equals artistRow.Id
                             where !String.IsNullOrWhiteSpace(titleSearchString) && titleRow.Searchname.ToUpper().Trim().Contains(titleSearchString)
                                || !String.IsNullOrWhiteSpace(albumSearchString) && albumRow.Searchname.ToUpper().Trim().Contains(albumSearchString)
                                || !String.IsNullOrWhiteSpace(interpretSearchString) && artistRow.Searchname.ToUpper().Trim().Contains(interpretSearchString)
                             orderby artistRow.Name, albumRow.Name, titleRow.Name
                             select new WSearchItem { Interpret = artistRow, Album = albumRow, Title = titleRow };

                return result.AsParallel().ToList();
            }
        }

        /// <summary>
        /// This method converts a normal string to a searchstring. The searchstring is used to find word equals or nnearly an expression.
        /// </summary>
        /// <param name="normalname">The normalname will  be converted into a searchname
        /// </param>
        /// <returns>
        /// The converted normalname
        /// </returns>
        public static string CreateSearchname(string normalname)
        {
            string retvalue = normalname.ToUpper().Trim();
            retvalue = retvalue.Replace(" ", "");
            retvalue = retvalue.Replace("Ä", "AE");
            retvalue = retvalue.Replace("Ö", "OE");
            retvalue = retvalue.Replace("Ü", "UE");
            retvalue = retvalue.Replace("ß", "SS");
            retvalue = retvalue.Replace("EI", "AI");


            return retvalue;

        }


        /// <summary>
        /// Liefert die Datenbankeinträge, welche doppelte Einträge  sind
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<WDuplicateEntry> ReadMultiEntries(EDuplicateCheckPriority priority)
        {

            string query = "SELECT i.Name, a.Name, title1.Name, title1.Path, title1.Filename, title1.bitrate, title1.Length, title1.bytes, title1.ID from Title title1 "
                + "JOIN Title title2 on title1.ID != title2.ID "
                + "JOIN Interpret i on i.ID = title1.Interpret_ID "
                + "LEFT JOIN Album a on a.ID = title1.Album_ID "
                + "WHERE title1.Interpret_ID = title2.Interpret_ID "
                + "AND (title1.Album_ID = title2.Album_ID OR title1.Album_ID is null) "
                + "AND title1.Name = title2.Name "
                + "ORDER BY title1.Interpret_ID, title1.Album_ID, title1.Name ";
            if (priority == EDuplicateCheckPriority.CREATIONDATE)
            {
                query += ", title1.Editdate ";
            }
            query += ", title1.bitrate, title1.Duration, title1.bytes";

            SqlCeConnection connection = new SqlCeConnection(global::MP3ManagerBase.Properties.Settings.Default.MP3ManagerConnection);
            connection.Open();

            List<WDuplicateEntry> returnValue = new List<WDuplicateEntry>();
            try
            {
                using (SqlCeCommand cmd = new SqlCeCommand(query, connection))
                {

                    SqlCeDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        WDuplicateEntry newEntry = new WDuplicateEntry();

                        newEntry.Interpret = !reader.IsDBNull(0) ? reader.GetSqlString(0).Value : "";
                        newEntry.Album = !reader.IsDBNull(1) ? reader.GetSqlString(1).Value : "";
                        newEntry.Name = !reader.IsDBNull(2) ? reader.GetSqlString(2).Value : "";
                        newEntry.Path = !reader.IsDBNull(3) ? reader.GetSqlString(3).Value : "";
                        newEntry.Filename = !reader.IsDBNull(4) ? reader.GetSqlString(4).Value : "";
                        newEntry.Bitrate = !reader.IsDBNull(5) ? reader.GetSqlInt32(5).Value : 0;
                        newEntry.Length = !reader.IsDBNull(6) ? reader.GetSqlString(6).Value : "";
                        newEntry.Bytes = !reader.IsDBNull(7) ? reader.GetSqlInt64(7).Value : 0;
                        newEntry.TitleId = !reader.IsDBNull(8) ? reader.GetSqlInt32(8).Value : 0;
                        returnValue.Add(newEntry);
                        log.DebugFormat("DuplicatEntry: {0}", newEntry.ToString());
                    }
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return returnValue;
        }

        /// <summary>
        /// Löscht doppelte Einträge sowhol aus der Datenbank, wie auch vom Filesystem.
        /// Eine Sicherheitskopie der Dateien wird angelegt, wenn das backupDirectory angegeben wurde.
        /// Die Auswahl der Duplikate erfolgt über eine Sortierung. 
        /// Nach der Ermittlung der doppelten Einträge, bleibt der 1. Eintrag erhalten und alle anderen werden gelöscht.
        /// </summary>
        /// <param name="priority">
        /// Eine zusätzlliche Prioritätensteuerung zur uche nach doppelten Einträgen
        /// </param>
        /// <param name="backupDirectory">
        /// Der Pfad zum Backup-Directory für die gelöschten Dateien
        /// </param>
        /// <returns>
        /// Eine Liste der gelöschten Dateien
        /// </returns>
        public IEnumerable<WDuplicateEntry> DeleteEnries(EDuplicateCheckPriority priority, string backupDirectory)
        {
            IEnumerable<WDuplicateEntry> duplicateEntries = ReadMultiEntries(priority);
            WDuplicateEntry savedEntry = null;
            List<WDuplicateEntry> deletedFiles = new List<WDuplicateEntry>();
            foreach (WDuplicateEntry curEntry in duplicateEntries)
            {
                if (savedEntry != null && savedEntry.Interpret == curEntry.Interpret && savedEntry.Album == curEntry.Album
                    && savedEntry.Name == curEntry.Name)
                {
                    deletedFiles.Add(curEntry);
                    DeleteEntry(curEntry.TitleId);
                    FileMgr.Instance.CopyFile(curEntry.Path, curEntry.Filename, backupDirectory);
                    FileMgr.Instance.DeleteFile(curEntry.Path, curEntry.Filename);
                }
                else
                {
                    savedEntry = curEntry;
                }

            }

            return deletedFiles;
        }

        /// <summary>
        /// Löscht eine Title-Entität aus der Datenbank
        /// </summary>
        /// <param name="titleId">
        /// Die ID des zu löschenden Entität
        /// </param>
        private void DeleteEntry(int titleId)
        {
            log.DebugFormat("DeleteEntry: titleId={0}", titleId);
            string query = String.Format("Delete FROM title WHERE ID = {0}", titleId);

            SqlCeConnection connection = new SqlCeConnection(global::MP3ManagerBase.Properties.Settings.Default.MP3ManagerConnection);
            connection.Open();

            SqlCeTransaction transaction = connection.BeginTransaction();

            try
            {
                using (SqlCeCommand cmd = new SqlCeCommand(query, connection, transaction))
                {
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Liefert eine Liste der gespeicherten Titel
        /// </summary>
        /// <param name="reorderStatus"></param>
        /// <returns></returns>
        public IEnumerable<Title> FetchTitles(Boolean reorderStatus)
        {
            using (var context = new MP3ManagerContext())
            {

                var retValue = from title in context.Titles
                               where title.IsOrdered == reorderStatus
                               select title;
                return retValue.ToList();
            }
        }

        /// <summary>
        /// Diese Methode aktualisiert die übergebene Zeile in der Datenbank
        /// </summary>
        /// <param name="elementToUpdate"></param>
        public void Update(Title elementToUpdate)
        {
            if (elementToUpdate == null)
            {
                throw new ArgumentException("elementToUpdate == null");
            }
            log.DebugFormat("updateTitle: id={0}", elementToUpdate.Id);
            using (var context = new MP3ManagerContext())
            {
                context.Titles.Attach(elementToUpdate);
                context.SaveChanges();
            }                
        }

        /// <summary>
        /// Diese Methode aktualisiert die übergebene Zeile in der Datenbank
        /// </summary>
        /// <param name="elementToUpdate"></param>
        public void Update(Interpret elementToUpdate)
        {
            if (elementToUpdate == null)
            {
                throw new ArgumentException("elementToUpdate == null");
            }
            log.DebugFormat("updateInterpret: id={0}", elementToUpdate.Id);
            using (var context = new MP3ManagerContext())
            {
                context.Interprets.Attach(elementToUpdate);
                context.SaveChanges();
            }                

        }

        /// <summary>
        /// Diese Methode aktualisiert die übergebene Zeile in der Datenbank
        /// </summary>
        /// <param name="elementToUpdate"></param>
        public void Update(Album elementToUpdate)
        {
            if (elementToUpdate == null)
            {
                throw new ArgumentException("elementToUpdate == null");
            }
            log.DebugFormat("updateAlbum: id={0}", elementToUpdate.Id);
            using (var context = new MP3ManagerContext())
            {
                context.Albums.Attach(elementToUpdate);
                context.SaveChanges();
            }                
        }

        /// <summary>
        /// This method updates the albums which are collections of different interprets
        /// </summary>
        public void SetCollections()
        {
            using (var context = new MP3ManagerContext())
            {
                var titles = (from t1 in context.Titles
                              from t2 in context.Titles
                              where t1.AlbumId == t2.AlbumId && t1.InterpretId != t2.InterpretId && t1.IsCollection == false
                              select t1).Distinct();
                foreach (var title in titles)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Setze isCollection = true (ID =  {0})", title.Id);
                    }
                    title.IsCollection = true;
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Diese Methode löscht die Zeile des Interpreten aus dem Dataset und der Datenbank
        /// </summary>
        /// <param name="interpret">
        /// Die Zeile die gelöscht werden soll
        /// </param>
        private void DeleteInterpret(Interpret interpret)
        {
            using (var context = new MP3ManagerContext())
            {
                context.Interprets.Remove(interpret);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Diese Methode löscht die angegebene Row aus dem Dataset und aus der Datenbanktabelle
        /// </summary>
        /// <param name="album"></param>
        private void DeleteAlbum(Album album)
        {
            using (var context = new MP3ManagerContext())
            {
                context.Albums.Remove(album);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Diese Methode löscht einen Titel aus der Datenbank.Sollte kein weiterer Titel mit dem Album verknüpft sein,
        /// so wird auch das Album gelöscht.Sollte kein weiterer Titel mit den Interpret verknüpft sein, so wird auch der Interpret gelöscht. 
        /// Sollte ein gültiger Dateiname mi dem Titel verknüpft sein, so erfolgt die Löschung erst nach der Bestätigung duch den Benutzer.
        /// </summary>
        /// <param name="title"></param>
        public void DeleteTitle(Title title)
        {
            string filename = title.Path + title.Filename;
            if (File.Exists(filename))
            {
                DialogResult result = MessageBox.Show("Es ist eine gültige Datei mit diesem Eintrag verknüpft. Sind Sie sicher, dass sie diesen Eintrag und die Datei löschen wollen?", "Info", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
                //Lösche die Datei
                File.Delete(filename);
            }

            //Lösche den Eintrag in der Datenbank
            long titleId = title.Id;
            long albumId = title.AlbumId;
            long interpretId = title.InterpretId;
            using (var context = new MP3ManagerContext())
            {
                //Ist kein weitere Titel mehr zum Album zugeordnet, so lösche das Album
                int countAlbum = context.Titles.Where(e => e.AlbumId == title.AlbumId)
                                               .Where(e => e.Id != title.Id)
                                               .Count();
                if (countAlbum == 0)
                {
                    var reloadedAlbum = context.Albums
                                               .Where(e => e.Id == title.AlbumId)
                                               .FirstOrDefault();
                    if (reloadedAlbum != null)
                    {
                        context.Albums.Remove(reloadedAlbum);
                    }
                }

                //Ist kein weiterer Titel diesem Interpret zugeordnet, dann lösche den Interpreten
                int countInterpret = context.Titles.Where(e => e.InterpretId == title.InterpretId)
                                                   .Where(e => e.Id != title.Id)
                                                   .Count();
                if (countInterpret == 0)
                {
                    var reloadedInterpret = context.Interprets
                                                   .Where(e => e.Id == title.InterpretId)
                                                   .FirstOrDefault();
                    if (reloadedInterpret != null)
                    {
                        context.Interprets.Remove(reloadedInterpret);
                    }
                }

                var reloadedTitle = context.Titles
                                           .Where(e => e.Id == title.Id)
                                           .FirstOrDefault();

                if (reloadedTitle != null)
                {
                    context.Titles.Remove(reloadedTitle);
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Liefert den Titel mit der angegebenen Id
        /// </summary>
        /// <param name="id">
        /// Eindeutiger Titelschlüssel
        /// </param>
        /// <returns>
        /// Liefert den Titel mit der angegebenen Id
        /// </returns>
        public Title GetTitle(long id)
        {
            using (var context = new MP3ManagerContext())
            {
                var title = (from t in context.Titles
                             where t.Id == id
                             select t).FirstOrDefault();
                return title;
            }
        }

        /// <summary>
        /// Liefert die höchste TitleId
        /// </summary>
        /// <returns></returns>
        public int MaxTitleId()
        {
            using (var context = new MP3ManagerContext())
            {
                long maxValue = context.Titles.Max(t => t.Id);
                return maxValue <= int.MaxValue ? (int)maxValue : int.MaxValue;
            }
        }

        /// <summary>
        /// Liefert die niedrigste TitleId
        /// </summary>
        /// <returns></returns>
        public int MinTitleId()
        {
            using (var context = new MP3ManagerContext())
            {
                long minValue = context.Titles.Min(t => t.Id);
                return minValue >= int.MinValue && minValue <= int.MaxValue ? (int)minValue : int.MinValue;
            }
        }

        /// <summary>
        /// Diese Methode liefert alle Titel, welche in einem Album vorhanden sind, welches noch nicht aktualisiert ist
        /// </summary>
        /// <returns></returns>
        internal List<WArtistAlbum> ReadTitlesToDownloadInformation()
        {
            using (var context = new MP3ManagerContext())
            {
                var result = context.Titles
                                                .Include("Album")
                                                .Include("Interpret")
                                                .Where(e => e.Album.InformationStatus == InformationDownloadStatus.NotStarted)
                                                .Where(e => e.Album != null && e.Album.Name != "" && e.Album.Name != WMP3FileInfo.UNKNOWN_ALBUM)
                                                .Where(e => e.Interpret != null && e.Interpret.Name != "" && e.Interpret.Name != WMP3FileInfo.UNKNOWN_INTERPRET)
                                                .OrderBy(e => e.InterpretId)
                                                .ThenBy(e => e.AlbumId)
                                                .Select(e => new WArtistAlbum()
                                                {
                                                    AlbumId = e.AlbumId,
                                                    AlbumName = e.Album.Name,
                                                    ArtistId = e.InterpretId,
                                                    ArtistName = e.Interpret.Name
                                                })
                                                .Distinct()
                                                .ToList();
                return result;
            }
        }

        /// <summary>
        /// Speichert die Entität der MusicBrainz-Informationen
        /// </summary>
        /// <param name="albumId">
        /// Der Primärschlüssel des Albums
        /// </param>
        /// <param name="entities">
        /// Die Entitäten die in die Datenbank eingetragen werden sollen.
        /// </param>
        internal void AddMusicBrainzEntities(long albumId, List<MusicBrainzInformation> entities)
        {
            using (var context = new MP3ManagerContext())
            {
                foreach (var entity in entities)
                {
                    if (!context.MusicBrainzInformation.Any(e => e.TitleMBId == entity.TitleMBId))
                    {
                        context.MusicBrainzInformation.Add(entity);
                    }
                }

                var album = context.Albums.Where(e => e.Id == albumId).SingleOrDefault();
                if (!context.Albums.Any(e => e.Id == albumId))
                {
                    string message = string.Format("Das Album mit der Id {0} konnte nicht gefunden werden.", albumId);
                    log.Error(message);
                    throw new EntityException(message);
                }

                album.InformationStatus = InformationDownloadStatus.Done;
                context.Entry(album).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();
            }
        }
     }
}
