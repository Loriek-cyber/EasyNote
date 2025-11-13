using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasyNote.Services;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using EasyNote.Syntax;
namespace EasyNote.View

{
    public partial class TerminalView : UserControl
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

        private readonly bool _isInitialized = false;

        public TerminalView()
        {
            InitializeComponent();
            InitializeEditor();
            _isInitialized = true;
        }

        private void InitializeEditor()
        {
            // Load custom syntax highlighting definitions
            HighlightingService.RegisterCustomHighlighting();
            
            // Set editor options for terminal-like experience
            TextEditor.Options.ShowSpaces = false;
            TextEditor.Options.ShowTabs = false;
            TextEditor.Options.ShowEndOfLine = false;
            TextEditor.Options.ConvertTabsToSpaces = true;
            TextEditor.Options.IndentationSize = 4;
            TextEditor.Options.EnableHyperlinks = false;
            TextEditor.Options.EnableEmailHyperlinks = false;
            
            // Remove scrollbars
            TextEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            TextEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            TextEditor.SyntaxHighlighting = HighlightingLoader.Load(SyntaxHelper.markdown.Reader, HighlightingManager.Instance);

            
            // Populate language selector
            LanguageSelector.ItemsSource = _codeLanguages;
            LanguageSelector.SelectedIndex = 0;
            
            // Set initial highlighting
            UpdateSyntaxHighlighting();
        }

        private void UpdateSyntaxHighlighting()
        {
            
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
            
            UpdateSyntaxHighlighting();
        }

        private void LanguageSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageSelector.SelectedItem is string selectedLanguage)
            {
                _selectedLanguage = selectedLanguage;
                if (_currentMode == EditorMode.Code)
                {
                    UpdateSyntaxHighlighting();
                }
            }
        }

        private void TextEditor_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                sendData();
            }
        }
        
        // Public methods for external access
        private EditorMode CurrentMode => _currentMode;
        private string CurrentLanguage => _selectedLanguage;
        private string GetText() => TextEditor.Text;
        private void SetText(string text) => TextEditor.Text = text;
        private void Clear() => TextEditor.Clear();

        private void sendData()
        {
            _= VisualerService.UpdateContentAsync(VisualerService.ActiveView.OriginalText + " " +GetText());
            Clear();
        }
    }
}
