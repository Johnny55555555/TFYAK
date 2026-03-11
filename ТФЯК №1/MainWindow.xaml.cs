using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ТФЯК__1
{
    public partial class MainWindow : Window
    {
        private string currentFilePath = "";
        private bool isTextChanged = false;

        public MainWindow()
        {
            InitializeComponent();

            Editor.TextChanged += Editor_TextChanged;
            ResultGrid.MouseDoubleClick += ResultGrid_MouseDoubleClick;

            this.Closing += MainWindow_Closing;
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            isTextChanged = true;
        }

        private bool CheckSaveChanges()
        {
            if (!isTextChanged && string.IsNullOrWhiteSpace(Editor.Text))
                return true;

            var result = MessageBox.Show(
                "Файл был изменён.\nСохранить изменения?",
                "Подтверждение",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Save_Click(null, null);
                return true;
            }

            if (result == MessageBoxResult.No)
                return true;

            return false;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!CheckSaveChanges())
                e.Cancel = true;
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSaveChanges())
                return;

            Editor.Clear();
            ResultGrid.ItemsSource = null;

            currentFilePath = "";
            isTextChanged = false;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSaveChanges())
                return;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                currentFilePath = dialog.FileName;
                Editor.Text = File.ReadAllText(currentFilePath);

                ResultGrid.ItemsSource = null;

                isTextChanged = false;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAs_Click(sender, e);
                return;
            }

            File.WriteAllText(currentFilePath, Editor.Text);
            isTextChanged = false;
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                currentFilePath = dialog.FileName;
                File.WriteAllText(currentFilePath, Editor.Text);
                isTextChanged = false;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSaveChanges())
                return;

            Close();
        }

        private void Undo_Click(object sender, RoutedEventArgs e) => Editor.Undo();
        private void Redo_Click(object sender, RoutedEventArgs e) => Editor.Redo();
        private void Cut_Click(object sender, RoutedEventArgs e) => Editor.Cut();
        private void Copy_Click(object sender, RoutedEventArgs e) => Editor.Copy();
        private void Paste_Click(object sender, RoutedEventArgs e) => Editor.Paste();
        private void Delete_Click(object sender, RoutedEventArgs e) => Editor.SelectedText = "";
        private void SelectAll_Click(object sender, RoutedEventArgs e) => Editor.SelectAll();

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LexicalAnalyzer scanner = new LexicalAnalyzer();
                List<Token> tokens = scanner.Analyze(Editor.Text);

                ResultGrid.ItemsSource = tokens;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка анализа:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ResultGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ResultGrid.SelectedItem is Token token)
            {
                int index = GetIndexFromLineColumn(token.Line, token.Start);

                if (index >= 0)
                {
                    Editor.Focus();
                    Editor.Select(index, token.End - token.Start + 1);
                }
            }
        }

        private int GetIndexFromLineColumn(int line, int column)
        {
            int currentLine = 1;
            int currentColumn = 1;

            for (int i = 0; i < Editor.Text.Length; i++)
            {
                if (currentLine == line && currentColumn == column)
                    return i;

                if (Editor.Text[i] == '\n')
                {
                    currentLine++;
                    currentColumn = 1;
                }
                else
                {
                    currentColumn++;
                }
            }

            return -1;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Лексический анализатор.\n\n" +
                "Программа выполняет разбор текста и выделяет лексемы:\n" +
                "- ключевые слова\n" +
                "- идентификаторы\n" +
                "- числа\n" +
                "- строки\n" +
                "- операторы и разделители.\n\n" +
                "Для запуска анализа нажмите кнопку \"Пуск\".",
                "Справка",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Компилятор (ЛР2)\n\n" +
                "Лексический анализатор\n\n" +
                "Студент: Пузырный Д.А.",
                "О программе",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
