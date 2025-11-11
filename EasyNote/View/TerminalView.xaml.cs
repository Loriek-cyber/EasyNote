using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EasyNote.Services;

// AvalonEdit
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace EasyNote.View
{
    public partial class TerminalView : UserControl
    {
        public enum TerminalMode { Markdown, LaTeX, Code }

        private TerminalMode _mode = TerminalMode.Markdown;
        private string _codeLanguage = "text";

        private TextBox _markdownBox;
        private TextEditor _codeBox;

        private readonly List<string> _history = new();
        private int _historyIndex = -1;

        private readonly string[] _cycleLangs = new[]
        {
            "csharp","javascript","python","xml","xaml","html","sql","java","c","vb","powershell","text"
        };

        public TerminalView()
        {
            InitializeComponent();
            SetEditor();
        }

        private void SetEditor()
        {
            if (_markdownBox != null)
                _markdownBox.PreviewKeyDown -= MarkdownBox_PreviewKeyDown;
            if (_codeBox != null)
                _codeBox.PreviewKeyDown -= CodeBox_PreviewKeyDown;

            switch (_mode)
            {
                case TerminalMode.Code:
                    _codeBox = new TextEditor
                    {
                        FontFamily = new FontFamily("Cascadia Mono, Consolas, Courier New"),
                        FontSize = 16,
                        Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                        Foreground = Brushes.Gainsboro,
                        ShowLineNumbers = true
                    };
                    _codeBox.TextArea.Caret.CaretBrush = Brushes.Khaki;
                    _codeBox.Options.EnableHyperlinks = false;
                    _codeBox.Options.EnableEmailHyperlinks = false;
                    _codeBox.Options.ConvertTabsToSpaces = true;
                    _codeBox.Options.IndentationSize = 4;
                    _codeBox.WordWrap = false;
                    SetSyntaxHighlighting(_codeLanguage);
                    _codeBox.PreviewKeyDown += CodeBox_PreviewKeyDown;
                    TerminalContent.Content = _codeBox;
                    StatusLeft.Text = $"Code • Lang: {_codeLanguage} • Ctrl+Enter send • Ctrl+Shift+Enter send selection • Ctrl+Shift+L cycle";
                    break;

                default:
                    _markdownBox = new TextBox
                    {
                        Foreground = Brushes.Gainsboro,
                        Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                        FontFamily = new FontFamily("Cascadia Mono, Consolas, Courier New"),
                        FontWeight = FontWeights.SemiBold,
                        FontSize = 16,
                        Padding = new Thickness(8),
                        BorderThickness = new Thickness(0),
                        CaretBrush = Brushes.Khaki,
                        AcceptsReturn = false, // Enter = submit
                        AcceptsTab = true,
                        TextWrapping = TextWrapping.Wrap
                    };
                    _markdownBox.PreviewKeyDown += MarkdownBox_PreviewKeyDown;
                    TerminalContent.Content = _markdownBox;
                    StatusLeft.Text = "Markdown • Enter send • Shift+Enter newline • Ctrl+B/I/K format • Ctrl+Shift+C fenced • Ctrl+Shift+V paste as code";
                    break;
            }
        }

        private void SetSyntaxHighlighting(string lang)
        {
            if (_codeBox == null) return;

            string name = LanguageToAvalonName((lang ?? "text").Trim().ToLowerInvariant());
            _codeLanguage = lang?.Trim().ToLowerInvariant() ?? "text";
            _codeBox.SyntaxHighlighting = string.IsNullOrEmpty(name)
                ? null
                : HighlightingManager.Instance.GetDefinition(name);
        }

        private static string LanguageToAvalonName(string lang)
        {
            // Map common names to AvalonEdit definition names (when available)
            return lang switch
            {
                "csharp" or "cs" => "C#",
                "xml" => "XML",
                "xaml" => "XML",
                "html" => "HTML",
                "javascript" or "js" => "JavaScript",
                "java" => "Java",
                "c" => "C",
                "cpp" or "c++" => "C++",
                "sql" => "SQL",
                "php" => "PHP",
                "tex" => "TeX",
                "vb" or "visualbasic" => "VBNET",
                // Many others can be added via custom .xshd if desired
                _ => null
            };
        }

        private void SwitchMode(string command)
        {
            var cmd = (command ?? string.Empty).Trim().ToLowerInvariant();
            if (cmd.Contains("latex"))
            {
                _mode = TerminalMode.LaTeX;
                SetEditor();
                ShowInfo("LaTeX mode.");
            }
            else if (cmd.Contains("code"))
            {
                var parts = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                _codeLanguage = parts.Length > 2 ? parts[2] : _codeLanguage;
                _mode = TerminalMode.Code;
                SetEditor();
                ShowInfo($"Code mode ({_codeLanguage}).");
            }
            else
            {
                _mode = TerminalMode.Markdown;
                SetEditor();
                ShowInfo("Markdown mode.");
            }
        }

        // ========== Markdown ==========
        private async void MarkdownBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (HandleGlobalModeShortcuts(e)) return;

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.B) { WrapSelectionMarkdown("**", "**"); e.Handled = true; return; }
                if (e.Key == Key.I) { WrapSelectionMarkdown("*", "*"); e.Handled = true; return; }
                if (e.Key == Key.K) { WrapSelectionMarkdown("`", "`"); e.Handled = true; return; }
                if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    if (Clipboard.ContainsText())
                    {
                        var clip = Clipboard.GetText();
                        InsertAtCaretMarkdown($"``````");
                        e.Handled = true; return;
                    }
                }
                if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    InsertAtCaretMarkdown($"``````");
                    _markdownBox.CaretIndex = Math.Max(0, _markdownBox.CaretIndex - 4);
                    e.Handled = true; return;
                }
            }

            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                InsertAtCaretMarkdown("\n");
                e.Handled = true; return;
            }

            if (e.Key == Key.Up && _history.Count > 0)
            {
                if (_historyIndex < 0) _historyIndex = _history.Count - 1;
                else _historyIndex = Math.Max(0, _historyIndex - 1);
                _markdownBox.Text = _history[_historyIndex];
                _markdownBox.CaretIndex = _markdownBox.Text.Length;
                e.Handled = true; return;
            }

            if (e.Key == Key.Down && _history.Count > 0)
            {
                if (_historyIndex >= 0 && _historyIndex < _history.Count - 1)
                {
                    _historyIndex++;
                    _markdownBox.Text = _history[_historyIndex];
                }
                else
                {
                    _historyIndex = -1;
                    _markdownBox.Clear();
                }
                _markdownBox.CaretIndex = _markdownBox.Text.Length;
                e.Handled = true; return;
            }

            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                string text = _markdownBox.Text.Trim();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (text.StartsWith("/mode", StringComparison.OrdinalIgnoreCase)) { SwitchMode(text); }
                    else if (text.StartsWith("/lang", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 1) { _codeLanguage = parts[1]; ShowInfo($"Code language: {_codeLanguage}"); }
                    }
                    else if (text.Equals("/clear", StringComparison.OrdinalIgnoreCase))
                    {
                        await VisualerService.UpdateContentAsync(string.Empty);
                        ShowInfo("Cleared.");
                    }
                    else
                    {
                        try
                        {
                            string current = VisualerService.ActiveView?.OriginalText ?? "";
                            if (_mode == TerminalMode.LaTeX)
                            {
                                await CompileAndUpdateLatex(text, current);
                            }
                            else
                            {
                                await VisualerService.UpdateContentAsync($"{current}\n{text}");
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowError("Error: " + ex.Message);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(text)) { _history.Add(text); _historyIndex = -1; }
                }
                _markdownBox.Clear();
                e.Handled = true;
            }
        }

        // ========== Code ==========
        private async void CodeBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (HandleGlobalModeShortcuts(e)) return;

            if (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
                               && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                CycleLanguage();
                e.Handled = true; return;
            }

            if (e.Key == Key.Enter &&
                (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                var sel = _codeBox.SelectedText?.Trim();
                if (!string.IsNullOrEmpty(sel)) { await SendCodeAsFence(sel); _codeBox.SelectedText = string.Empty; }
                e.Handled = true; return;
            }

            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                string text = _codeBox.Text?.Trim() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(text)) { await SendCodeAsFence(text); _codeBox.Text = string.Empty; }
                e.Handled = true; return;
            }
        }

        // Shared helpers

        private bool HandleGlobalModeShortcuts(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.M) { _mode = TerminalMode.Markdown; SetEditor(); ShowInfo("Markdown mode."); e.Handled = true; return true; }
                if (e.Key == Key.E) { _mode = TerminalMode.Code; SetEditor(); ShowInfo($"Code mode ({_codeLanguage})."); e.Handled = true; return true; }
                if (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Shift) == 0) { _mode = TerminalMode.LaTeX; SetEditor(); ShowInfo("LaTeX mode."); e.Handled = true; return true; }
            }
            return false;
        }

        private async Task SendCodeAsFence(string code)
        {
            try
            {
                string current = VisualerService.ActiveView?.OriginalText ?? "";
                var fenced = $"``````";
                await VisualerService.UpdateContentAsync($"{current}\n{fenced}");
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        private void InsertAtCaretMarkdown(string toInsert)
        {
            var i = _markdownBox.CaretIndex;
            _markdownBox.Text = _markdownBox.Text.Insert(i, toInsert);
            _markdownBox.CaretIndex = i + toInsert.Length;
        }

        private void WrapSelectionMarkdown(string before, string after)
        {
            int start = _markdownBox.SelectionStart;
            int len = _markdownBox.SelectionLength;
            string sel = len > 0 ? _markdownBox.SelectedText : string.Empty;

            if (len > 0)
            {
                _markdownBox.Text = _markdownBox.Text.Remove(start, len)
                                   .Insert(start, $"{before}{sel}{after}");
                _markdownBox.SelectionStart = start + before.Length;
                _markdownBox.SelectionLength = sel.Length;
            }
            else
            {
                _markdownBox.Text = _markdownBox.Text.Insert(start, before + after);
                _markdownBox.CaretIndex = start + before.Length;
            }
        }

        private void CycleLanguage()
        {
            int idx = Array.IndexOf(_cycleLangs, _codeLanguage.ToLowerInvariant());
            idx = (idx + 1) % _cycleLangs.Length;
            _codeLanguage = _cycleLangs[idx];
            SetSyntaxHighlighting(_codeLanguage);
            ShowInfo($"Code language: {_codeLanguage}");
        }

        // Minimal LaTeX: append $$...$$; to render in WPF, add WpfMath later if desired.
        private async Task CompileAndUpdateLatex(string expr, string current)
        {
            var block = $"$${expr}$$";
            await VisualerService.UpdateContentAsync($"{current}\n{block}");
        }

        // Banner feedback
        private DispatcherTimer _msgTimer;
        private void ShowInfo(string msg) => ShowBanner(msg, Colors.LimeGreen);
        private void ShowError(string msg) => ShowBanner(msg, Colors.Red);

        private void ShowBanner(string msg, Color border)
        {
            if (ErrorLayer == null || ErrorMsg == null) return;
            ErrorMsg.Text = msg;
            ErrorLayer.BorderBrush = new SolidColorBrush(border);
            ErrorLayer.Visibility = Visibility.Visible;

            _msgTimer ??= new DispatcherTimer { Interval = TimeSpan.FromSeconds(2.5) };
            _msgTimer.Tick -= HideBanner;
            _msgTimer.Tick += HideBanner;
            _msgTimer.Start();
        }

        private void HideBanner(object sender, EventArgs e)
        {
            _msgTimer.Stop();
            ErrorLayer.Visibility = Visibility.Collapsed;
            _msgTimer.Tick -= HideBanner;
        }
    }
}
