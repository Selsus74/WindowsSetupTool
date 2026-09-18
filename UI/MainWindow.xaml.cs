using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WindowsSetupTool.Models;
using WindowsSetupTool.Setup;

namespace WindowsSetupTool.UI;

public partial class MainWindow : Window
{
    private readonly List<SetupTask> _explorerSettingsTasks = [];
    private readonly List<SetupTask> _userSettingsTasks = [];

    public MainWindow()
    {
        InitializeComponent();

        LoadSystemInformation();
        CreateTasks();
        DisplayTasks();
    }

    private void LoadSystemInformation()
    {
        ComputerNameText.Text =
            SystemInformation.GetComputerName();

        UserNameText.Text =
            SystemInformation.GetUserName();
    }

    private void CreateTasks()
    {
        _userSettingsTasks.Add(new SetupTask
        {
            Name = "Neuen Benutzer Anlegen",
            Description = "Erstellt einen neuen lokalen Benutzer.",
            IsSelected = false,
            Execute = ExplorerSettings.EnableClassicContextMenu
        });


        _explorerSettingsTasks.Add(new SetupTask
        {
            Name = "Altes Kontextmenü aktivieren",
            Description = "Aktiviert das von Windows 10 bekannte Kontextmenü.",
            IsSelected = true,
            Execute = ExplorerSettings.EnableClassicContextMenu
        });
        _explorerSettingsTasks.Add(new SetupTask
        {
            Name = "Dateiendungen anzeigen",
            Description = "Zeigt bekannte Dateiendungen im Windows Explorer an.",
            IsSelected = true,
            Execute = ExplorerSettings.ShowFileExtensions
        });
    }

    //---renders the tasks in the taskform---
    private void DisplayTasks()
    {
        TaskPanel.Children.Clear();

        //--explorer settings (just a checkbox)--
        foreach (SetupTask task in _explorerSettingsTasks)
        {
            CheckBox checkBox = new()
            {
                Content = CreateTaskContent(task),
                IsChecked = task.IsSelected,
                Tag = task,
                Margin = new Thickness(0, 5, 0, 5)
            };

            TaskPanel.Children.Add(checkBox);
        }
    }

    private StackPanel CreateTaskContent(SetupTask task)
    {
        StackPanel panel = new();

        panel.Children.Add(
            new TextBlock
            {
                Text = task.Name,
                FontWeight = FontWeights.Bold,
                FontSize = 15
            });

        panel.Children.Add(
            new TextBlock
            {
                Text = task.Description,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 3, 0, 0)
            });

        return panel;
    }

    private void SelectAll_Click(
        object sender,
        RoutedEventArgs e)
    {
        foreach (CheckBox checkBox in TaskPanel.Children.OfType<CheckBox>())
        {
            checkBox.IsChecked = true;
        }
    }

    private async void StartSetup_Click(
        object sender,
        RoutedEventArgs e)
    {
        List<SetupTask> selectedTasks = [];

        foreach (CheckBox checkBox in TaskPanel.Children.OfType<CheckBox>())
        {
            if (checkBox.Tag is SetupTask task &&
                checkBox.IsChecked == true)
            {
                selectedTasks.Add(task);
            }
        }

        if (selectedTasks.Count == 0)
        {
            MessageBox.Show(
                "Bitte mindestens eine Einstellung auswählen.",
                "Keine Auswahl",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        SetupProgress.Minimum = 0;
        SetupProgress.Maximum = selectedTasks.Count;
        SetupProgress.Value = 0;

        foreach (SetupTask task in selectedTasks)
        {
            try
            {
                StatusText.Text = $"Einrichtung läuft... ({SetupProgress.Value}/{SetupProgress.Maximum})";

                await Task.Delay(300);

                task.Execute?.Invoke();

                SetupProgress.Value++;

                StatusText.Text = $"Schritt beendet... ({SetupProgress.Value}/{SetupProgress.Maximum})";
            }
            catch (Exception ex)
            {
                StatusText.Text =
                    $"✗ Fehler bei: {task.Name}";

                MessageBox.Show(
                    $"Fehler bei:\n\n{task.Name}\n\n{ex.Message}",
                    "Setup-Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        if (SetupProgress.Value >= SetupProgress.Maximum)
        {
            SetupProgress.Foreground = new SolidColorBrush(
                Color.FromRgb(76, 175, 80));

            StatusText.Text = "✓ Einrichtung erfolgreich abgeschlossen";

            StatusText.Foreground = new SolidColorBrush(
                Color.FromRgb(76, 175, 80));
        }

        MessageBox.Show(
            "Die ausgewählten Einstellungen wurden verarbeitet.",
            "Setup abgeschlossen",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}