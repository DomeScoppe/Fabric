using Loom.Core;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace Loom
{
    /// <summary>
    /// Interaction logic for EnginePathDialog.xaml
    /// </summary>
    public partial class EnginePathDialog : Window
    {
        public string FabricPath { get; private set; }

        public EnginePathDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void OnBrowseButtonClicked(object sender, RoutedEventArgs e)
        {
            FolderBrowser.Browse(pathTextBox);
        }

        private void OnOk_Button_Click(object sender, RoutedEventArgs e)
        {
            var path = pathTextBox.Text.Trim();
            messageTextBlock.Text = string.Empty;

            if(string.IsNullOrEmpty(path))
            {
                messageTextBlock.Text = "Invalid path.";
            }
            else if(path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                messageTextBlock.Text = "Invalid character(s) used in path.";
            }
            else if(!Directory.Exists(Path.Combine(path, @"Fabric\EngineAPI\")))
            {
                messageTextBlock.Text = "Path doesn't contain Fabric Engine files.";
            }

            if(string.IsNullOrEmpty(messageTextBlock.Text))
            {
                if (!Path.EndsInDirectorySeparator(path)) path += @"\";

                FabricPath = path;
                DialogResult = true;
                Close();
            }
        }
    }
}
