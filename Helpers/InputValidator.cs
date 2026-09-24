//---imports---
using System.Windows;
using System.Windows.Controls;
//---namespace---
namespace WindowsSetupTool.Helpers
{
    //---class init---
    public static class InputValidator
    {
        //---bool returning method for validating all text and password boxes---
        public static bool ValidateAll(DependencyObject root)
        {
            bool textBoxesValid = ValidateAllTextBoxes(root);
            bool passwordBoxesValid = ValidateAllPasswordBoxes(root);

            return textBoxesValid && passwordBoxesValid;
        }

        //---bool returning method to validate all enabled textboxes---     
        public static bool ValidateAllTextBoxes(DependencyObject root)
        {
            var textBoxes = VisualTreeHelperExtensions.GetAllTextBoxes(root)
                .Where(tb => tb.IsEnabled);

            foreach (var tb in textBoxes)
            {
                if (!ValidateSingleTextBox(tb))
                    return false;
            }

            return true;
        }

        //---bool returning method to validate all enabled textboxes---
        public static bool ValidateAllPasswordBoxes(DependencyObject root)
        {
            var passwordBoxes = VisualTreeHelperExtensions.GetAllPasswordBoxes(root)
                .Where(pw => pw.IsEnabled);

            foreach (var pw in passwordBoxes)
            {
                if (!ValidateSinglePasswordBox(pw))
                    return false;
            }

            return true;
        }

        //---validates a single textbox returns a bool---
        public static bool ValidateSingleTextBox(TextBox tb)
        {
            return !string.IsNullOrWhiteSpace(tb.Text);
        }

        //---validates a single passwordbox returns a bool---
        public static bool ValidateSinglePasswordBox(PasswordBox pw)
        {
            return !string.IsNullOrWhiteSpace(pw.Password);
        }
    }
}