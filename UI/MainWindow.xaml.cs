using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WindowsSetupTool.Models;
using WindowsSetupTool.Setup;
using WindowsSetupTool.Helpers;
using System.Linq.Expressions;

namespace WindowsSetupTool.UI;

public partial class MainWindow : Window
{
    //---user tasks predefine
    private readonly SetupTask CreateNewLocalUserTask = new();
    private readonly CheckBox createUserCheckbox = new();
    private readonly TextBox usernameBox = new();
    private readonly PasswordBox userPasswordBox = new();
    private readonly CheckBox userIsAdminCheckBox = new();

    //---explorer task list predefine---
    private readonly List<SetupTask> _explorerSettingsTasks = [];

    //---main window---
    public MainWindow()
    {
        InitializeComponent();

        LoadSystemInformation();
        CreateTasks();
        DisplayTasks();
        ResetProgressbar();
    }

    //---get sys info for window header---
    private void LoadSystemInformation()
    {
        ComputerNameText.Text =
            SystemInformation.GetComputerName();

        UserNameText.Text =
            SystemInformation.GetUserName();
    }

    //---create tasks---
    private void CreateTasks()
    {
        //---create user tasks---
        CreateNewLocalUserTask.Name = "Neuen Benutzer Anlegen";
        CreateNewLocalUserTask.Description = "Erstellt einen neuen lokalen Benutzer.";
        CreateNewLocalUserTask.IsSelected = false;
        CreateNewLocalUserTask.Execute = null;

        //---create explorer tasks---
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
        //=====USER SETTINGS=====
        //---clear panel---
        UserTaskPanel.Children.Clear();

        //---add user checkbox---
        createUserCheckbox.Content = CreateTaskContent(CreateNewLocalUserTask);
        createUserCheckbox.IsChecked = CreateNewLocalUserTask.IsSelected;
        createUserCheckbox.Tag = CreateNewLocalUserTask;
        createUserCheckbox.Margin = new Thickness(0, 5, 0, 5);
        
        UserTaskPanel.Children.Add(createUserCheckbox);

        //---add stackpanel for inputs---
        StackPanel userInputPanel = new()
        {
            Margin = new Thickness(20, 0, 0, 10),
            IsEnabled = CreateNewLocalUserTask.IsSelected   // initial status from task
        };

        //---input forms in stackpanel---
        usernameBox.Margin = new Thickness(0, 2, 0, 2);
        userPasswordBox.Margin = new Thickness(0, 2, 0, 2);
        userIsAdminCheckBox.Content = "Als Administrator anlegen";
        userIsAdminCheckBox.Margin = new Thickness(0, 2, 0, 2);

        //---add all elements to stackpanel---
        userInputPanel.Children.Add(new TextBlock { Text = "Benutzername:" });
        userInputPanel.Children.Add(usernameBox);
        userInputPanel.Children.Add(new TextBlock { Text = "Passwort:" });
        userInputPanel.Children.Add(userPasswordBox);
        userInputPanel.Children.Add(userIsAdminCheckBox);

        //---add stackpanel to parent usertaskpanel---
        UserTaskPanel.Children.Add(userInputPanel);

        //---eventhandler switches input form depending on initial user checkbox---
        createUserCheckbox.Checked += (s, e) => userInputPanel.IsEnabled = true;
        createUserCheckbox.Unchecked += (s, e) => userInputPanel.IsEnabled = false;


        //=====EXPLORER SETTINGS=====
        ExplorerTaskPanel.Children.Clear();

        foreach (SetupTask task in _explorerSettingsTasks)
        {
            CheckBox checkBox = new()
            {
                Content = CreateTaskContent(task),
                IsChecked = task.IsSelected,
                Tag = task,
                Margin = new Thickness(0, 5, 0, 5)
            };

            ExplorerTaskPanel.Children.Add(checkBox);
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

    //---progressbar resetter---
    private void ResetProgressbar()
    {
        StatusText.Text = "";
        StatusText.Foreground = new SolidColorBrush(Color.FromRgb(76, 141, 255));
        SetupProgress.Value = 0;
        SetupProgress.Foreground = new SolidColorBrush(Color.FromRgb(76, 141, 255));
    }

    //---start setup---
    private async void StartSetup_Click(
        object sender,
        RoutedEventArgs e)
    {
        ResetProgressbar();

        //---create list for all selected tasks---
        List<SetupTask> selectedTasks = [];

        //---user tasks define execute with current values---
        if (createUserCheckbox.IsChecked ?? false)
        {
            CreateNewLocalUserTask.IsSelected = true;
            CreateNewLocalUserTask.Execute = () =>
                UserSettings.CreateLocalUser(
                    usernameBox.Text,
                    userPasswordBox.Password,
                    userIsAdminCheckBox.IsChecked ?? false
                );

            selectedTasks.Add(CreateNewLocalUserTask);
        }
        else
        {
            CreateNewLocalUserTask.IsSelected = false;
            CreateNewLocalUserTask.Execute = null;
        }

        //---get selected tasks from explorer task panel---
        foreach (CheckBox checkBox in ExplorerTaskPanel.Children.OfType<CheckBox>())
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

        //---changes color of progress bar and sets status text with color depending on error levels---
        if (SetupProgress.Value >= SetupProgress.Maximum)
        {
            if (Logger.ErrorCount > 0)
            {
                StatusText.Text = $"✘ Einrichtung mit {Logger.WarningCount} Warning(s) und {Logger.ErrorCount} Error(s) abgeschlossen";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(244, 45, 12));
                SetupProgress.Foreground = new SolidColorBrush(Color.FromRgb(244, 45, 12));
            }
            else if (Logger.WarningCount > 0)
            {
                StatusText.Text = $"✘ Einrichtung mit {Logger.WarningCount} Warning(s) und {Logger.ErrorCount} Error(s) abgeschlossen";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(244, 179, 12));
                SetupProgress.Foreground = new SolidColorBrush(Color.FromRgb(244, 179, 12));
            }
            else
            {
                StatusText.Text = $"✔ Einrichtung mit ohne Fehler abgeschlossen";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                SetupProgress.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            }
        }

        //---popup msgbox when done with error level summary---
        MessageBox.Show(
            $"Einrichtung beendet mit: {Logger.GetSummary()};",
            "Setup abgeschlossen",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );

        //---reset if run again without closing---
        Logger.ResetCounters();

    }
}