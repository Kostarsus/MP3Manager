using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using MP3ManagerBase.manager;
using MP3ManagerBase.helpers;
using System.IO;
using System.Threading.Tasks;
using MP3ManagerBase.model;
using System.Windows.Media;

namespace MP3ManagerBase
{
    public partial class MainWindow
    {
        private List<WSynchronizeError> errors = null;
        private static long filesToInsert = 0;
        private static long filesInserted = 0;
        private MP3DataMgr dataMgr = MP3DataMgr.Instance;
        //Create a Delegate that matches 
        //the Signature of the ProgressBar's SetValue method
        private delegate void UpdateProgressBarDelegate(
                System.Windows.DependencyProperty dp, Object value);

        


 
        private void searchPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                RootDirectory.Text = folderDialog.SelectedPath;
                if (log.IsDebugEnabled)
                    log.Debug("Selected Directory: " + RootDirectory.Text);
            }
        }


        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(RootDirectory.Text))
                return;
            this.Cursor = System.Windows.Input.Cursors.Wait;
            try
            {
                var files = FileMgr.Instance.readFiles(RootDirectory.Text, Recursive.IsChecked);
                //Extract only the files, which aren't read
                files = dataMgr.ExtractDatabaseFromList(files);
                filesToInsert = files.Count();
                progress.Minimum = 0;
                progress.Maximum = filesToInsert;
                filesInserted = 0;
                dataMgr.FileInserted += new MP3DataMgr.DBEventHandler(dataMgr_FileInserted);
                this.errors = new List<WSynchronizeError>();
                ErrorView.ItemsSource = this.errors;
                 
                long start = DateTime.Now.Ticks;

                try
                {
                    this.errors.AddRange(dataMgr.InsertFilesInDatabase(files));
                }
                catch (Exception ex)
                {
                    log.Fatal(ex);
                    this.errors.Add(new WSynchronizeError {  Message = ex.Message });
                }

                long end = DateTime.Now.Ticks;
                TimeSpan dif=new TimeSpan(end - start);

                Console.WriteLine("Dauer=" + dif.TotalMilliseconds);
                //set the collection albums
                dataMgr.SetCollections();
                if (this.errors.Count > 0)
                {
                    ErrorView.ItemsSource = this.errors;
                    ErrorView.Visibility = Visibility.Visible;
                    ExportButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = System.Windows.Input.Cursors.Arrow;
            }

        }


        private void dataMgr_FileInserted(object sender,string title)
        {
            filesInserted++;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(progress.SetValue);
            Dispatcher.Invoke(updatePbDelegate,
                        System.Windows.Threading.DispatcherPriority.Background,
                        new object[] { System.Windows.Controls.ProgressBar.ValueProperty, (double)filesInserted });
           
        }

        private string readFilename()
        {
            SaveFileDialog saveFileDiag = new SaveFileDialog();
            saveFileDiag.AddExtension = true;
            saveFileDiag.Filter = "CSV-Dateien|*.csv|Alle Dateien|*.*";
            saveFileDiag.DefaultExt = "*.csv";
            saveFileDiag.CheckPathExists = true;
            DialogResult result = saveFileDiag.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return saveFileDiag.FileName;
            }
            return String.Empty;

        }
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {

            if (errors == null)
            {
                System.Windows.MessageBox.Show("Es gibt keine Daten zu exportieren");
                return;
            }
            string filename = readFilename();
            if (String.IsNullOrWhiteSpace(filename))
            {
                return;
            }
            using (TextWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("Pfad;Dateiname;Meldung");
                foreach (var error in errors)
                {
                    string line = String.Concat(error.Path, ";", error.Filename, ";", error.Message);
                    writer.WriteLine(line);
                }
                writer.Close();
            }

        }


    }

}
