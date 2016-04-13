using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MP3ManagerBase.userinterface;
using MP3ManagerBase.manager;
using MP3ManagerBase.helpers;
using log4net.Config;
using System.Windows.Threading;
using MP3ManagerBase.Views;
using Microsoft.Win32;
using System.Windows.Forms;

namespace MP3ManagerBase
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int BORDER = 50;
        public MainWindow()
        {
            InitializeComponent();
            InitializeEx();

        }

        private void InitializeEx()
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Debug("Start");
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            ExecuteReady += new ExecuteReadyHandler(duplicatedEntries_ExecuteReady);
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Globals.AdditionalInfomrationReadingTask == null || Globals.AdditionalInfomrationReadingTask.IsCompleted || 
                Globals.AdditionalInfomrationReadingTask.IsCanceled || Globals.AdditionalInfomrationReadingTask.IsFaulted)
            {
                var additionalInfoReader = new AlbumAdditionalReader();
                Globals.AdditionalInfomrationReadingTask = additionalInfoReader.UpdateAlbumInformation();
                if (Globals.AdditionalInfomrationReadingTask.IsFaulted)
                {
                    log.Error(Globals.AdditionalInfomrationReadingTask.Exception);
                }
            }

        }

        void duplicatedEntries_ExecuteReady(object sender, List<WListPlayItem> e)
        {
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tabSynchronize.Focus();
        }

 
        private void tabSynchronize_GotFocus(object sender, RoutedEventArgs e)
        {
            TabPanelSynchronize.Height = this.ActualHeight - BORDER;
            TabPanelSynchronize.Width = this.ActualWidth -  BORDER;

        }

        private void TabSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            TabPanelSearch.Height = this.ActualHeight - BORDER;
            TabPanelSearch.Width = this.ActualWidth - BORDER;
        }

        private void Reorder_GotFocus(object sender, RoutedEventArgs e)
        {
            TabReorder.Height = this.Height - BORDER;
            TabReorder.Width = this.Width - BORDER ;

        }

        private void CreateCollection_GotFocus(object sender, RoutedEventArgs e)
        {
            //TabCreateCollection.Height = this.Height - BORDER;
            //TabCreateCollection.Width = this.Width - BORDER;

        }

        private void DiffMusic_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void LoadMusicDifferenceView()
        {
            var musicDfferences = dataMgr.GetMusicBrainzList();
            var listToDisplay = musicDfferences.GroupBy(e => e.AlbumMBId)
                                               .Select(g => new MissingAlben()
                                                {
                                                    Album = (g.FirstOrDefault() == null || g.First().Album == null) ? "nicht geladen" : g.First().Album.Name,
                                                    AlbumMBId = g.Key,
                                                    Interpret = (g.FirstOrDefault() == null || g.First().Artist == null) ? "nicht geladen" : g.First().Artist.Name,
                                                    InterpretMBId = (g.FirstOrDefault() == null) ? null : g.First().ArtistMBId,
                                                    Title = (g.FirstOrDefault() == null) ? null : g.First().Title,
                                                    TitleMBId = (g.FirstOrDefault() == null ) ? null : g.First().TitleMBId 
                                                })
                                               .ToList();
            this.MusicBrainzRecords.ItemsSource = listToDisplay;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            LoadMusicDifferenceView();

        }

        private void DiffExport_Click(object sender, RoutedEventArgs e)
        {
            var selectedListboxItems = MusicBrainzRecords.SelectedItems;
            if (selectedListboxItems == null)
            {
                return;
            }

            Microsoft.Win32.SaveFileDialog saveFileDiag = new Microsoft.Win32.SaveFileDialog();
            saveFileDiag.AddExtension = true;
            saveFileDiag.Filter = "M3U-Dateien|*.m3u|Extended-M3U-Dateien|*.m3e|Textdateien|*.txt";
            saveFileDiag.DefaultExt = "*.m3u";
            saveFileDiag.CheckPathExists = true;
            var result = saveFileDiag.ShowDialog();

            List<WSongInformation> songsForExport = new List<WSongInformation>();
            foreach (MissingAlben item in selectedListboxItems)
            {
                var titles = dataMgr.GetTitles(item.AlbumMBId);
                foreach (var title in titles)
                {
                    WSongInformation newInformation = new WSongInformation()
                    {
                        Interpret = item.Interpret,
                        Album = item.Album,
                        Title = title.Title
                    };
                    songsForExport.Add(newInformation);
                }
            }

            switch (saveFileDiag.FilterIndex)
            {
                case 1:
                    FileMgr.Instance.M3UExport(saveFileDiag.FileName, songsForExport);
                    break;
                case 2:
                    FileMgr.Instance.M3UExtendedExport(saveFileDiag.FileName, songsForExport);
                    break;
                default:
                    FileMgr.Instance.TextExport(saveFileDiag.FileName, songsForExport);
                    break;
            }
        }
    }
}
