using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using MP3ManagerBase.manager;

namespace MP3ManagerBase
{
    public partial class MainWindow
    {

        private void selectDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            targetDirectory.Text = folderDialog.SelectedPath;
            if (log.IsDebugEnabled)
                log.Debug("Selected Directory: " + folderDialog.SelectedPath);
        }

        /// <summary>
        /// Diese Methode führt die Umstrukturierung der MP3 durch
        /// </summary>
        public void DoReorder()
        {
            if (String.IsNullOrWhiteSpace(targetDirectory.Text))
                return;
            MP3DataMgr mp3Mgr = MP3DataMgr.Instance;
            var elements = mp3Mgr.FetchTitles(false);
            try
            {
                foreach (var element in elements)
                {
                    string destPath = targetDirectory.Text;
                    if (!destPath.EndsWith("\\"))
                    {
                        destPath += "\\";
                    }
                    if (saveCompilationsExtra.IsChecked == true && element.IsCollection)
                    {
                        destPath += element.Album.Name;
                    }
                    else
                    {
                        destPath += element.Interpret.Name + "\\" + element.Album.Name;
                    }
                    destPath += "\\";

                    string destFilename = element.Name + ".mp3";

                    //Aktualisiere Dateisystem
                    string sourcefile = element.Path;
                    if (!sourcefile.EndsWith("\\"))
                    {
                        sourcefile += "\\";
                    }
                    sourcefile += element.Filename;
                    Dictionary<string, string> errors = new Dictionary<string, string>();
                    try
                    {
                        FileMgr.Instance.MoveFile(sourcefile, destPath, destFilename);

                        if (changeInDataBase.IsChecked == true)
                        {
                            //Aktualisiere die Datenbank
                            element.Path = destPath;
                            element.Filename = destFilename;
                            element.IsOrdered = true;
                            mp3Mgr.Update(element);
                        }
                    }
                    catch 
                    {
                        //Hier soll nichts geschehen, weil die Schleife weiterlaufen oll. 
                        //Der Try-Catch-Block wurde eingeführt, damit die Datenbank nur aktualisiert wird, wenn 
                        //die Dateibewegung vollzogen wurde.
                    }
                }

            }
            catch (Exception e)
            {
                log.Fatal(e);
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private void Reorder_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            DoReorder();
            this.Cursor = System.Windows.Input.Cursors.Arrow;

        }


    }
}
