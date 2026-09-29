//---imports---
using System.Windows;
using System.Windows.Media;
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

            SourceInitialized += (s, e) =>
            {
                var bg = ((SolidColorBrush)FindResource("WindowBorderBrush")).Color;
                var text = ((SolidColorBrush)FindResource("TextBrush")).Color;
                TitleBarHelper.Apply(this, bg, text);
            };

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
