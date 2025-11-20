using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EasyNote.Models;
using EasyNote.Services;
using Microsoft.Win32;

namespace EasyNote.View
{
    public partial class SidebarView : UserControl
    {
        public List<Document> Documents { get; private set; }

        public SidebarView()
        {
            InitializeComponent();
            BuildTree();
        }

        private void BuildTree()
        {
            if (App.dbs.Count != 0)
            {
                DocumentDAO dao = new DocumentDAO();
                Documents = dao.GetAllDocuments();
                foreach (var document in Documents)
                {
                    var button = new Button
                    {
                        Content = document.Title,
                        Margin = new Thickness(2),
                        Background = Brushes.Transparent,
                        Foreground = Brushes.White,
                        FontSize = 18, 
                        BorderBrush = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                    };
                    button.Click += (sender, args) =>
                    {
                      VisualerService.Now = document;
                      VisualerService.UpdateContentAsync();
                    };
                    
                    threev.Items.Add(button);
                }
            }
        }

        public void CreateDocument(object sender, RoutedEventArgs routedEventArgs)
        {
            if (App.dbs.Count != 0)
            {
                
                Document doc = new Document();
                doc.Title = InputDialog.Show("Insert the new document title:");;
                doc.Content = "";
                doc.Path =doc.Title + ".md" ;;
                doc.LastModified = DateTime.Now;
                DocumentDAO dao = null;
                try
                {
                    dao = new DocumentDAO();
                }
                catch (Exception e)
                {
                    Console.WriteLine("C'e' un problema con la connessione al database:");
                }
                dao?.Insert(doc);
                Documents.Add(doc);
            }
            else
            {
                Console.WriteLine("Database non esistente\n");
            }
            BuildTree();
        }

        private void OpenDb(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            bool? result = opd.ShowDialog();
            if (result != true) return;
            App.dbs.Add(opd.FileName);
            BuildTree();
        }
        
        private void NewDb(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFolderDialog();

            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                // FolderName = path completo della cartella scelta
                string folderPath = ofd.FolderName;
                if (string.IsNullOrWhiteSpace(folderPath))
                    return;

                string dbPath = Path.Combine(folderPath, "EasyNote.db");

                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                }

                App.dbs.Add(dbPath);
            }
            BuildTree();
        }

        
    }
    
    
}


//Bottone -> 

/*
 var button = new Button
                {
                    Content = doc.Title,
                    Margin = new Thickness(2),
                    Background = Brushes.Transparent,
                    Foreground = Brushes.White,
                    FontSize = 18, 
                    BorderBrush = Brushes.Transparent,
                    BorderThickness = new Thickness(0)
                };

 */