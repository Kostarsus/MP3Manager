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









 
 
 


    }
}
