//---imports---
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//---namespace---
namespace WindowsSetupTool.Helpers
{
    //---class init---
    public static class VisualTreeHelperExtensions
    {
        //---enumerator method to get all textboxes in a given context---
        public static IEnumerable<TextBox> GetAllTextBoxes(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is TextBox tb)
                    yield return tb;

                foreach (var descendant in GetAllTextBoxes(child))
                    yield return descendant;
            }
        }

        //---enumerator method to get all passwordboxes in a given context---
        public static IEnumerable<PasswordBox> GetAllPasswordBoxes(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is PasswordBox pw)
                    yield return pw;

                foreach (var descendant in GetAllPasswordBoxes(child))
                    yield return descendant;
            }
        }
    }
}
