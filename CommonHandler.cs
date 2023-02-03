using System;

namespace LangTextChecker
{
    static class CommonHandler
    {
        public static string FileDialog(string extension)
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
    }
}
