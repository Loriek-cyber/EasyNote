using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using EasyNote.Models;
using EasyNote.Services;
using Microsoft.Win32;

namespace EasyNote.View
{
    public partial class SidebarView : UserControl
    {
        public ObservableCollection<Document> Documents { get; private set; }

        public event EventHandler<Document> DocumentSelected;

        public SidebarView()
        {
            InitializeComponent();
            Documents = new ObservableCollection<Document>();
            DocumentsList.ItemsSource = Documents;
            RefreshDocuments();
        }

        private void RefreshDocuments()
        {
            Documents.Clear();

            if (!HasActiveDatabase())
            {
                UpdateEmptyState(true);
                return;
            }

            try
            {
                var dao = new DocumentDAO();
                var docs = dao.GetAllDocuments();
                
                foreach (var doc in docs)
                {
                    Documents.Add(doc);
                }

                UpdateEmptyState(Documents.Count == 0);
            }
            catch (Exception ex)
            {
                ShowError("Failed to load documents", ex.Message);
                UpdateEmptyState(true);
            }
        }

        private void UpdateEmptyState(bool isEmpty)
        {
            EmptyState.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
            DocumentsList.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;
        }

        private bool HasActiveDatabase()
        {
            return App.dbs != null && App.dbs.Count > 0;
        }

        private void DocumentItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Document doc)
            {
                VisualerService.Now = doc;
                VisualerService.UpdateContentAsync();
                DocumentSelected?.Invoke(this, doc);
            }
        }

        private void CreateDocument(object sender, RoutedEventArgs e)
        {
            if (!HasActiveDatabase())
            {
                ShowWarning("No Database", "Please create or open a database first.");
                return;
            }

            string title = InputDialog.Show("Enter the new document title:");
            
            if (string.IsNullOrWhiteSpace(title))
                return;

            try
            {
                var doc = new Document
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = title.Trim(),
                    Content = "",
                    Path = SanitizeFileName(title) + ".md",
                    LastModified = DateTime.Now
                };

                var dao = new DocumentDAO();
                dao.Insert(doc);
                Documents.Add(doc);
                UpdateEmptyState(false);
            }
            catch (Exception ex)
            {
                ShowError("Failed to create document", ex.Message);
            }
        }

        private void OpenDb(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "EasyNote Database (*.db)|*.db|All files (*.*)|*.*",
                Title = "Open Database"
            };

            if (dialog.ShowDialog() != true)
                return;

            if (App.dbs.Contains(dialog.FileName))
            {
                ShowWarning("Already Open", "This database is already open.");
                return;
            }

            App.dbs.Add(dialog.FileName);
            RefreshDocuments();
        }

        private void NewDb(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select folder for new database"
            };

            if (dialog.ShowDialog() != true)
                return;

            string folderPath = dialog.FolderName;
            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            string dbPath = Path.Combine(folderPath, "EasyNote.db");

            try
            {
                if (File.Exists(dbPath))
                {
                    var result = MessageBox.Show(
                        "A database already exists at this location. Open it instead?",
                        "Database Exists",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes)
                        return;
                }
                else
                {
                    SQLiteConnection.CreateFile(dbPath);
                }

                if (!App.dbs.Contains(dbPath))
                {
                    App.dbs.Add(dbPath);
                }

                RefreshDocuments();
            }
            catch (Exception ex)
            {
                ShowError("Failed to create database", ex.Message);
            }
        }

        private string SanitizeFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (char c in invalid)
            {
                name = name.Replace(c, '_');
            }
            return name;
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowWarning(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void Refresh() => RefreshDocuments();
    }
}