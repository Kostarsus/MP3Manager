using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MP3ManagerBase.manager;
using MP3ManagerBase.model;
using System.Windows.Forms;

namespace MP3ManagerBase
{
    public partial class MainWindow
    {
        private const long GigaByte = 1073741824;
        private const long MinSize = 4 * 1048576;
        private const int CollectionMaxTries = 10;

        private void collectionDestinationDirSelection_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            collectionDestinationDir.Text = folderDialog.SelectedPath;

        }


        private void CreateCollection_Click(object sender, RoutedEventArgs e)
        {
            if (collectionSize.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Bitte geben Sie eine Größe der Collection an!");
                return;
            }
            if (string.IsNullOrWhiteSpace(collectionDestinationDir.Text))
            {
                System.Windows.MessageBox.Show("Bitte geben Sie ein Zielverzeichnis an!");
                return;
            }
            this.Cursor = System.Windows.Input.Cursors.Wait;
            ComboBoxItem selectedItem = collectionSize.SelectedItem as ComboBoxItem;
            long sizeInGigabyte = Int64.Parse(selectedItem.Tag.ToString()) * GigaByte;
            long? sizeLeft = sizeInGigabyte;
            MP3DataMgr mp3Mgr=MP3DataMgr.Instance;

            int maxTitleId = mp3Mgr.MaxTitleId();
            int minTitleId = mp3Mgr.MinTitleId();
            long currentTry = 0;
            List<int> selectedTitles = new List<int>();
            Random rnd = new Random();
            while (sizeLeft > MinSize && currentTry < CollectionMaxTries)
            {
                int newTitleId=rnd.Next(minTitleId, maxTitleId);
                if (selectedTitles.Contains(newTitleId))
                {
                    //Titel schon vorhanden
                    currentTry++;
                    Console.WriteLine("currentTry++" + currentTry);
                    continue;
                }

                Title newTitleRow = mp3Mgr.GetTitle(newTitleId);
                if (newTitleRow == null)
                {
                    // Hier wird der Try nicht erhöht, weil eine Leerstelle (wahrscheinlich gelöschte Id) getroffen wurde
                    continue;
                }
                if (sizeLeft - newTitleRow.Bytes <= 0)
                {
                    currentTry++;
                    Console.WriteLine("currentTry++" + currentTry);
                    continue;
                }

                //Kopiere nun den Titel ins Zielverzeichnis
                currentTry = 1;
                selectedTitles.Add(newTitleId);
                sizeLeft -= newTitleRow.Bytes;
                FileMgr.Instance.CopyFile(newTitleRow.Path, newTitleRow.Filename, collectionDestinationDir.Text);
                Console.WriteLine("currentTry" + currentTry + "; sizeLeft= " + sizeLeft);
            }
            this.Cursor = System.Windows.Input.Cursors.Arrow;

        }

    }
}
