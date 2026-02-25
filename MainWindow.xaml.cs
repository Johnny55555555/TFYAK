using Microsoft.Win32;
using System.IO;
using System.Windows;

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
        }

        private void Editor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            isTextChanged = true;
        }

        private bool CheckSaveChanges()
        {
            if (!isTextChanged)
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
            else if (result == MessageBoxResult.No)
            {
                return true;
            }
            else
            {
                return false; // Cancel
            }
        }


        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSaveChanges())
                return;

            Editor.Clear();
            OutputBox.Clear();
            currentFilePath = "";
            isTextChanged = false;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSaveChanges())
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName;
                Editor.Text = File.ReadAllText(currentFilePath);
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;
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


        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "СПРАВКА ПО ПРОГРАММЕ \"Компилятор (GUI)\"\n\n" +

                "        МЕНЮ ФАЙЛ         \n" +
                "Создать – очищает редактор и создаёт новый документ.\n" +
                "Открыть – открывает текстовый файл (*.txt).\n" +
                "Сохранить – сохраняет текущий файл.\n" +
                "Сохранить как – сохраняет файл под новым именем.\n" +
                "Выход – закрывает программу.\n\n" +

                "        МЕНЮ ПРАВКА        \n" +
                "Отменить – отменяет последнее действие.\n" +
                "Повторить – возвращает отменённое действие.\n" +
                "Вырезать – удаляет выделенный текст и копирует его.\n" +
                "Копировать – копирует выделенный текст.\n" +
                "Вставить – вставляет текст из буфера обмена.\n" +
                "Удалить – удаляет выделенный фрагмент.\n" +
                "Выделить всё – выделяет весь текст в редакторе.\n\n" +

                "         МЕНЮ ТЕКСТ        \n" +
                "Раздел предназначен для отображения теоретической части проекта.\n\n" +

                "         ПУСК         \n" +
                "Кнопка предназначена для запуска процесса анализа текста\n" +
                "(в текущей версии функционал не реализован).\n\n" +

                "         ОБЛАСТИ ПРОГРАММЫ        \n" +
                "Верхняя область – редактор исходного текста.\n" +
                "Нижняя область – окно вывода результатов анализа.\n" +
                "Между ними расположен разделитель для изменения размеров областей.\n\n" +

                "Разработчик: Студент Пузырный Д.А.",
                "Справка",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Компилятор (GUI)\n\n" +
                "Версия 1.0\n\n" +
                "Студент Пузырный Д.А.",
                "О программе");
        }
    }
}