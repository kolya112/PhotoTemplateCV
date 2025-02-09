using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PTComputerVisionModule;
using INIManager;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms; // Так как в WPF в .NET Framework 4.8 нет диалога для выбора директории, пришлось юзать фреймворк WinForms для этого, где этот диалог есть
using System.Drawing;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace PhotoTemplateCV
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal HubManager settings = new HubManager($"{Directory.GetCurrentDirectory()}\\settings.ini"); // Инициализация объекта для работы с файлом настроек (settings.ini)

        public MainWindow()
        {
            InitializeComponent();

            bool settingsFileHasBeenRewrite = false; // Если конфигурационный файл программы был создан/перезаписан

            if (!File.Exists($"{Directory.GetCurrentDirectory()}\\settings.ini") || !settings.KeyExists("main", "outputDirectory") || !settings.KeyExists("main", "color"))
            {
                try
                {
                    File.WriteAllText($"{Directory.GetCurrentDirectory()}\\settings.ini", $"[main]{Environment.NewLine}color=green{Environment.NewLine}outputDirectory={Directory.GetCurrentDirectory()}"); // Вносится структура
                    settingsFileHasBeenRewrite = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошло исключение при начальной обработке конфигурационного файла программы: {ex.InnerException}{Environment.NewLine}{Environment.NewLine} Текст ошибки: {ex.Message}", "Вызвано исключение",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }

            // Интро-руководство
            if (settingsFileHasBeenRewrite)
                MessageBox.Show($"Программа позволяет анализировать содержимое изображений, находить геометрические структуры и обозначать их границы линиями из трёх разных цветов: зелёным, голубым и красным. На вход получаются изображения разрешением 240x240px в формате JPG, содержащие структуры на белом фонеПрограмма разделена на две части: для входных и выходных данных.{Environment.NewLine}{Environment.NewLine}Обратите внимание, что в указанную выходную директорию будут сохраняться CSV файл с результатами (при его наличии данные новых сканирований будут добавляться) и выходные изображения (название изображения состоит из названия исходного изображения, постфикса _output и случайной комбинации цифр). По умолчанию выходная директория соответствует рабочей директории программы, изменить её можно в секции выходных данных.{Environment.NewLine}{Environment.NewLine}Авторы проекта: {Environment.NewLine} Николай Юрченко (kolya112) {Environment.NewLine} Даниил Бойков; {Environment.NewLine}{Environment.NewLine} Команда \"Кодовые пионеры\"", "Добро пожаловать в PhotoTemplateCV",
                    MessageBoxButton.OK, MessageBoxImage.Information);

            // Автозаполнение полей из настроек
            outputDirectoryTextBox.Text = settings.GetString("main", "outputDirectory");
            string color = settings.GetString("main", "color");
            Common.Global.lastColor = color;
            if (color == "green")
                greenTraceButton.IsChecked = true;
            else if (color == "blue")
                blueTraceButton.IsChecked = true;
            else if (color == "red")
                redTraceButton.IsChecked = true;
            else
            {
                MessageBox.Show("Обнаружено неизвестное значение цвета в конфигурационном файле программы, необходимо осуществить перезапись файла настроек", "Ошибка при получении цвета",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                var mainWindow = new MainWindow(true); // Инициализация объекта главной страницы программы для принудительной перезаписи конфигурационного файла
                mainWindow.Show();
                this.Close();
            }
        }

        public MainWindow(bool forceSettingsRewrite = false)
        {
            InitializeComponent();

            bool settingsFileHasBeenRewrite = false; // Если конфигурационный файл программы был создан/перезаписан

            if (forceSettingsRewrite || (!File.Exists($"{Directory.GetCurrentDirectory()}\\settings.ini") || !settings.KeyExists("main", "outputDirectory") || !settings.KeyExists("main", "color")))
            {
                try
                {
                    File.WriteAllText($"{Directory.GetCurrentDirectory()}\\settings.ini", $"[main]{Environment.NewLine}color=green{Environment.NewLine}outputDirectory={Directory.GetCurrentDirectory()}"); // Вносится структура
                    settingsFileHasBeenRewrite = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошло исключение при начальной обработке конфигурационного файла программы: {ex.InnerException}{Environment.NewLine}{Environment.NewLine} Текст ошибки: {ex.Message}", "Вызвано исключение",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }

            // Интро-руководство
            if (settingsFileHasBeenRewrite)
                MessageBox.Show($"Программа позволяет анализировать содержимое изображений, находить геометрические структуры и обозначать их границы линиями из трёх разных цветов: зелёным, голубым и красным. На вход получаются изображения разрешением 240x240px в формате JPG, содержащие структуры на белом фонеПрограмма разделена на две части: для входных и выходных данных.{Environment.NewLine}{Environment.NewLine}Обратите внимание, что в указанную выходную директорию будут сохраняться CSV файл с результатами (при его наличии данные новых сканирований будут добавляться) и выходные изображения (название изображения состоит из названия исходного изображения, постфикса _output и случайной комбинации цифр). По умолчанию выходная директория соответствует рабочей директории программы, изменить её можно в секции выходных данных.{Environment.NewLine}{Environment.NewLine}Авторы проекта: {Environment.NewLine} Николай Юрченко (kolya112) {Environment.NewLine} Даниил Бойков; {Environment.NewLine}{Environment.NewLine} Команда \"Кодовые пионеры\"", "Добро пожаловать в PhotoTemplateCV",
                    MessageBoxButton.OK, MessageBoxImage.Information);

            // Автозаполнение полей из настроек
            outputDirectoryTextBox.Text = settings.GetString("main", "outputDirectory");
            string color = settings.GetString("main", "color");
            if (color == "green")
                greenTraceButton.IsChecked = true;
            else if (color == "blue")
                blueTraceButton.IsChecked = true;
            else if (color == "red")
                redTraceButton.IsChecked = true;
            else
            {
                MessageBox.Show("Обнаружено неизвестное значение цвета в конфигурационном файле программы, необходимо осуществить перезапись файла настроек", "Ошибка при получении цвета",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                var mainWindow = new MainWindow(true); // Инициализация объекта главной страницы программы для принудительной перезаписи конфигурационного файла
                mainWindow.Show();
                this.Close();
            }

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        // Кнопка закрытия процесса программы
        private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseButton.Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200));
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseButton.Background = new SolidColorBrush(Color.FromArgb(0, 200, 200, 200));
        }

        // Кнопка сворачивания окна программы
        private void MinimizeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MinimizeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MinimizeButton.Background = new SolidColorBrush(Color.FromArgb(0, 200, 200, 200));
        }

        private void MinimizeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            MinimizeButton.Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200));
        }

        // Логотип программы
        private void AboutProgram_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"PhotoTemplateCV - проект для московской предпрофессиональной олимпиады, заключающийся в анализе входных графических изображений и последующей обработки с целью нахождения количества структур на входном изображении и их выделения. {Environment.NewLine}{Environment.NewLine} Авторы проекта: {Environment.NewLine} Николай Юрченко (kolya112) {Environment.NewLine} Даниил Бойков; {Environment.NewLine}{Environment.NewLine} Команда \"Кодовые пионеры\"", "О программе", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Кнопка открытия выходной директории
        private void OpenDirectory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Directory.Exists(outputDirectoryTextBox.Text))
                Process.Start(outputDirectoryTextBox.Text);
            else
            {
                outputDirectoryTextBox.Text = Directory.GetCurrentDirectory();
                MessageBox.Show("Указанной вами директории не существует или он недоступен программе", "Ошибка при открытии директории",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // При включении выключателя зелёного цвета
        private void GreenTraceButton_Checked(object sender, RoutedEventArgs e)
        {
            settings.WriteString("main", "color", "green");
            Common.Global.lastColor = "green";
        }

        // При включении выключателя синего цвета
        private void BlueTraceButton_Checked(object sender, RoutedEventArgs e)
        {
            settings.WriteString("main", "color", "blue");
            Common.Global.lastColor = "blue";
        }

        // При включении выключателя красного цвета
        private void RedTraceButton_Checked(object sender, RoutedEventArgs e)
        {
            settings.WriteString("main", "color", "red");
            Common.Global.lastColor = "red";
        }

        // Обзор файлов для выбора исходного изображения (фотошаблона)
        private void InputFileBrowse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var selectFile = new OpenFileDialog();
            selectFile.Title = "Выбор фотошаблона (240x240px, JPG)";
            selectFile.Filter = "Файлы JPG (*.jpg)|*.jpg";
            selectFile.DefaultExt = ".jpg";
            selectFile.CheckFileExists = true;

            if (selectFile.ShowDialog() == true)
            {
                var imageInfo = Image.FromFile(selectFile.FileName);

                if (imageInfo.Size.Width == 240 && imageInfo.Size.Height == 240)
                    inputFileTextBox.Text = selectFile.FileName;
                else
                    MessageBox.Show("Выбранное графическое изображение как фотошаблон не соответствует его требованиям: оно не имеет разрешение 240x240", "Неподдерживаемое разрешение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OutputDirectoryBrowse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var selectDirectory = new FolderBrowserDialog();
            selectDirectory.Description = "Выберите выходную директорию";
            selectDirectory.RootFolder = Environment.SpecialFolder.MyComputer;

            if (selectDirectory.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                if (Directory.Exists(selectDirectory.SelectedPath))
                    outputDirectoryTextBox.Text = selectDirectory.SelectedPath;
                else
                {
                    outputDirectoryTextBox.Text = Directory.GetCurrentDirectory();
                    MessageBox.Show("Указанной директории не существует или он недоступен программе. Выходная директория установлена на рабочую директорию программы.", "Ошибка при открытии директории",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
        }

        // При изменнии текста в поле ввода пути к изображению (фотошаблону)
        private void inputFileTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string newText = inputFileTextBox.Text;

            if (File.Exists(newText))
            {
                if (Path.GetExtension(newText) == ".jpg")
                {
                    var imageInfo = Image.FromFile(newText);

                    if (imageInfo.Size.Width == 240 && imageInfo.Size.Height == 240)
                    {
                        Common.Global.inputImagePath = newText;
                        inputImage.Source = new BitmapImage(new Uri(newText));
                    }
                    else
                        MessageBox.Show("Выбранное графическое изображение как фотошаблон не соответствует его требованиям: оно не имеет разрешение 240x240", "Неподдерживаемое разрешение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Кнопка для открытия входного изображения (фотошаблона)
        private void OpenInputImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string imagePath = Common.Global.inputImagePath;

            if (!string.IsNullOrEmpty(imagePath))
                if (File.Exists(imagePath))
                    Process.Start(imagePath);
                else
                {
                    Common.Global.inputImagePath = "";
                    inputFileTextBox.Text = "";
                    inputImage.Source = new BitmapImage(new Uri("Resources\\fileNotDone.png", UriKind.Relative));
                    MessageBox.Show("Указанного входного изображения (фотошаблона) не существует или он недоступен программе.", "Ошибка при открытии изображения",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }

        }

        // Кнопка для открытия выходного изображения
        private void OpenOutputImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string lastOutputImagePath = Common.Global.lastOutputImagePath;

            if (!string.IsNullOrEmpty(lastOutputImagePath))
                if (File.Exists(lastOutputImagePath))
                    Process.Start(lastOutputImagePath);
                else
                {
                    Common.Global.lastOutputImagePath = "";
                    outputImage.Source = new BitmapImage(new Uri("Resources\\fileNotDone.png", UriKind.Relative));
                    MessageBox.Show("Последнего сгенерированного выходного изображения не существует или он недоступен программе.", "Ошибка при открытии изображения",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }


        }

        // Кнопка для сканирования изображения (фотошаблона)
        private void ScanButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string pathToTemplate = Common.Global.inputImagePath;
            string outputDirectory = outputDirectoryTextBox.Text;

            // Отключаем UI элементы
            ScanButton.IsEnabled = false;
            inputFileTextBox.IsEnabled = false;
            InputFileBrowse.IsEnabled = false;
            outputDirectoryTextBox.IsEnabled = false;
            OutputDirectoryBrowse.IsEnabled = false;

            // Проверка входного изображения (фотошаблона) на соответствие требованием

            if (string.IsNullOrEmpty(pathToTemplate))
            {
                MessageBox.Show("Не выбрано входное изображение (фотошаблон). Воспользуйтесь полем ввода или кнопкой \"Обзор\" в левой части программы.", "Ошибка при сканировании",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                ScanButton.IsEnabled = true;
                inputFileTextBox.IsEnabled = true;
                InputFileBrowse.IsEnabled = true;
                outputDirectoryTextBox.IsEnabled = true;
                OutputDirectoryBrowse.IsEnabled = true;
                return;
            }

            if (!File.Exists(pathToTemplate))
            {
                Common.Global.inputImagePath = "";
                inputFileTextBox.Text = "";
                inputImage.Source = new BitmapImage(new Uri("Resources\\fileNotDone.png", UriKind.Relative));
                MessageBox.Show("Указанного входного изображения (фотошаблона) не существует или он недоступен программе.", "Ошибка при сканировании",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ScanButton.IsEnabled = true;
                inputFileTextBox.IsEnabled = true;
                InputFileBrowse.IsEnabled = true;
                outputDirectoryTextBox.IsEnabled = true;
                OutputDirectoryBrowse.IsEnabled = true;
                return;
            }

            if (Path.GetExtension(pathToTemplate) != ".jpg")
            {
                MessageBox.Show("Выбранное графическое изображение как фотошаблон не соответствует его требованиям: оно не имеет расширение JPG", "Неподдерживаемое расширение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ScanButton.IsEnabled = true;
                inputFileTextBox.IsEnabled = true;
                InputFileBrowse.IsEnabled = true;
                outputDirectoryTextBox.IsEnabled = true;
                OutputDirectoryBrowse.IsEnabled = true;
                return;
            }

            var imageInfo = Image.FromFile(pathToTemplate);

            if (imageInfo.Size.Width != 240 || imageInfo.Size.Height != 240)
            {
                MessageBox.Show("Выбранное графическое изображение как фотошаблон не соответствует его требованиям: оно не имеет разрешение 240x240", "Неподдерживаемое разрешение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ScanButton.IsEnabled = true;
                inputFileTextBox.IsEnabled = true;
                InputFileBrowse.IsEnabled = true;
                outputDirectoryTextBox.IsEnabled = true;
                OutputDirectoryBrowse.IsEnabled = true;
                return;
            }

            // Проверка выходной директории на соответствие

            if (!Directory.Exists(outputDirectory))
            {
                MessageBox.Show("Выбранной выходной директории не существует или он недоступен программе. Выходная директория установлена на рабочую директорию программы.", "Ошибка при сканировании",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                outputDirectoryTextBox.Text = Directory.GetCurrentDirectory();
                outputDirectory = Directory.GetCurrentDirectory();
            }

            MessageBox.Show("Все проверки пройдены, программа приступает к этапу сканирования. Во время сканирования возможны непродолжительные зависания графического интерфейса (UI).", "Начало сканирования",
                MessageBoxButton.OK, MessageBoxImage.Information);

            try
            {
                var core = new Core(pathToTemplate, outputDirectory, Common.Global.lastColor);

                (int structuresCount, int bordersCount, string outputImagePath) = core.Execute();

                MessageBox.Show("Сканирование успешно завершено. Информация о количестве найденных структур и количестве границ доступна в соответствующем окне. Выходное изображение сохранено и отображено в программе, информация о сканировании внесена в файл scanner_info.csv", "Сканирование завершено",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                Common.Global.lastOutputImagePath = outputImagePath;
                outputImage.Source = new BitmapImage(new Uri(outputImagePath));
                structuresCountLabel.Content = structuresCount.ToString();
                contoursCountLabel.Content = bordersCount.ToString();

                Common.Functions.WriteDataToCSV(outputDirectory + "\\scanner_info.csv", Path.GetFileName(pathToTemplate), structuresCount);

                ScanButton.IsEnabled = true;
                inputFileTextBox.IsEnabled = true;
                InputFileBrowse.IsEnabled = true;
                outputDirectoryTextBox.IsEnabled = true;
                OutputDirectoryBrowse.IsEnabled = true;

                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникло исключение: {ex}{Environment.NewLine}{Environment.NewLine}Текст ошибки: {ex.Message}", "Возникло исключение",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                ScanButton.IsEnabled = true;
                inputFileTextBox.IsEnabled = true;
                InputFileBrowse.IsEnabled = true;
                outputDirectoryTextBox.IsEnabled = true;
                OutputDirectoryBrowse.IsEnabled = true;
            }
        }

        // Кнопка для открытия CSV файла текущей выходной директории
        private void OpenCSVFileButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string outputDirectory = outputDirectoryTextBox.Text;
            if (File.Exists(outputDirectory + "\\scanner_info.csv"))
                Process.Start(outputDirectory + "\\scanner_info.csv");
            else
                MessageBox.Show("CSV-файл с результатми сканирований изображений (фотошаблонов) в выбранной выходной директории не найден. Возможно, он был удалён или после последнего сканирования производилась смена выходной директории.", "CSV-файл не найден",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
