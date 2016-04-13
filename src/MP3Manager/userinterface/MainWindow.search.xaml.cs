using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MP3ManagerBase.manager;
using System.Windows;
using MP3ManagerBase.helpers;
using MP3ManagerBase.userinterface;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
namespace MP3ManagerBase
{
    public partial class MainWindow
    {
        private void SearchStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                playSearchResultView.Items.Clear();
                var searchresult = MP3DataMgr.Instance.Search(SearchBox.Text) as IEnumerable<WSearchItem>;
                //Trage nun die Elemente in das Listview-Control ein
                foreach (var resultItem in searchresult)
                {
                    playSearchResultView.Items.Add(new SearchResult { Interpret = resultItem.Interpret.Name, Album = resultItem.Album.Name, Title = resultItem.Title.Name, Item = resultItem });
                }

            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SearchStart_Click(sender, e);
            }
        }


        private void playSearchResultView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchResult s = (SearchResult)playSearchResultView.SelectedItem;
            if (s == null)
            {
                return;
            }

            string fullFilename = s.Item.Title.Path + s.Item.Title.Filename;
            if (!File.Exists(fullFilename))
            {

               MessageBox.Show("Kann die Datei " + fullFilename + " nicht finden");
               MP3DataMgr.Instance.DeleteTitle(s.Item.Title);
            }


        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Source = null;


        }

        private void PreviousTitle_Click(object sender, RoutedEventArgs e)
        {
            if (playSearchResultView.SelectedIndex > 0)
            {
                playSearchResultView.SelectedIndex--;
                Play_Click(sender, e);
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            SearchResult s = (SearchResult)playSearchResultView.SelectedItem;
            if (s == null)
            {
                return;
            }

            string fullFilename = s.Item.Title.Path + s.Item.Title.Filename;
            this.mediaElement.Source = new Uri(fullFilename);
            this.mediaElement.Play();
        }

        private void NextTitle_Click(object sender, RoutedEventArgs e)
        {
            if (playSearchResultView.SelectedIndex < playSearchResultView.Items.Count)
            {
                playSearchResultView.SelectedIndex++;
                Play_Click(sender, e);
            }
        }

        private void MusicBrainzRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
