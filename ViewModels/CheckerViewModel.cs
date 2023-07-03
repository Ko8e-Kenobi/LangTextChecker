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
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        private CheckerModel myCheckerModel;
        public CheckerViewModel()
        {
            myCheckerModel = new CheckerModel()
            {
                MessageFileName = "Choose \"message.ini\" file",
                PermissiveFileName = "Choose \"permissive.ini\" file",
                LanguageFileName = "Choose \"lang****.txt\" file",
                Status = "To start: Open language file and at least one of \"permissives.ini\" or \"messages.ini\" file.",
                Counter = "..."
            };
        }

        #region Public Binding Properties
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

        public string PermissiveFileName
        {
            get
            {
                return myCheckerModel.PermissiveFileName;
            }
            set
            {
                myCheckerModel.PermissiveFileName = value;
                OnPropertyChanged("PermissiveFileName");
            }
        }

        public string LanguageFileName
        {
            get
            {
                return myCheckerModel.LanguageFileName;
            }
            set
            {
                myCheckerModel.LanguageFileName = value;
                OnPropertyChanged("LanguageFileName");
            }
        }

        public string ResultText
        {
            get
            {
                return myCheckerModel.ResultText;
            }
            set
            {
                myCheckerModel.ResultText = value;
                OnPropertyChanged("ResultText");
            }
        }

        public string FoundCounter
        {
            get
            {
                return myCheckerModel.Counter;
            }
            set
            {
                myCheckerModel.Counter = value;
                OnPropertyChanged("FoundCounter");
            }
        }

        public string Status
        {
            get
            {
                return myCheckerModel.Status;
            }
            set
            {
                myCheckerModel.Status = value;
                OnPropertyChanged("Status");
            }
        }
        #endregion

        #region OpenMessageFile Simple implementation of ICommand

        private ICommand openMessageFile;
        public ICommand OpenMessageFile
        {
            get
            {
                if (openMessageFile == null)
                    openMessageFile = new Commands.VoidCommand(_OpenMessageFile, true) { };
                return openMessageFile;
            }
            set
            {
                openMessageFile = value;
            }
        }
        private void _OpenMessageFile()
        {
            MessageFileName = CommonHandler.FileDialog("ini");
        }

        #endregion

        #region OpenPermissivesFile Simple implementation of ICommand
        private ICommand openPermissiveFile;
        public ICommand OpenPermissiveFile
        {
            get
            {
                if (openPermissiveFile == null)
                    openPermissiveFile = new Commands.VoidCommand(_OpenPermissiveFile, true) { };
                return openPermissiveFile;
            }
            set
            {
                openPermissiveFile = value;
            }
        }

        private void _OpenPermissiveFile()
        {
            PermissiveFileName = CommonHandler.FileDialog("ini");
        }

        #endregion

        #region OpenLanguageFile Simple implementation of ICommand
        private ICommand openLanguageFile;
        public ICommand OpenLanguageFile
        {
            get
            {
                if (openLanguageFile == null)
                    openLanguageFile = new Commands.VoidCommand(_OpenLanguageFile,true) { };
                return openLanguageFile;
            }
            set
            {
                openLanguageFile = value;
            }
        }

        private void _OpenLanguageFile()
        {
            LanguageFileName = CommonHandler.FileDialog("txt");
        }

        #endregion

        #region CompareMessages Simple implementation of ICommand
        private ICommand compareMessages;
        public ICommand CompareMessages
        {
            get
            {
                if (compareMessages == null)
                    compareMessages = new Commands.VoidCommand(_CompareMessages,true) { };
                return compareMessages;
            }
            set
            {
                compareMessages = value;
            }
        }

        private void _CompareMessages()
        {
            Task TCompareMessages = new Task(() => ResultText = Compare(MessageFileName, CompareMessagesOperation));
            TCompareMessages.Start();
        }

        #endregion

        #region ComparePermissives Simple implementation of ICommand
        private ICommand comparePermissives;
        public ICommand ComparePermissives
        {
            get
            {
                if (comparePermissives == null)
                    comparePermissives = new Commands.VoidCommand(_ComparePermissives,true) { };
                return comparePermissives;
            }
            set
            {
                comparePermissives = value;
            }
        }

        private void _ComparePermissives()
        {
            Task TComparePermissives = new Task(() => Compare(PermissiveFileName, ComparePermissivesOperation));
            TComparePermissives.Start();
        }

        #endregion

        #region GenerateOPPermissivesFilesFromHMI Simple implementation of ICommand
        private ICommand generateOPPermissive;
        public ICommand GenerateOPPermissive
        {
            get
            {
                if (generateOPPermissive == null)
                    generateOPPermissive = new Commands.VoidCommand(_GenerateOPPermissive, true) { };
                return generateOPPermissive;
            }
            set
            {
                generateOPPermissive = value;
            }
        }

        private void _GenerateOPPermissive()
        {
            Task TGenerateOPPermissive = new Task(() => GenerateOPPermissivesFilesFromHMI());
            TGenerateOPPermissive.Start();
        }

        #endregion

        #region Exit Simple implementation of ICommand

        private ICommand exit;
        public ICommand Exit
        {
            get
            {
                if (exit == null)
                    exit = new Commands.VoidCommand(CloseChecker, true) { };
                return exit;
            }
            set
            {
                exit = value;
            }
        }
        private void CloseChecker()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region About Simple implementation of ICommand

        private ICommand about;
        public ICommand About
        {
            get
            {
                if (about == null)
                    about = new Commands.VoidCommand(AboutHandler, true) { };
                return about;
            }
            set
            {
                about = value;
            }
        }
        private void AboutHandler()
        {
            MessageBox.Show("This utility makes Your life easy if You are Danieli Automation L1.\n\n" +
                "Easy way to find miising text or combine translated Permissives files for OP panels.\n\n" +
                "Author - Denis Shishmariov\n" +
                "Contact - d.shishmarev@danieli.com", "Utility for Danieli Automation L1");
        }

        #endregion

        delegate string CompareFunction(StreamReader operationSr, string languageText);

        string Compare(string operationFileName, CompareFunction Compare)
        {
            Status = myCheckerModel.PROCESSING;
            FileStream operationFs = new FileStream(operationFileName, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader operationSr = new StreamReader(operationFs); // создаем «потоковый читатель» и связываем его с файловым потоком

            FileStream languageFs = new FileStream(LanguageFileName, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader language = new StreamReader(languageFs); // создаем «потоковый читатель» и связываем его с файловым потоком

            
            string languageText = language.ReadToEnd();

            string text = Compare(operationSr, languageText);

            operationSr.Close();
            operationFs.Close();
            language.Close();
            languageFs.Close();
            MessageBox.Show($"Found {FoundCounter} missed texts.");
            Status = myCheckerModel.READY;
            return text;
        }

        string CompareMessagesOperation(StreamReader operationSr, string languageText)
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
                            ResultText = text;
                            FoundCounter = textCounterFun.ToString();
                            Status = $"Found text: {lineText}";
                        }
                    }
                }
            }
            FoundCounter = textCounterFun.ToString();
            return text;
        }

        string ComparePermissivesOperation(StreamReader operationSr, string languageText)
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
                            ResultText = text;
                            FoundCounter = textCounterFun.ToString();
                            Status = $"Found text: {lineText}";
                        }
                    }
                }
            }
            
            return text;
        }

        void GenerateOPPermissivesFilesFromHMI()
        {
            FileStream permissivesFs = new FileStream(PermissiveFileName, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
            StreamReader permissives = new StreamReader(permissivesFs); // создаем «потоковый читатель» и связываем его с файловым потоком
            int movedTextsCounter = 0, createdFilesCounter = 0;
            string fileList="";
            while (!permissives.EndOfStream)
            {
                string lineText = permissives.ReadLine();
                if (lineText.StartsWith("[") && !lineText.Contains("[0ST_IntkPerm_Empty]"))
                {
                    string tagName = lineText.Substring(1, lineText.Length - 2);
                    Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\OPfiles");
                    FileStream permissiveOPfs = File.Create($@"{Directory.GetCurrentDirectory()}\OPfiles\{tagName}.intk");
                    StreamWriter permissivesOPWr = new StreamWriter(permissiveOPfs, Encoding.Default);
                    Status = $"New permissive file created: {tagName}";
                    fileList = fileList.Insert(0, $"[{tagName}]\r\n");
                    ResultText = fileList;
                    permissivesOPWr.WriteLine($"{lineText}");
                    createdFilesCounter++;
                    do
                    {
                        lineText = permissives.ReadLine();
                        if (!lineText.StartsWith("Intk00") && (lineText.StartsWith("Intk") || lineText.StartsWith("Panel")))
                        {
                            int charId = lineText.IndexOf("=");
                            if (charId >= 0)
                            {
                                
                                Encoding enc = Encoding.Default;
                                FileStream languageFs = new FileStream(LanguageFileName, FileMode.Open, FileAccess.Read); //открывает файл только на чтение
                                StreamReader language = new StreamReader(languageFs, enc); // создаем «потоковый читатель» и связываем его с файловым потоком
                                string filteredText = lineText.Remove(0, charId + 1).Trim();
                                string headerText = lineText.Remove(charId + 1, lineText.Length - charId - 1).Trim();

                                while (!language.EndOfStream && filteredText != "")
                                {
                                    string languageText = language.ReadLine();
                                    if (languageText.Contains(filteredText))
                                    {
                                        charId = languageText.IndexOf("=");
                                        string translatedText = languageText.Remove(0, charId + 1).Trim();
                                        lineText = $"{headerText}{translatedText}";
                                        FoundCounter = movedTextsCounter++.ToString();
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
            Status = myCheckerModel.READY;
            MessageBox.Show($"{createdFilesCounter} files have been created, {movedTextsCounter} texts have been moved.");
        }

        #region Complicated Implementation of ICommand. !!!! To test and implement later !!!!

        //private bool canBrowseMessageFileComplicated;
        //private ICommand openMessageFileCmdComplicated;
        //public ICommand OpenMessageFileCmdComplicated
        //{
        //    get
        //    {
        //        if (openMessageFileCmdComplicated == null) openMessageFileCmdComplicated = new Commands.CompicatedCommand(o => canBrowseMessageFileComplicated, o => BrowseMessageFile());
        //        return openMessageFileCmdComplicated;
        //    }
        //    set
        //    {
        //        openMessageFileCmdComplicated = value;
        //    }
        //}
        //private void BrowseMessageFile()
        //{
        //    MessageBox.Show("Test");
        //}
        #endregion

    }
}
