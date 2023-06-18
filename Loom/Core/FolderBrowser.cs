using Microsoft.Win32;
using System;
using System.Windows.Controls;

namespace Loom.Core
{
    public static class FolderBrowser
    {
        public static void Browse(object control)
        {
            var dialog = new OpenFileDialog();
            String remove = "Select folder";
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.FileName = remove;

            if (dialog.ShowDialog() == true)
            {
                var textBox = control as TextBox;
                textBox.Text = dialog.FileName.Remove(dialog.FileName.IndexOf(remove), remove.Length);
            }
        }
    }
}
