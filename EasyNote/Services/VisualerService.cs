using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EasyNote.Models;
using Microsoft.Web.WebView2.Wpf;

namespace EasyNote.Services
{
    public static class VisualerService
    {
        private static WebView2? WebView { get; set; }
        public static Document? Now { get; set; }
        private static bool IsInitialized => WebView?.CoreWebView2 != null;

        private static DispatcherTimer? _saveTimer;
        private static readonly TimeSpan SaveDelay = TimeSpan.FromSeconds(1); // Save 1 second after the last change

        public static async Task AddToDocument(string content)
        {
            if (Now == null) return;
            Now.Content += "\n" + content;
            RequestSave(); // Request a save after modification
            await UpdateContentAsync();
        }

        public static void RequestSave()
        {
            if (_saveTimer == null) return;
            
            // a document that has no ID is a new document that hasn't been saved yet.
            // the first save is done through an explicit user action (e.g. create document button)
            if (Now?.Id == null) return;

            _saveTimer.Stop();
            _saveTimer.Start();
        }

        private static async void SaveDocumentAsync(object? sender, EventArgs e)
        {
            if (_saveTimer == null || Now == null || Now.Id == null) return;
            
            _saveTimer.Stop();

            // Make sure LastModified is updated before saving
            Now.LastModified = DateTime.Now;

            // It's important to copy the document data to a new object
            // to avoid issues with the object being modified while saving.
            var docToSave = new Document
            {
                Id = Now.Id,
                Title = Now.Title,
                Content = Now.Content,
                Path = Now.Path,
                LastModified = Now.LastModified
            };

            await Task.Run(() =>
            {
                try
                {
                    using (var dc = new DocumentDAO())
                    {
                        dc.Update(docToSave);
                    }
                }
                catch (Exception ex)
                {
                    // It would be good to have a logging mechanism here
                    Console.WriteLine($"Error saving document: {ex.Message}");
                }
            });
        }
        
        // Call this once from the control that contains the WebView2
        public static async Task InitAsync(WebView2 webView)
        {
            WebView = webView ?? throw new ArgumentNullException(nameof(webView));
            await EnsureReadyAsync();
            
            await NavigateToStringAsync(MarkdownService.RenderMarkdownLatex(Now?.Markdown()));

            // Initialize the save timer
            _saveTimer = new DispatcherTimer
            {
                Interval = SaveDelay
            };
            _saveTimer.Tick += SaveDocumentAsync;
        }

        public static async Task UpdateContentAsync()
        {
            EnsureServiceCreated();
            if(Now == null)return; //exit without doing anything
            await EnsureReadyAsync();
            await NavigateToStringAsync(MarkdownService.RenderMarkdownLatex(Now?.Markdown()));
        }
        
        // ----------------- public API ----------------- 
        
        public static async Task NavigateAsync(string url)
        {
            EnsureServiceCreated();
            await OnUiAsync(() => WebView.Source = new Uri(url));
        }
        
        public static Task ExecuteScriptAsync(string js)
        {
            EnsureServiceCreated();
            return OnUiAsync(async () =>
            {
                if (!IsInitialized) 
                    await WebView.EnsureCoreWebView2Async();
                await WebView.ExecuteScriptAsync(js ?? string.Empty);
            });
        }

        // ----------------- helpers ----------------- 

        private static async Task EnsureReadyAsync()
        {
            EnsureServiceCreated();
            
            if (WebView.Dispatcher.CheckAccess())
            {
                await WebView.EnsureCoreWebView2Async();
            }
            else
            {
                await WebView.Dispatcher.InvokeAsync(async () =>
                {
                    await WebView.EnsureCoreWebView2Async();
                }).Task.Unwrap();
            }
        }

        private static Task NavigateToStringAsync(string? html)
        {
            return OnUiAsync(() => WebView.NavigateToString(html ?? string.Empty));
        }

        private static void EnsureServiceCreated()
        {
            if (WebView == null)
                throw new InvalidOperationException("VisualerService not initialized. Call InitAsync(WebView2) first.");
        }
        
        private static async Task OnUiAsync(Func<Task> action)
        {
            if (WebView.Dispatcher.CheckAccess())
            {
                await action();
            }
            else
            {
                await WebView.Dispatcher.InvokeAsync(action).Task.Unwrap();
            }
        }

        private static Task OnUiAsync(Action action)
        {
            if (WebView.Dispatcher.CheckAccess())
            {
                action();
                return Task.CompletedTask;
            }
            else
            {
                return WebView.Dispatcher.InvokeAsync(action).Task;
            }
        }
    }
}
