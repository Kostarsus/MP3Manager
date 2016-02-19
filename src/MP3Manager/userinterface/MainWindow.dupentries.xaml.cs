using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MP3ManagerBase.manager;
using System.Windows;
using System.Windows.Forms;
using MP3ManagerBase.helpers;

namespace MP3ManagerBase
{
    public partial class MainWindow
    {
        // Declare the delegate (if using non-generic pattern).
        public delegate void ExecuteReadyHandler(object sender, List<WListPlayItem> e);

        // Declare the event.
        public event ExecuteReadyHandler ExecuteReady;



        public EDuplicateCheckPriority AdditionalPriority
        {

            get
            {
                
                if (creationDatePriority.IsChecked == true)
                    return EDuplicateCheckPriority.CREATIONDATE;
                return EDuplicateCheckPriority.NONE;
            }
        }

        /// <summary>
        /// Gibt das Sicherungsverzeichnis zurück oder legt es fest
        /// </summary>
        public string BackupDirectory 
        {
            get
            {
                return backupDirectory.Text;
            }
            set
            {
                backupDirectory.Text = value;
            }
        }
        /// <summary>
        /// Gibt zurück ob eine Sicherheitskoppie gemacht werden soll, oder nicht
        /// </summary>
        public bool MakeBackup
        {
            get
            {
                return makeBackup.IsChecked == true;
            }
            set
            {
                if (value == true)
                {
                    dirSelect.IsEnabled = true;
                }
                makeBackup.IsChecked = value;
            }
        }

        private void searchDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            backupDirectory.Text = folderDialog.SelectedPath;

        }


        private void makeBackup_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)makeBackup.IsChecked)
            {
                dirSelect.IsEnabled = true;
            }
            else
            {
                dirSelect.IsEnabled = false;
            }

        }

        private void processDuplicateEntries_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            EDuplicateCheckPriority checkPriority = AdditionalPriority;
            String backupDir=null;
            if (MakeBackup)
            {
                backupDir = BackupDirectory;
            }

            var mp3Mgr = MP3DataMgr.Instance;
            IEnumerable<WDuplicateEntry> duplicatedEnties = null;
            try
            {
                duplicatedEnties = mp3Mgr.DeleteEnries(checkPriority, backupDir);
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                System.Windows.MessageBox.Show(ex.Message);
                this.Cursor = System.Windows.Input.Cursors.Arrow;
                return;
            }

            var reorganize = from entry in duplicatedEnties
                             select new WListPlayItem
                             {
                                 Album = entry.Album,
                                 Interpret = entry.Interpret,
                                 Filename = entry.Filename,
                                 Id = entry.TitleId,
                                 Path = entry.Path,
                                 Title = entry.Name
                             };
            ExecuteReady(this, reorganize.ToList());
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }


    }
}
