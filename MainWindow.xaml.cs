using System;
using System.Windows;
using System.IO;
using System.Windows.Input;
using System.Text;
using LangTextChecker.ViewModels;
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
            View.CheckerView checkerView = new View.CheckerView();
            checkerView.Show();
            Close();

        }
    }
}
