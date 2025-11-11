using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EasyNote.Services;
using Syncfusion.Windows.Edit;

namespace EasyNote.View
{
    public partial class TerminalView : UserControl
    {
        public enum TerminalMode { Markdown, LaTeX, Code }

        private TerminalMode _mode = TerminalMode.Markdown;
        private string _codeLanguage = "text";

        private TextBox _markdownBox;
        private EditControl _codeBox;

        // History per input veloce
        private readonly List<string> _history = new();
        private int _historyIndex = -1;

        // Lingue ciclabili per Code
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
                    _codeBox = new EditControl
                    {
                        FontFamily = new FontFamily("Cascadia Mono, Consolas, Courier New"),
                        FontSize = 16,
                        ShowLineNumber = false,
                        EnableOutlining = true
                    };
                    SetSyntaxLanguage(_codeLanguage);
                    _codeBox.PreviewKeyDown += CodeBox_PreviewKeyDown;
                    TerminalContent.Content = _codeBox;
                    break;

                default:
                    _markdownBox = new TextBox
                    {
                        Foreground = Brushes.White,
                        Background = new SolidColorBrush(Color.FromRgb(34, 34, 34)),
                        FontFamily = new FontFamily("Cascadia Mono, Consolas, Courier New"),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Padding = new Thickness(3),
                        BorderThickness = new Thickness(0),
                        CaretBrush = Brushes.Yellow,
                        AcceptsReturn = false, // Enter = submit
                        AcceptsTab = true,
                        TextWrapping = TextWrapping.Wrap
                    };
                    _markdownBox.PreviewKeyDown += MarkdownBox_PreviewKeyDown;
                    TerminalContent.Content = _markdownBox;
                    break;
            }
        }

        private void SetSyntaxLanguage(string lang)
        {
            if (_codeBox == null) return;
            switch ((lang ?? "text").Trim().ToLowerInvariant())
            {
                case "csharp":
                case "cs":
                    _codeBox.DocumentLanguage = Languages.CSharp; _codeLanguage = "csharp"; break;
                case "xml":
                    _codeBox.DocumentLanguage = Languages.XML; _codeLanguage = "xml"; break;
                case "xaml":
                    _codeBox.DocumentLanguage = Languages.XAML; _codeLanguage = "xaml"; break;
                case "vb":
                case "visualbasic":
                    _codeBox.DocumentLanguage = Languages.VisualBasic; _codeLanguage = "vb"; break;
                case "javascript":
                case "js":
                    _codeBox.DocumentLanguage = Languages.JScript; _codeLanguage = "javascript"; break;
                case "sql":
                    _codeBox.DocumentLanguage = Languages.SQL; _codeLanguage = "sql"; break;
                case "html":
                    _codeBox.DocumentLanguage = Languages.HTML; _codeLanguage = "html"; break;
                case "java":
                    _codeBox.DocumentLanguage = Languages.Java; _codeLanguage = "java"; break;
                case "c":
                    _codeBox.DocumentLanguage = Languages.C; _codeLanguage = "c"; break;
                case "delphi":
                    _codeBox.DocumentLanguage = Languages.Delphi; _codeLanguage = "delphi"; break;
                case "powershell":
                case "ps":
                    _codeBox.DocumentLanguage = Languages.PowerShell; _codeLanguage = "powershell"; break;
                case "python":
                case "py":
                    _codeBox.DocumentLanguage = Languages.Text; _codeLanguage = "python"; break; // no built-in
                default:
                    _codeBox.DocumentLanguage = Languages.Text; _codeLanguage = "text"; break;
            }
        }

        private void SwitchMode(string command)
        {
            var cmd = (command ?? string.Empty).Trim().ToLowerInvariant();
            if (cmd.Contains("latex"))
            {
                _mode = TerminalMode.LaTeX;
                SetEditor();
                ShowInfo("Modalità LaTeX attivata.");
            }
            else if (cmd.Contains("code"))
            {
                var parts = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                _codeLanguage = parts.Length > 2 ? parts[2] : _codeLanguage;
                _mode = TerminalMode.Code;
                SetEditor();
                ShowInfo($"Modalità Code attivata ({_codeLanguage}).");
            }
            else
            {
                _mode = TerminalMode.Markdown;
                SetEditor();
                ShowInfo("Modalità Markdown attivata.");
            }
        }

        // ========== Markdown handlers ==========
        private async void MarkdownBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Switch mode shortcuts
            if (HandleGlobalModeShortcuts(e)) return;

            // Formatting shortcuts
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.B) { WrapSelectionMarkdown("**", "**"); e.Handled = true; return; } // bold
                if (e.Key == Key.I) { WrapSelectionMarkdown("*", "*"); e.Handled = true; return; }   // italic
                if (e.Key == Key.K) { WrapSelectionMarkdown("`", "`"); e.Handled = true; return; }   // inline code
                if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    // paste as fenced code
                    if (Clipboard.ContainsText())
                    {
                        var clip = Clipboard.GetText();
                        InsertAtCaretMarkdown($"``````");
                        e.Handled = true; return;
                    }
                }
                if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    // insert fenced code block
                    InsertAtCaretMarkdown($"``````");
                    // place caret between the fences
                    _markdownBox.CaretIndex = Math.Max(0, _markdownBox.CaretIndex - 4);
                    e.Handled = true; return;
                }
            }

            // New line in Markdown input
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                InsertAtCaretMarkdown("\n");
                e.Handled = true; return;
            }

            // History navigation
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

            // Submit on Enter (no modifiers)
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                string text = _markdownBox.Text.Trim();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    // commands
                    if (text.StartsWith("/mode", StringComparison.OrdinalIgnoreCase))
                    {
                        SwitchMode(text);
                    }
                    else if (text.StartsWith("/lang", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 1) { _codeLanguage = parts[1]; ShowInfo($"Lingua codice: {_codeLanguage}"); }
                    }
                    else if (text.Equals("/clear", StringComparison.OrdinalIgnoreCase))
                    {
                        await VisualerService.UpdateContentAsync(string.Empty);
                        ShowInfo("Documento svuotato.");
                    }
                    else
                    {
                        try
                        {
                            string currentText = VisualerService.ActiveView?.OriginalText ?? "";
                            if (_mode == TerminalMode.LaTeX)
                            {
                                await CompileAndUpdateLatex(text, currentText);
                            }
                            else
                            {
                                await VisualerService.UpdateContentAsync($"{currentText}\n{text}");
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowError("Errore: " + ex.Message);
                        }
                    }

                    // push to history
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        _history.Add(text);
                        _historyIndex = -1;
                    }
                }

                _markdownBox.Clear();
                e.Handled = true;
            }
        }

        // ========== Code handlers ==========
        private async void CodeBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Switch mode shortcuts
            if (HandleGlobalModeShortcuts(e)) return;

            // Cycle language
            if (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
                               && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                CycleLanguage();
                e.Handled = true; return;
            }

            // Send selection only
            if (e.Key == Key.Enter &&
                (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                var sel = _codeBox.SelectedText?.Trim();
                if (!string.IsNullOrEmpty(sel))
                {
                    await SendCodeAsFence(sel);
                    
                }
                e.Handled = true; return;
            }

            // Send all
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                string text = _codeBox.Text?.Trim() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    await SendCodeAsFence(text);
                    _codeBox.Text = string.Empty;
                }
                e.Handled = true; return;
            }
        }

        // Helpers

        private bool HandleGlobalModeShortcuts(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.M) { _mode = TerminalMode.Markdown; SetEditor(); ShowInfo("Modalità Markdown attivata."); e.Handled = true; return true; }
                if (e.Key == Key.E) { _mode = TerminalMode.Code; SetEditor(); ShowInfo($"Modalità Code attivata ({_codeLanguage})."); e.Handled = true; return true; }
                if (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Shift) == 0) { _mode = TerminalMode.LaTeX; SetEditor(); ShowInfo("Modalità LaTeX attivata."); e.Handled = true; return true; }
            }
            return false;
        }

        private async Task SendCodeAsFence(string code)
        {
            try
            {
                string currentText = VisualerService.ActiveView?.OriginalText ?? "";
                var fenced = $"``````";
                await VisualerService.UpdateContentAsync($"{currentText}\n{fenced}");
            }
            catch (Exception ex)
            {
                ShowError("Errore: " + ex.Message);
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
            SetSyntaxLanguage(_codeLanguage);
            ShowInfo($"Lingua codice: {_codeLanguage}");
        }

        // LaTeX: per default appende blocco $$...$$ al documento; per render WPF nativo, installa WpfMath
        private async Task CompileAndUpdateLatex(string expr, string current)
        {
            // Append as Markdown math block (viewer lato VisualerService deve supportarlo)
            var block = $"$${expr}$$";
            await VisualerService.UpdateContentAsync($"{current}\n{block}");
        }

        // UI feedback
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
