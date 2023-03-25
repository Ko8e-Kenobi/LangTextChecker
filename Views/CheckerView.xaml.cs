using System.Windows;
using LangTextChecker.ViewModels;

namespace LangTextChecker.View
{
    public partial class CheckerView : Window
    {
        public CheckerView()
        {
            InitializeComponent();
            DataContext = new CheckerViewModel();
        }
    }
}
