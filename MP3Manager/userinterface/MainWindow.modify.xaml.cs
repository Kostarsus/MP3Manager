using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MP3ManagerBase.manager;
using System.Windows;
using MP3ManagerBase.helpers;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO;
using MP3ManagerBase.factory;
namespace MP3ManagerBase
{
    public partial class MainWindow
    {
        private class SearchResult
        {
            public string Interpret { get; set; }
            public string Album { get; set; }
            public string Title { get; set; }
            public WSearchItem Item { get; set; }
        }
        private void modifySearch_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = this.Cursor = System.Windows.Input.Cursors.Wait;
            SearchInDatabase();
            this.Cursor = System.Windows.Input.Cursors.Arrow;

        }

        private void search_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                modifySearch_Click(sender, e);
            }
        }


        /// <summary>        
        /// Sucht in der Datenbank und trägt das Ergebnis in das Listview ein
        /// </summary>
        private void SearchInDatabase()
        {
            searchResultView.Items.Clear();

            //Suche die gesuchten Elemente in der Datenbank
            var searchResult = MP3DataMgr.Instance.Search(searchInterpret.Text, searchAlbum.Text, searchTitle.Text);
            if (searchResult.Count() == 0)
            {
                searchResult = MP3DataMgr.Instance.Search(searchFree.Text);
            }

            //Trage nun die Elemente in das Listview-Control ein
            foreach (var resultItem in searchResult)
            {
                searchResultView.Items.Add(new SearchResult { Interpret = resultItem.Interpret.Name, Album = resultItem.Album.Name, Title = resultItem.Title.Name, Item = resultItem });
            }
        }
    
        private void searchResultView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchResult s = (SearchResult)searchResultView.SelectedItem;
            if (s == null)
                return;
            WSearchItem item = s.Item;
            modifyInterpret.Text = item.Interpret.Name;
            modifyAlbum.Text = item.Album.Name;
            modifyTitle.Text = item.Title.Name;
            modifyPath.Text = item.Title.Path + item.Title.Filename;
            modifyIsCollection.IsChecked = item.Title.IsCollection;
            modifyBitrate.Text = item.Title.Bitrate.ToString();
            modifyLength.Text = item.Title.Length.ToString();
            modifyBytes.Text = item.Title.Bytes.ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrWhiteSpace(modifyPath.Text))
                return;
            string path = ExtractPath();
            string filename = ExtractFilename();
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "MP3-Dateien|*.mp3|Alle Dateien|*.*";
            dlg.InitialDirectory = path;
            dlg.FileName = filename;
            DialogResult res=dlg.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                modifyPath.Text = dlg.FileName;
            }
        }

        /// <summary>
        /// Extrahiert den Pfad aus der Textbox modifyPath
        /// </summary>
        /// <returns></returns>
        private string ExtractPath()
        {
            if (String.IsNullOrWhiteSpace(modifyPath.Text) || modifyPath.Text.IndexOf("\\") == -1)
            {
                throw new ArgumentNullException("Pfad wurde nicht richtig belegt");
            }
            return modifyPath.Text.Substring(0, modifyPath.Text.LastIndexOf("\\"));
        }

        /// <summary>
        /// Extrahiert den Datinamen aus der Textbox modifyPath
        /// </summary>
        /// <returns></returns>
        private string ExtractFilename()
        {
            if (String.IsNullOrWhiteSpace(modifyPath.Text) || modifyPath.Text.IndexOf("\\") == -1)
            {
                throw new ArgumentNullException("Pfad wurde nicht richtig belegt");
            }
            return modifyPath.Text.Substring(modifyPath.Text.LastIndexOf("\\") + 1); 
        }

        /// <summary>
        /// Aktualisiert die Informationen des InterpretRow-Objektes in der Datenbank
        /// </summary>
        /// <param name="item"></param>
        private void UpdateInterpret(WSearchItem item)
        {
            item.Interpret.Name = modifyInterpret.Text;
            MP3DataMgr.Instance.Update(item.Interpret);
        }

        /// <summary>
        /// Aktualisiert die Informationen des AlbumRow-Objektes in der Datenbank
        /// </summary>
        /// <param name="item"></param>
        private void UpdateAlbum(WSearchItem item)
        {
            item.Album.Name = modifyAlbum.Text;
            MP3DataMgr.Instance.Update(item.Album);
        }
        /// <summary>
        /// Aktualisiert die Informationen des TitleRow-Objektes in der Datenbank und im Filesystem
        /// </summary>
        /// <param name="item"></param>
        private void UpdateTitle(WSearchItem item)
        {
            item.Title.Name = modifyTitle.Text;
            item.Title.IsCollection = modifyIsCollection.IsChecked == true ? true: false;
            string path=String.Empty;
            string filename=String.Empty;
            try
            {
                path = ExtractPath();
                filename = ExtractFilename();
                if (path != item.Title.Path || filename != item.Title.Filename)
                {
                    string source = item.Title.Path + item.Title.Filename;
                    if (!File.Exists(source))
                    {
                        System.Windows.MessageBox.Show("Die Datei befindet sich nicht mehr an dem gespeicherten Ort. Das umkopieren der Datei ist nicht möglich!");
                    }
                    FileMgr.Instance.MoveFile(source, path, filename);
                }
                item.Title.Path = ExtractPath();
                item.Title.Filename = ExtractFilename();
            }
            catch (ArgumentNullException)
            {
                //Datei wird nicht nach null kopiert und auch nicht in der Datenbank geändert
                System.Windows.MessageBox.Show("Ein leerer Pfad ist nicht erlaubt. Die Änderung des Pfades wird nicht ausgeführt!");
                return;
            }
            finally
            {
                MP3DataMgr.Instance.Update(item.Title);
            }
        }
        /// <summary>
        /// Aktualisiert den MP3-Tag der Datei
        /// </summary>
        /// <param name="item"></param>
        private void UpdateMP3Tag(WSearchItem item)
        {
            string filename = item.Title.Path + item.Title.Filename;
            if (!File.Exists(filename))
            {
                return;
            }
            MP3FileHandler mp3Handler = new MP3FileHandler();
            mp3Handler.ChangeMP3Tag(filename, modifyInterpret.Text, modifyAlbum.Text, modifyTitle.Text);
        }

        private void ModifyAccept_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            SearchResult s = (SearchResult)searchResultView.SelectedItem;
            if (s == null)
            {
                this.Cursor = System.Windows.Input.Cursors.Arrow;
                return;
            }
            WSearchItem item = s.Item;
            UpdateMP3Tag(item);            
            UpdateInterpret(item);
            UpdateAlbum(item);
            UpdateTitle(item);
            this.Cursor = System.Windows.Input.Cursors.Arrow;

        }

        private void ModifyDelete_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;

            SearchResult s = (SearchResult)searchResultView.SelectedItem;
            if (s == null)
            {
                this.Cursor = System.Windows.Input.Cursors.Arrow;
                return;
            }
            MP3DataMgr.Instance.DeleteTitle(s.Item.Title);
            SearchInDatabase();

            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }



    }
}
