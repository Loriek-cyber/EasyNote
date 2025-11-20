using System;
using System.DirectoryServices.ActiveDirectory;
using System.Threading.Tasks;
using System.Windows;
using EasyNote;
using EasyNote.Services;
using EasyNote.View;

namespace EasyNote
{
    public partial class App : Application
    {
        public static bool Lock = false;
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
                new DBService();
            }
            catch (DBService.NotConnectedException) // CATTURA L'ECCEZIONE
            {
                var recoveryWin = new EasyNote.View.NoDbWindow();
                recoveryWin.Show();
            }
        }

        public void AvviaMainWindow()
        {
            // Crea e mostra la finestra principale
            var main = new MainWindow();
            MainWindow = main; // Assegna la MainWindow corrente
            main.Show();
            // Reimposta la chiusura: se chiudo la Main, si chiude l'app
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}