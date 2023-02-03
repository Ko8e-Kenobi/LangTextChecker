using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LangTextChecker.Models;
using System.IO;
using System.Windows.Input;
using System.Windows;
namespace LangTextChecker.ViewModels
{
    public class CheckerViewModel : INotifyPropertyChanged
    {
        private CheckerModel myCheckerModel;

        public CheckerViewModel() 
        {
            myCheckerModel = new CheckerModel()
            {
                MessageFileName = "Choose \"message.ini\" file"
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public string MessageFileName
        {
            get
            {
                return myCheckerModel.MessageFileName;
            }
            set 
            {
                myCheckerModel.MessageFileName = value;
                OnPropertyChanged("MessageFileName");
            }
        }

        private delegate string CompareFunction(StreamReader operationSr, string languageText, out string counter);

        private string Compare(string operationFileName, string languageFileName, out string counter, CompareFunction Compare)
        {
            FileStream operationFs = new FileStream(operationFileName, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader operationSr = new StreamReader(operationFs); // создаем «потоковый читатель» и связываем его с файловым потоком

            FileStream languageFs = new FileStream(languageFileName, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader language = new StreamReader(languageFs); // создаем «потоковый читатель» и связываем его с файловым потоком
            string languageText = language.ReadToEnd(), text = "";

            Mouse.OverrideCursor = Cursors.Wait;

            text = Compare(operationSr, languageText, out counter);

            operationSr.Close();
            operationFs.Close();
            language.Close();
            languageFs.Close();
            MessageBox.Show($"Found {counter} missed texts.");
            Mouse.OverrideCursor = Cursors.Arrow;
            return text;
        }

        private string CompareMessagesOperation(StreamReader operationSr, string languageText, out string counter)
        {
            string lineText, text = "";
            int charId, textCounterFun = 0;
            while (!operationSr.EndOfStream)
            {
                lineText = operationSr.ReadLine();
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
            counter = textCounterFun.ToString();
            return text;
        }

        private string ComparePermissivesOperation(StreamReader operationSr, string languageText, out string counter)
        {
            string lineText, text = "";
            int charId, textCounterFun = 0;
            while (!operationSr.EndOfStream)
            {

                lineText = operationSr.ReadLine();
                if (!lineText.StartsWith("Intk00") && (lineText.StartsWith("Intk") || lineText.StartsWith("Panel")))
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
            counter = textCounterFun.ToString();
            return text;
        }

        public string CompareMessages(string messageFileName, string languageFileName, out string counter)
        {
            return Compare(messageFileName, languageFileName, out counter, CompareMessagesOperation);
        }

        public string ComparePermissives(string permissiveFileName, string languageFileName, out string counter)
        {
            return Compare(permissiveFileName, languageFileName, out counter, ComparePermissivesOperation);
        }

        public void GenerateOPPermissivesFilesFromHMI(string permissiveFileName, string languageFileName)
        {
            FileStream permissivesFs = new FileStream(permissiveFileName, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
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
                                FileStream languageFs = new FileStream(languageFileName, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
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
    }
}
