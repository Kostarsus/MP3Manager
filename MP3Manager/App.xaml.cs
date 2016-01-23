using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using MP3ManagerBase.manager;

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile="app.config")]
namespace MP3ManagerBase
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string dataPath=Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            AppDomain.CurrentDomain.SetData("DataDirectory",dataPath);


            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            UnhandledError window = new UnhandledError(e.Exception);
            window.ShowDialog();
            App.Current.Shutdown();
        }
    }
}
