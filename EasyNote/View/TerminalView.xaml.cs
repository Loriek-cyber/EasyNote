using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasyNote.Services;

namespace EasyNote.View

{
    public partial class TerminalView 
    {
        public enum EditorMode
        {
            Markdown,
            Latex,
            Code
        }

        private EditorMode _currentMode = EditorMode.Markdown;
        private string _selectedLanguage = "C#";
        
        // Available programming languages for code mode
        private readonly List<string> _codeLanguages =
        [
            "C#", "JavaScript", "Python", "C++", "Java",
            "XML", "HTML", "CSS", "SQL", "PowerShell",
            "PHP", "TypeScript", "JSON" // Removed Ruby
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
            TextEditor.CommandBindings.Clear();
            
            // Remove scrollbars
            TextEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            TextEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            // Populate language selector
            LanguageSelector.ItemsSource = _codeLanguages;
            LanguageSelector.SelectedIndex = 0;

            
        }

        
        

        private void OnModeChanged(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized || TextEditor == null)
                return;

            if (Equals(sender, MarkdownMode))
            {
                _currentMode = EditorMode.Markdown;
                LanguageSelector.Visibility = Visibility.Collapsed;
            }
            else if (Equals(sender, LatexMode))
            {
                _currentMode = EditorMode.Latex;
                LanguageSelector.Visibility = Visibility.Collapsed;
            }
            else if (Equals(sender, CodeMode))
            {
                _currentMode = EditorMode.Code;
                LanguageSelector.Visibility = Visibility.Visible;
            }
            
            
        }

        private void ChangeMode(EditorMode mode)
        {
            if (mode == EditorMode.Markdown)
            {
                SendData();
                MarkdownMode.IsChecked = true;
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
                e.Handled = true; // we handle all Ctrl shortcuts
                //TODO: Adding to the start a variable Key, so that i can change everything
                switch (e.Key)
                {
                    case Key.Enter:
                        SendData();
                        break;

                    case Key.B:
                        // ****  → caret nel mezzo
                        InsertText("****", 2);
                        break;

                    case Key.I:
                        // ** → caret nel mezzo
                        InsertText("**", 1);
                        break;

                    case Key.E:
                        // $$ → caret in the middle
                        InsertText("$$", 1);
                        break;
                    case Key.K:
                        
                        
                        break;
                    case Key.M:
                        ChangeMode(EditorMode.Markdown);
                        break;
                    case Key.D1:
                        InsertText("# ",2);
                        break;
                    case Key.D2:
                        InsertText("## ",3);
                        break;
                    
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                
            }

            
        }

        private void InsertText(string text, int offset)
        {
            int caret = TextEditor.CaretOffset;

            TextEditor.Document.Insert(caret, text);

            // place caret inside the markers
            TextEditor.CaretOffset = caret + offset;
        }

        
        
        
        
        // Public methods for external access
        private EditorMode CurrentMode => _currentMode;
        private string CurrentLanguage => _selectedLanguage;
        private string GetText() => TextEditor.Text;
        private void SetText(string text) => TextEditor.Text = text;
        private void Clear() => TextEditor.Clear();

        private void SendData()
        {
            _= VisualerService.UpdateContentAsync(VisualerService.ActiveView.OriginalText + " " +GetText());
            Clear();
        }


        
    }
}
