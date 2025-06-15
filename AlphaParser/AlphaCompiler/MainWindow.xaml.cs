using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Antlr4.Runtime;
using AlphaCompiler.Generation;

using AlphaCompiler.Semantics;

namespace AlphaCompiler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CodeInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateLineNumbers();
        }

        private void CodeInput_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            lineNumberScrollViewer.ScrollToVerticalOffset(codeScrollViewer.VerticalOffset);
        }

        private void UpdateLineNumbers()
        {
            int lineCount = codeInput.LineCount;
            var lines = new List<string>();
            for (int i = 1; i <= lineCount; i++)
            {
                lines.Add(i.ToString());
            }
            lineNumbers.Text = string.Join("\n", lines);
        }

        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            Resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            Resources["EditorBackground"] = new SolidColorBrush(Color.FromRgb(45, 45, 45));
            Resources["TextColor"] = new SolidColorBrush(Colors.White);
            themeToggle.Content = "☀ Tema Claro";
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(243, 243, 243));
            Resources["EditorBackground"] = new SolidColorBrush(Colors.White);
            Resources["TextColor"] = new SolidColorBrush(Colors.Black);
            themeToggle.Content = "🌙 Tema Oscuro";
        }
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            var code = codeInput.Text;
            outputBox.Clear();

            try
            {
                var input = new AntlrInputStream(code);
                var lexer = new AlphaScanner(input);
                var tokens = new CommonTokenStream(lexer);
                var parser = new AlphaParser(tokens);
                var tree = parser.program();

                if (parser.NumberOfSyntaxErrors > 0)
                {
                    outputBox.Text = "❌ Error de sintaxis.";
                    return;
                }

                var checker = new SymbolTableBuilder();
                checker.Visit(tree);
                
                if (checker.Errors.Count > 0)
                {
                    outputBox.Text = "❌ Errores semánticos:\n" + string.Join("\n", checker.Errors);
                    return;
                }
                //imprimir el checker
                //Console.WriteLine($"checker: {checker} con {checker.Errors.Count} errores");
                var generator = new IRBuilder();
                generator.Visit(tree);

                var generatedCode = generator.Program.Emit();
                var result = CodeRunner.CompileAndRun(generatedCode);

                outputBox.Text = "✅ Ejecución completada:\n" + result + 
                                 "\n\n⚙️ Código generado:\n" + generatedCode;
                
                 
            }
            catch (Exception ex)
            {
                outputBox.Text = $"❌ Excepción: {ex.Message}";
            }
        }
    }
}