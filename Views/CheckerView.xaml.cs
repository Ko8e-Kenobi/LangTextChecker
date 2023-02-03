using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LangTextChecker.ViewModels;

namespace LangTextChecker.View
{
    /// <summary>
    /// Interaction logic for CheckerView.xaml
    /// </summary>
    public partial class CheckerView : Window
    {
        private const string PROCESSING = "Wait while processing...", READY = "Ready...";
        Checker MyChecker = new Checker();

        public CheckerView()
        {
            InitializeComponent();
            DataContext = new CheckerViewModel();
        }

        private void compareMessages_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBox.Text = PROCESSING;
            resultText.Text = MyChecker.CompareMessages(MessageFileName.Text, LanguageFileName.Text, out string counter);
            textCounter.Text = counter;
            StatusTextBox.Text = READY;
        }

        private void comparePermissives_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBox.Text = PROCESSING;
            resultText.Text = MyChecker.ComparePermissives(PermissiveFileName.Text, LanguageFileName.Text, out string counter);
            textCounter.Text = counter;
            StatusTextBox.Text = READY;
        }

        private void SplitToOPIntk_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBox.Text = PROCESSING;
            MyChecker.GenerateOPPermissivesFilesFromHMI(PermissiveFileName.Text, LanguageFileName.Text);
            StatusTextBox.Text = READY;
        }
    }
}
