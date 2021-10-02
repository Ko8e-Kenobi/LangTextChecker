using System;
using System.Windows;
using System.IO;


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
            resultText.Text = Compare();
        }
        private string Compare() {
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

        private void browsePermissiveFile_Click(object sender, RoutedEventArgs e)
        {
            PermissiveFileName.Text = FileDialog("ini");
        }
    }
}
