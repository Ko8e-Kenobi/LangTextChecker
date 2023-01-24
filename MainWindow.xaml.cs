using System;
using System.Windows;
using System.IO;
using System.Windows.Input;
using System.Text;
namespace LangTextChecker
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private const string PROCESSING = "Wait while processing...", READY = "Ready...";
        private void browseMessageFile_Click(object sender, RoutedEventArgs e)
        {   
            MessageFileName.Text = FileDialog("ini");
        }

        private void browseLanguageFile_Click(object sender, RoutedEventArgs e)
        {            
             LanguageFileName.Text = FileDialog("txt");            
        }
        private string FileDialog(string extension) 
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = $"{extension.ToUpper()} Files (*.{extension})|*.{extension}";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;                
                return filename;
            }
            else
            {
                return "File not found";
            }
            
        }

        private void compare_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBox.Text = PROCESSING;
            resultText.Text = CompareMessages();
            StatusTextBox.Text = READY;
        }
        private string CompareMessages() {
            FileStream messagesFs = new FileStream(MessageFileName.Text, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader messages = new StreamReader(messagesFs); // создаем «потоковый читатель» и связываем его с файловым потоком

            FileStream languageFs = new FileStream(LanguageFileName.Text, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader language = new StreamReader(languageFs); // создаем «потоковый читатель» и связываем его с файловым потоком
            string lineText = "", languageText = language.ReadToEnd(), text = "";
            int charId = 0, textCounterFun = 0;

            while (!messages.EndOfStream)
            {

                lineText = messages.ReadLine();
                charId = lineText.IndexOf("=");
                if (charId >= 0)
                {
                    lineText = lineText.Remove(0, charId + 1).Trim();
                    //MessageBox.Show(lineText);
                    charId = lineText.IndexOf(":");
                    if (charId == 0)
                    {
                        lineText = lineText.Trim();                        
                        if (!languageText.Contains(lineText))
                        {
                            textCounterFun++;
                            text = text.Insert(0, $"{lineText}\r\n");
                        }                        
                    }
                }            
            }
            textCounter.Text = textCounterFun.ToString();
            messages.Close();
            messagesFs.Close();
            language.Close();
            languageFs.Close();
            MessageBox.Show($"Found {textCounterFun.ToString()} missed texts.");
            return text;
        }

        private string ComparePermissives()
        {
            FileStream permissivesFs = new FileStream(PermissiveFileName.Text, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader permissives = new StreamReader(permissivesFs); // создаем «потоковый читатель» и связываем его с файловым потоком

            FileStream languageFs = new FileStream(LanguageFileName.Text, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader language = new StreamReader(languageFs); // создаем «потоковый читатель» и связываем его с файловым потоком
            string lineText = "", languageText = language.ReadToEnd(), text = "";
            int charId = 0, textCounterFun = 0;
            Mouse.OverrideCursor = Cursors.Wait;

            while (!permissives.EndOfStream)
            {

                lineText = permissives.ReadLine();
                if (!lineText.StartsWith("Intk00") && (lineText.StartsWith("Intk")||lineText.StartsWith("Panel")))
                {
                    charId = lineText.IndexOf("=");
                    if (charId >= 0)
                    {
                        lineText = lineText.Remove(0, charId + 1).Trim();
                        if (!languageText.Contains(lineText) && !text.Contains(lineText))
                        {
                            textCounterFun++;
                            text = text.Insert(0, $"{lineText}\r\n");
                        }                        
                    }
                }
            }
            textCounter.Text = textCounterFun.ToString();
            permissives.Close();
            permissivesFs.Close();
            language.Close();
            languageFs.Close();
            MessageBox.Show($"Found {textCounterFun.ToString()} missed texts.");
            Mouse.OverrideCursor = Cursors.Arrow;
            return text;
        }

        private void browsePermissiveFile_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBox.Text = PROCESSING;
            PermissiveFileName.Text = FileDialog("ini");
            StatusTextBox.Text = READY;
        }

        private void comparePermissives_Click(object sender, RoutedEventArgs e)
        {
            resultText.Text = ComparePermissives();
        }
        private void GenerateOPPermissivesFilesFromHMI()
        {
            FileStream permissivesFs = new FileStream(PermissiveFileName.Text, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader permissives = new StreamReader(permissivesFs); // создаем «потоковый читатель» и связываем его с файловым потоком

            
            string lineText = "", languageText = "";
            int charId = 0, textCounterFun = 0;
            Mouse.OverrideCursor = Cursors.Wait;

            while (!permissives.EndOfStream)
            {
                lineText = permissives.ReadLine();
                if (lineText.StartsWith("[") && !lineText.Contains("[0ST_IntkPerm_Empty]"))
                {
                    var tagName = lineText.Substring(1, (int)(lineText.Length - 2));
                    FileStream permissiveOPfs = File.Create($@"{Directory.GetCurrentDirectory()}\OPfiles\{tagName}.intk");
                    StreamWriter permissivesOPWr = new StreamWriter(permissiveOPfs, Encoding.Unicode);

                    permissivesOPWr.WriteLine($"{lineText}");
                    do
                    {
                        lineText = permissives.ReadLine();
                        if (!lineText.StartsWith("Intk00") && (lineText.StartsWith("Intk") || lineText.StartsWith("Panel")))
                        {
                            charId = lineText.IndexOf("=");
                            if (charId >= 0) 
                            {
                                Encoding enc = Encoding.GetEncoding(1251);
                                FileStream languageFs = new FileStream(LanguageFileName.Text, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
                                StreamReader language = new StreamReader(languageFs, enc); // создаем «потоковый читатель» и связываем его с файловым потоком
                                var filteredText = lineText.Remove(0, charId + 1).Trim();
                                var headerText = lineText.Remove(charId + 1, lineText.Length - charId - 1).Trim();
                                if (filteredText == "Up position delay NOT expired")
                                {
                                    var sdkgjh = 0;
                                }
                                while (!language.EndOfStream && filteredText != "")
                                {
                                    languageText = language.ReadLine();
                                    if (languageText.Contains(filteredText))
                                    {
                                        charId = languageText.IndexOf("=");
                                        var translatedText = languageText.Remove(0, charId + 1).Trim();
                                        lineText = $"{headerText}{translatedText}";
                                        break;
                                    }
                                }
                                language.Close();
                                languageFs.Close();
                            }
                        }
                        permissivesOPWr.WriteLine($"{lineText}");
                    }
                    while (!lineText.StartsWith("Branches"));
                    permissivesOPWr.Close();
                    permissiveOPfs.Close();
                }
            }

            permissivesFs.Close();
            permissives.Close();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void SplitToOPIntk_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBox.Text = PROCESSING;
            GenerateOPPermissivesFilesFromHMI();
            StatusTextBox.Text = READY;
        }
    }
}
