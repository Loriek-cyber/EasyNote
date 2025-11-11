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
        public static WebView2 WebView { get; private set; }
        public static ViewDocument ActiveView { get; private set; }

        public static bool IsInitialized => WebView?.CoreWebView2 != null;

        // Call this once from the control that contains the WebView2
        public static async Task InitAsync(WebView2 webView)
        {
            if (webView is null) throw new ArgumentNullException(nameof(webView));

            WebView = webView;

            if (ActiveView == null)
            {
                ActiveView = new ViewDocument
                {
                    OriginalText = "# New Document"
                };
                ActiveView.ToHtml();
            }

            await EnsureReadyAsync();
            await NavigateToStringAsync(ActiveView.Html);
        }

        public static async Task UpdateContentAsync(string newMarkdown)
        {
            EnsureServiceCreated();

            if (ActiveView == null)
                ActiveView = new ViewDocument();

            ActiveView.OriginalText = newMarkdown ?? string.Empty;
            ActiveView.ToHtml();

            await EnsureReadyAsync();
            await NavigateToStringAsync(ActiveView.Html);
        }

        public static async Task RefreshContentAsync()
        {
            EnsureServiceCreated();

            if (ActiveView == null) return;

            ActiveView.ToHtml();
            await EnsureReadyAsync();
            await NavigateToStringAsync(ActiveView.Html);
        }

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

        private static Task NavigateToStringAsync(string html)
        {
            return OnUiAsync(() => WebView.NavigateToString(html ?? string.Empty));
        }

        private static void EnsureServiceCreated()
        {
            if (WebView == null)
                throw new InvalidOperationException("VisualerService not initialized. Call InitAsync(WebView2) first.");
        }

        // FIX: Gestione corretta dei Task async
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