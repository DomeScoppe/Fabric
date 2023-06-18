using System.Windows.Controls;

namespace Loom.Core
{
    public partial class LoggerConsole : UserControl
    {
        public LoggerConsole()
        {
            InitializeComponent();
        }

        private void OnClearButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Logger.Clear();
        }

        private void OnMessageFilterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var filter = 0x0;
            if(toggleInfo.IsChecked == true)
            {
                filter |= (int)MessageType.Info;
            }
            if (toggleWarn.IsChecked == true)
            {
                filter |= (int)MessageType.Warn;
            }
            if (toggleError.IsChecked == true)
            {
                filter |= (int)MessageType.Error;
            }

            Logger.SetMessageFilter(filter);
        }
    }
}
