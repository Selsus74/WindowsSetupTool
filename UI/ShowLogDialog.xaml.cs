//---imports---
using System.Windows;
using WindowsSetupTool.Helpers;
//---namespace---
namespace WindowsSetupTool.UI
{
    //---class init---
    public partial class ShowLogDialog : Window
    {
        public ShowLogDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ShowLogButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.OpenLatestLog();
            Close();
        }
    }
}
