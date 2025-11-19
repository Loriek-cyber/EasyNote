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
        
        
        
        //code section to add to the update:
        //serve a fare un update
        public static async Task SaveDocument()
        {
            Console.WriteLine("[Save Document] Starting...\n");
            DocumentDAO dc = new DocumentDAO();
            if (Now != null) dc.Update(Now);
            Console.WriteLine("[Save Document] End...\n");
        }

        public static async Task AddToDocument(string content)
        {
            Now.Content += "\n"+content;
            await UpdateContentAsync();
        }
        
        
        
        // Call this once from the control that contains the WebView2
        public static async Task InitAsync(WebView2 webView)
        {
            WebView = webView ?? throw new ArgumentNullException(nameof(webView));
            await EnsureReadyAsync();
            
            await NavigateToStringAsync(MarkdownService.RenderMarkdownLatex(Now?.Markdown()));
            
            //going to remove this double call and integrate the document
            //await UpdateContentAsync(ActiveView.Html);
        }

        public static async Task UpdateContentAsync()
        {
            EnsureServiceCreated();

            if (Now == null)
            {
                Now = new Document();
            } ;
            
            await EnsureReadyAsync();
            await NavigateToStringAsync(MarkdownService.RenderMarkdownLatex(Now?.Markdown()));
            
            //TODO: Capire se il salvataggio va bene qui
            /*
             * DocumentDAO dc = new DocumentDAO();
             * dc.Update(Now);
             */
        }

        public static async Task RefreshContentAsync()
        {
            EnsureServiceCreated();
            if (Now == null) return;
            await EnsureReadyAsync();
            await NavigateToStringAsync(MarkdownService.RenderMarkdownLatex(Now?.Markdown()));
        }

        // ----------------- public API -----------------
        /*
         * Questi sono dei servizi deprecabili ma che non rimuovero
         */
        
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
            
            // FIX: Assicurati di essere sul thread UI
            if (WebView.Dispatcher.CheckAccess())
            {
                await WebView.EnsureCoreWebView2Async();
            }
            else
            {
                await WebView.Dispatcher.InvokeAsync(async () =>
                {
                    await WebView.EnsureCoreWebView2Async();
                }).Task.Unwrap(); // IMPORTANTE: Unwrap() per aspettare il Task interno
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