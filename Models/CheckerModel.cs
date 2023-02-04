using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace LangTextChecker.Models
{
    public class CheckerModel : INotifyPropertyChanged
    {
        public string PROCESSING { get => "Wait while processing..."; }
        public string READY { get => "Ready..."; }

        private string status;

        private string counter;

        private string resultText;

        private string messageFileName;

        private string permissiveFileName;

        private string languageFileName;

        public string MessageFileName { get => messageFileName; set => messageFileName = value; } 

        public string PermissiveFileName { get => permissiveFileName; set => permissiveFileName = value; }

        public string LanguageFileName { get => languageFileName; set => languageFileName = value; }

        public string ResultText { get => resultText; set => resultText = value; }

        public string Counter { get => counter; set => counter = value; }

        public string Status { get => status; set => status = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "") 
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
