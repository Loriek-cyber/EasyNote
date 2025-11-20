using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EasyNote.Models;
using EasyNote.Services;

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
            DocumentDAO dl = null;
            try
            {
                dl = new DocumentDAO();
                
            }
            catch (DBService.NotConnectedException)
            {
                return;
            }
            Documents = dl.GetAllDocuments();

            if(Documents == null) return;
            foreach (var doc in Documents)
            {
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
                
                button.Click += (sender, args) =>
                {
                    VisualerService.SaveDocument();
                    DocumentDAO dl = new DocumentDAO();
                    VisualerService.Now = dl.GetByPath(doc.Path);
                    VisualerService.ExecuteScriptAsync("");
                    VisualerService.RefreshContentAsync();
                };

                threev.Items.Add(button);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Document doc = new Document();
            doc.Title = "Nuovo documento";
            doc.Path = "path";
            doc.LastModified = DateTime.Now;
            if (Documents == null) Documents = new List<Document>();
            Console.WriteLine("if you are here you are fucked");
            Console.WriteLine(DBService.DbPath);
            Documents.Add(doc);
            BuildTree();
            foreach (var document in Documents)
            {
                DocumentDAO dc = new DocumentDAO();
                if ( document.Id != null)
                    dc.Update(document);
                else dc.Insert(document);
            }
        }

        private void OpenDB(object sender, RoutedEventArgs routedEventArgs)
        {
            DBService.OpenDb();
        }

        private void CreateDB(object sender, RoutedEventArgs routedEventArgs)
        {       
            DBService.NewDb();
        }
    }
}