using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MP3ManagerBase.factory;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using MP3ManagerBase.helpers;
using System.Windows.Forms;
using MP3ManagerBase.manager.Export;

namespace MP3ManagerBase.manager
{
    /// <summary>
    /// This class implements methods to interact with the physicaly filesystem
    /// </summary>
    public sealed class FileMgr
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly static  FileMgr _instance = new FileMgr();

        private FileMgr()
        {
        }

        public static FileMgr Instance { get { return _instance; } }
        private static object fileAddLock = new Object();
        /// <summary>
        /// This method reads the files of a directory and add the informations in a list of WFileInfo-Objectes
        /// </summary>
        /// <param name="path">
        /// The path to start reading
        /// </param>
        /// <param name="recursive">
        /// If this parameter is true, the subdirectories are read, too
        /// </param>
        /// <returns>
        /// A list of WFileInfo-Objectes.The wrappers contains all releveant informations of the files in the directory 
        /// </returns>
        /// 
        private List<WFileInfo> readDirectory(String path, bool? recursive)
        {
            if (String.IsNullOrWhiteSpace(path))
                return new List<WFileInfo>();
            if (!path.EndsWith("\\"))
                path += "\\";
            List<WFileInfo> retValue = new List<WFileInfo>();
            var filenames = Directory.EnumerateFiles(path, "*.mp3").ToArray();
            if (filenames.Length > 0)
            {
                var rangePartitioner = Partitioner.Create(0, filenames.Length);
                Parallel.ForEach(rangePartitioner, (range, loopstate) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        string filename = filenames[i];
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Read Directory: " + path + filename);
                        }
                        string normalizedFilename = NormalizeFilename(filename);
                        WFileInfo newElement = new WFileInfo { Path = path, Filename = normalizedFilename };

                        FileInfo fileInfo = new FileInfo(filename);
                        newElement.Bytes = Int64.Parse(fileInfo.Length.ToString());
                        newElement.EditDate = fileInfo.LastWriteTime;
                        lock (fileAddLock)
                        {
                            retValue.Add(newElement);
                        }
                    }
                });
            }
            if (recursive.GetValueOrDefault(false))
            {
                var directories = Directory.EnumerateDirectories(path);
                foreach (var directory in directories)
                {
                    retValue.AddRange(readDirectory(directory, recursive));
                }
            }
            return retValue;

        }

        /// <summary>
        /// This method extract the real filename
        /// </summary>
        /// <param name="filename">
        /// The fllpath filename
        /// </param>
        /// <returns>
        /// Returns the filename without path
        /// </returns>
        public string NormalizeFilename(string filename)
        {
            if (String.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");
            if (filename.IndexOf('\\') == -1)
                return filename.Trim();
            string[] filenameSegments = filename.Split('\\');
            return filenameSegments[filenameSegments.Length - 1].Trim();
        }

        /// <summary>
        /// This method read all files an return them in wrappers. Every single wrapper contains alll relevant file information.
        /// </summary>
        /// <param name="rootPath">
        /// The path where the read starts
        /// </param>
        /// <param name="recursive">
        /// If this parameter is true, all subdirectories are read, too
        /// </param>
        /// <returns>
        /// A list of file information wrappers in the directory
        /// </returns>
        public IEnumerable<WFileInfo> readFiles(String rootPath, bool? recursive)
        {
            if (String.IsNullOrWhiteSpace(rootPath))
            {
                return new List<WFileInfo>();
            }
            List<WFileInfo> retValue = readDirectory(rootPath, recursive);

            return retValue;
        }

        /// <summary>
        /// Diese Methode kopiert die angegebene Datei in das Zielverzeichnis
        /// </summary>
        /// <param name="filepath">
        /// Der Pfad zur Datei die gelöscht werden soll
        /// </param>
        /// <param name="filename">
        /// Der Dateiname der Datei die gelöscht werden soll
        /// </param>
        /// <param name="backupDirectory">
        /// Das Zielverzeichnis
        /// </param>
        public void CopyFile(string filepath, string filename, string destinationDirectory)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Copy File: Path:{0}, Name:{1}, Destination:{2}", filepath, filename, destinationDirectory);
            }
            if (String.IsNullOrWhiteSpace(destinationDirectory))
            {
                //Es wurde kein Zielverzeichnis angegeben, also ist nichts zu kopieren
                return;
            }
            if (String.IsNullOrWhiteSpace(filepath) || String.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("filepath = " + filepath + ", filename=" + filename);
            }
            
            if (!filepath.EndsWith("\\"))
            {
                filepath += "\\";
            }
            if (!destinationDirectory.EndsWith("\\"))
            {
                destinationDirectory += "\\";
            }
            string sourceFile = Path.Combine(filepath, filename);
            string destinationFile = Path.Combine(destinationDirectory, filename);
            try
            {
                if (File.Exists(destinationFile))
                {
                    destinationFile += DateTime.Now.ToString("yyyMMdd_HHmmssffffffff");
                }
                if (File.Exists(sourceFile))
                {
                    File.Copy(sourceFile, destinationFile);
                }
            }
            catch (IOException)            
            {
                throw;
            }

        }

        /// <summary>
        /// Diese Methode löscht eine Datei
        /// </summary>
        /// <param name="filepath">
        /// Der Pfad zur Datei
        /// </param>
        /// <param name="filename">
        /// Der Name der Datei
        /// </param>
        public void DeleteFile(string filepath, string filename)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("DeleteFile: filepath:{0}, filename:{1}", filepath, filename);
            }
                if (String.IsNullOrWhiteSpace(filepath) || String.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("filepath = " + filepath + ", filename=" + filename);
            }

            if (!filepath.EndsWith("\\"))
            {
                filepath += "\\";
            }
            string completeFilename = filepath + filename;
            try
            {
                if (File.Exists(completeFilename))
                {
                    File.Delete(completeFilename);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Diese Methode verschiebt eine Datei
        /// </summary>
        /// <param name="sourceFile">
        /// Die Dateiquelle
        /// </param>
        /// <param name="destPath">
        /// Der Zielpfad
        /// </param>
        /// <param name="destFilename">
        /// Der Zieldateiname
        /// </param>
        public Dictionary<string, string> MoveFile(string sourceFile, string destPath,  string destFilename)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Move File: sourcefile:{0}, destPath:{1}, destfile:{2}", sourceFile, destPath, destFilename);
            }
            Dictionary<string, string> retValue = new Dictionary<string, string>();
            if (String.IsNullOrWhiteSpace(sourceFile))
            {
                throw new ArgumentException("Quelldatei fehlt!");
            }
            if (String.IsNullOrWhiteSpace(destPath))
            {
                throw new ArgumentException("Zielverzeichnis fehlt!");
            }
            if (String.IsNullOrWhiteSpace(destFilename))
            {
                throw new ArgumentException("Destfilename fehlt!");
            }

            if (!File.Exists(sourceFile))
            {
                throw new Exception("SourceFile : " + sourceFile + " existiert nicht!");
            }

            if (!Directory.Exists(destPath))
            {
                try
                {
                    Directory.CreateDirectory(destPath);
                }
                catch (Exception e)
                {
                    log.Fatal(e); 
                    retValue.Add("Path: " + destPath, e.Message);
                    return retValue;
                }
            }
            if  (!destPath.EndsWith("\\"))
            {
                destPath +=  "\\";
            }
            string destfile=destPath + destFilename;

            if (File.Exists(destFilename))
            {
                retValue.Add(destFilename, "FileExists");
            }
            else
            {
                try
                {
                    File.Move(sourceFile, destfile);
                }
                catch (Exception e)
                {
                    log.Fatal(e);
                    retValue.Add(destFilename, e.Message);
                }
            }
            return retValue;
        }

        /// <summary>
        /// Erstellt eine Datei mit den übergebenen Liedern als Textexport
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="songs"></param>
        public void TextExport(string fileName, List<WSongInformation> songs)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                MessageBox.Show(string.Format("Da Verzeichnis {0} existiert nicht.", Path.GetDirectoryName(fileName)));
                return;
            }

            Text export = new Text();
            var exportResult = export.Export(songs);
            string text = exportResult != null ? export.TextEncoding.GetString(exportResult) : string.Empty;
            File.WriteAllText(fileName, text);
        }

        /// <summary>
        /// Erstellt eine Datei mit den übergebenen Liedern als MP3U-Export
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="songs"></param>
        public void M3UExport(string fileName, List<WSongInformation> songs)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                MessageBox.Show(string.Format("Da Verzeichnis {0} existiert nicht.", Path.GetDirectoryName(fileName)));
                return;
            }

            M3U export = new M3U();
            var exportResult = export.Export(songs);
            string text = exportResult != null ? export.TextEncoding.GetString(exportResult) : string.Empty; 
            File.WriteAllText(fileName, text);
        }

        /// <summary>
        /// Erstellt eine Datei mit den übergebenen Liedern als M3UExtended-Export
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="songs"></param>
        public void M3UExtendedExport(string fileName, List<WSongInformation> songs)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                MessageBox.Show(string.Format("Da Verzeichnis {0} existiert nicht.", Path.GetDirectoryName(fileName)));
                return;
            }

            M3UExtended export = new M3UExtended();
            var exportResult = export.Export(songs);
            string text = exportResult != null ? export.TextEncoding.GetString(exportResult) : string.Empty;
            File.WriteAllText(fileName, text);
        }

    }
}
