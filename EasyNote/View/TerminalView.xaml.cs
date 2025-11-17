using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasyNote.Services;
using EasyNote.Syntax;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace EasyNote.View
{
    public partial class TerminalView // Non hai specificato, ma assumo sia UserControl o Window
    {
        public enum EditorMode
        {
            Markdown,
            Latex,
            Code,
            AImode
        }

        private EditorMode _currentMode = EditorMode.Markdown;
        private string _selectedLanguage = "C#";

        // Available programming languages for code mode
        private readonly List<string> _codeLanguages =
        [
            "C#", "JavaScript", "Python", "C++", "Java",
            "XML", "HTML", "CSS", "SQL", "PowerShell",
            "PHP", "TypeScript", "JSON"
        ];

        private readonly bool _isInitialized;

        public TerminalView()
        {
            InitializeComponent();
            InitializeEditor();
            _isInitialized = true;
        }

        private void InitializeEditor()
        {
            // Set editor options for terminal-like experience
            TextEditor.Options.ShowSpaces = false;
            TextEditor.Options.ShowTabs = false;
            TextEditor.Options.ShowEndOfLine = false;
            TextEditor.Options.ConvertTabsToSpaces = true;
            TextEditor.Options.IndentationSize = 4;
            TextEditor.Options.EnableHyperlinks = false;
            TextEditor.Options.EnableEmailHyperlinks = false;
            TextEditor.CommandBindings.Clear(); // Attenzione: questo rimuove anche Ctrl+C, Ctrl+V, Undo/Redo. Sei sicuro?

            // Remove scrollbars
            TextEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            TextEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            // Populate language selector
            LanguageSelector.ItemsSource = _codeLanguages;
            LanguageSelector.SelectedIndex = 0;
            
            // Impostazioni iniziali UI
            LanguageSelector.Visibility = Visibility.Collapsed;
            TextEditor.ShowLineNumbers = false;
        }

        private void OnModeChanged(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized || TextEditor == null)
                return;

            if (Equals(sender, MarkdownMode))
            {
                _currentMode = EditorMode.Markdown;
                LanguageSelector.Visibility = Visibility.Collapsed;
                TextEditor.SyntaxHighlighting = SyntaxHelper.Markdown;
                TextEditor.ShowLineNumbers = false; // CORREZIONE: Nascondi i numeri di riga
            }
            else if (Equals(sender, LatexMode))
            {
                _currentMode = EditorMode.Latex;
                LanguageSelector.Visibility = Visibility.Collapsed;
                TextEditor.ShowLineNumbers = false; // CORREZIONE: Nascondi i numeri di riga
                TextEditor.SyntaxHighlighting = SyntaxHelper.Latex;
            }
            else if (Equals(sender, CodeMode))
            {
                _currentMode = EditorMode.Code;
                //for now this is on pause
                //LanguageSelector.Visibility = Visibility.Visible;
                TextEditor.ShowLineNumbers = true;
                TextEditor.SyntaxHighlighting = SyntaxHelper.Python;
            }
            else if (Equals(sender, EditorMode.AImode))
            {
                _currentMode = EditorMode.AImode;
                LanguageSelector.Visibility = Visibility.Collapsed;
                TextEditor.ShowLineNumbers = false;
            }
        }

        private void ChangeMode(EditorMode mode)
        {
            // Impostando IsChecked = true, si scatenerà l'evento OnModeChanged
            // che aggiornerà l'interfaccia (numeri di riga, visibilità ComboBox)
            if (mode == EditorMode.Markdown)
            {
                MarkdownMode.IsChecked = true;
            }
            else if (mode == EditorMode.Latex)
            {
                LatexMode.IsChecked = true;
            }
            else if (mode == EditorMode.Code)
            {
                CodeMode.IsChecked = true;
                // Non serve più: TextEditor.ShowLineNumbers = true;
                // Verrà gestito dall'evento OnModeChanged
            }
        }

        private void LanguageSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageSelector.SelectedItem is string selectedLanguage)
            {
                _selectedLanguage = selectedLanguage;
            }
        }

        private void TextEditor_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true; // Gestiamo noi tutti gli shortcut Ctrl
                //TODO: Adding to the start a variable Key, so that i can change everything
                switch (e.Key)
                {
                    case Key.Enter:
                        SendData();
                        break;
                    case Key.B: // Bold
                        InsertText("****", 2);
                        break;
                    case Key.I: // Italic
                        InsertText("**", 1);
                        break;
                    case Key.E: // (E)quation?
                        InsertText("$$", 1);
                        break;
                    case Key.K: // (K)ode
                        ChangeMode(EditorMode.Code);
                        break;
                    case Key.M: // (M)arkdown
                        ChangeMode(EditorMode.Markdown);
                        break;
                    case Key.L:
                        ChangeMode(EditorMode.Latex);
                        break;
                    case Key.D1: // Header 1
                        InsertText("# ", 2);
                        break;
                    case Key.D2: // Header 2
                        InsertText("## ", 3);
                        break;
                }
            }
        }

        private void InsertText(string text, int offset)
        {
            var caret = TextEditor.CaretOffset;
            TextEditor.Document.Insert(caret, text);
            TextEditor.CaretOffset = caret + offset;
        }

        // CORREZIONE: Aggiornato il commento, questi metodi sono privati.
        // Metodi privati per l'accesso interno allo stato dell'editor
        private EditorMode CurrentMode => _currentMode;
        private string CurrentLanguage => _selectedLanguage; // Usato in SendData
        private string GetText() => TextEditor.Text;
        private void SetText(string text) => TextEditor.Text = text;
        private void Clear() => TextEditor.Clear();

        private async void SendData()
        {
            try
            {
                string original = VisualerService.ActiveView.OriginalText;
                string text = GetText();
                
                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }

                if (CurrentMode == EditorMode.Code)
                {
                    string formatted =
                        $"{original}\n```{_selectedLanguage}\n{text}\n```\n";

                    await VisualerService.UpdateContentAsync(formatted);
                }
                else
                {
                    string formatted = $"{original} {text}";
                    await VisualerService.UpdateContentAsync(formatted);
                }

                Clear(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}