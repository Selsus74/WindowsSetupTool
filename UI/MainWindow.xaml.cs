//---imports---
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsSetupTool.Helpers;
using WindowsSetupTool.Models;
using WindowsSetupTool.Setup;
//---namespace---
namespace WindowsSetupTool.UI;

//---class init---
public partial class MainWindow : Window
{
    //---progress helper init---
    private readonly SetupProgressHelper _progressHelper;

    //---system tasks predefine---
    private readonly SetupTask _setHostnameTask = new();
    private readonly CheckBox _setHostnameCheckBox = new();
    private readonly TextBox _setHostnameTextBox = new();

    //---user tasks predefine
    private readonly SetupTask _createNewLocalUserTask = new();
    private readonly CheckBox _createUserCheckbox = new();
    private readonly TextBox _usernameBox = new();
    private readonly PasswordBox _userPasswordBox = new();
    private readonly CheckBox _userIsAdminCheckBox = new();

    private readonly SetupTask _activateAdminTask = new();
    private readonly CheckBox _activateAdminCheckbox = new();
    private readonly PasswordBox _adminPasswordBox = new();

    //---explorer task list predefine---
    private readonly List<SetupTask> _explorerSettingsTasks = [];

    //---software task list predefine---
    private readonly List<SetupTask> _softwareInstallTasks = [];

    //---invoke main window---
    public MainWindow()
    {
        InitializeComponent();

        SourceInitialized += (s, e) =>
        {
            var bg = ((SolidColorBrush)FindResource("WindowBorderBrush")).Color;
            var text = ((SolidColorBrush)FindResource("TextBrush")).Color;
            TitleBarHelper.Apply(this, bg, text);
        };

        //---must be UI-Thread and before setting task execute---
        _progressHelper = new SetupProgressHelper(new Progress<SetupProgressInfo>(OnProgressChanged));
        _progressHelper.AllTasksCompleted += OnAllTasksCompleted;

        SetupProgress.Minimum = 0;

        LoadSystemInformation();
        CreateTasks();
        DisplayTasks();
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
        //---create system tasks---
        _setHostnameTask.Name = "Hostnamen ändern";
        _setHostnameTask.Description = "Hostnamen des Geräts festlegen.";
        _setHostnameTask.IsSelected = false;
        _setHostnameTask.Execute = null;

        //---create user tasks---
        _createNewLocalUserTask.Name = "Neuen Benutzer Anlegen";
        _createNewLocalUserTask.Description = "Erstellt einen neuen lokalen Benutzer.";
        _createNewLocalUserTask.IsSelected = false;
        _createNewLocalUserTask.Execute = null;

        //---create admin tasks---
        _activateAdminTask.Name = "Lokalen Administrator aktivieren";
        _activateAdminTask.Description = "Aktiviert den standard lokalen Administrator.";
        _activateAdminTask.IsSelected = false;
        _activateAdminTask.Execute = null;

        //---create explorer tasks---
        _explorerSettingsTasks.Add(new SetupTask
        {
            Name = "Altes Kontextmenü aktivieren",
            Description = "Aktiviert das von Windows 10 bekannte Kontextmenü.",
            IsSelected = true,
            Execute = () => ExplorerSettings.EnableClassicContextMenu(_progressHelper)
        });
        _explorerSettingsTasks.Add(new SetupTask
        {
            Name = "Dateiendungen anzeigen",
            Description = "Zeigt bekannte Dateiendungen im Windows Explorer an.",
            IsSelected = true,
            Execute = () => ExplorerSettings.ShowFileExtensions(_progressHelper)
        });

        //---create software tasks---
        //---get all software from repository---
        SoftwareRepository softwareRepository = new SoftwareRepository();
        var allSoftware = softwareRepository.GetAllSoftwares;
        foreach (var software in allSoftware)
        {
            _softwareInstallTasks.Add(new SetupTask
            {
                Name = software.Name,
                Description = $"Installiere {software.Name}",
                IsSelected = false,
                Execute = () => WingetInstaller.InstallAsync(software, _progressHelper)
            });
        }
    }

    //---renders the tasks in the taskform---
    private void DisplayTasks()
    {
        //=====SYSTEM TASKS=====
        SystemTaskPanel.Children.Clear();

        _setHostnameCheckBox.Content = CreateTaskContent(_setHostnameTask);
        _setHostnameCheckBox.IsChecked = _setHostnameTask.IsSelected;
        _setHostnameCheckBox.Tag = _setHostnameTask;
        _setHostnameCheckBox.Margin = new Thickness(0, 5, 0, 5);

        var hostnameWarningText = new TextBlock { Text = "Bitte vergeben Sie einen Hostnamen!", Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };

        //---add stackpanel for inputs---
        StackPanel hostnameInputPanel = new()
        {
            Margin = new Thickness(20, 0, 0, 10),
            IsEnabled = _setHostnameTask.IsSelected   // initial status from task
        };
        //---hide warning if initial status is not selected---
        if (!_setHostnameTask.IsSelected)
        {
            hostnameWarningText.Visibility = Visibility.Hidden;
        }
        
        _setHostnameTextBox.Margin = new Thickness(0, 2, 0, 2);
        hostnameInputPanel.Children.Add(new TextBlock { Text = "Hostname:" });
        hostnameInputPanel.Children.Add(_setHostnameTextBox);
        hostnameInputPanel.Children.Add(hostnameWarningText);

        SystemTaskPanel.Children.Add(_setHostnameCheckBox);
        SystemTaskPanel.Children.Add(hostnameInputPanel);

        //---eventhandlers for input form---
        _setHostnameCheckBox.Checked += (s, e) =>
        {
            hostnameInputPanel.IsEnabled = true;

            if (!InputValidator.ValidateSingleTextBox(_setHostnameTextBox))
            {
                hostnameWarningText.Visibility = Visibility.Visible;
            }

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };
        _setHostnameCheckBox.Unchecked += (s, e) =>
        {
            hostnameInputPanel.IsEnabled = false;

            hostnameWarningText.Visibility = Visibility.Hidden;

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };
        _setHostnameTextBox.TextChanged += (s, e) =>
        {
            if (InputValidator.ValidateSingleTextBox(_setHostnameTextBox))
            {
                hostnameWarningText.Visibility = Visibility.Hidden;
            }
            else
            {
                hostnameWarningText.Visibility = Visibility.Visible;
            }

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };


        //=====USER SETTINGS=====
        //---clear panel---
        UserTaskPanel.Children.Clear();

        //---add user checkbox---
        _createUserCheckbox.Content = CreateTaskContent(_createNewLocalUserTask);
        _createUserCheckbox.IsChecked = _createNewLocalUserTask.IsSelected;
        _createUserCheckbox.Tag = _createNewLocalUserTask;
        _createUserCheckbox.Margin = new Thickness(0, 5, 0, 5);
        
        UserTaskPanel.Children.Add(_createUserCheckbox);

        //---add warning texts---
        var usernameWarningText = new TextBlock { Text = "Bitte vergeben Sie einen Benutzernamen!", Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
        var userPasswordWarningText = new TextBlock { Text = "Bitte vergeben Sie ein Passwort!", Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };

        //---add stackpanel for inputs---
        StackPanel userInputPanel = new()
        {
            Margin = new Thickness(20, 0, 0, 10),
            IsEnabled = _createNewLocalUserTask.IsSelected   // initial status from task
        };
        if (!_createNewLocalUserTask.IsSelected)
        {
            usernameWarningText.Visibility = Visibility.Hidden;
            userPasswordWarningText.Visibility = Visibility.Hidden;
        }

        //---input forms in stackpanel---
        _usernameBox.Margin = new Thickness(0, 2, 0, 2);
        _userPasswordBox.Margin = new Thickness(0, 2, 0, 2);
        _userIsAdminCheckBox.Content = "Als Administrator anlegen";
        _userIsAdminCheckBox.Margin = new Thickness(0, 2, 0, 2);

        //---add all elements to stackpanel---
        userInputPanel.Children.Add(new TextBlock { Text = "Benutzername:" });
        userInputPanel.Children.Add(_usernameBox);
        userInputPanel.Children.Add(usernameWarningText);
        userInputPanel.Children.Add(new TextBlock { Text = "Passwort:" });
        userInputPanel.Children.Add(_userPasswordBox);
        userInputPanel.Children.Add(userPasswordWarningText);
        userInputPanel.Children.Add(_userIsAdminCheckBox);

        //---add stackpanel to parent usertaskpanel---
        UserTaskPanel.Children.Add(userInputPanel);

        //---eventhandlers for input form---
        _createUserCheckbox.Checked += (s, e) =>
        {
            userInputPanel.IsEnabled = true;

            if (!InputValidator.ValidateSingleTextBox(_usernameBox))
            {
                usernameWarningText.Visibility = Visibility.Visible;
            }

            if (!InputValidator.ValidateSinglePasswordBox(_userPasswordBox))
            {
                userPasswordWarningText.Visibility = Visibility.Visible;
            }

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };
        _createUserCheckbox.Unchecked += (s, e) =>
        {
            userInputPanel.IsEnabled = false;

            usernameWarningText.Visibility = Visibility.Hidden;
            userPasswordWarningText.Visibility = Visibility.Hidden;

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };
        _usernameBox.TextChanged += (s, e) =>
        {
            if (InputValidator.ValidateSingleTextBox(_usernameBox))
            {
                usernameWarningText.Visibility = Visibility.Hidden;
            }
            else usernameWarningText.Visibility = Visibility.Visible;

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };
        _userPasswordBox.PasswordChanged += (s, e) =>
        {
            if (InputValidator.ValidateSinglePasswordBox(_userPasswordBox))
            {
                userPasswordWarningText.Visibility = Visibility.Hidden;
            }
            else userPasswordWarningText.Visibility = Visibility.Visible;

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };


        //=====ADMIN SETTINGS=====
        //---add admin checkbox---
        _activateAdminCheckbox.Content = CreateTaskContent(_activateAdminTask);
        _activateAdminCheckbox.IsChecked = _activateAdminTask.IsSelected;
        _activateAdminCheckbox.Tag = _activateAdminTask;
        _activateAdminCheckbox.Margin = new Thickness(0, 5, 0, 5);
        _adminPasswordBox.Margin = new Thickness(0, 2, 0, 2);

        var adminPasswordWarningText = new TextBlock { Text = "Bitte vergeben Sie ein Passwort!", Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };

        //---add stackpanel for inputs---
        StackPanel adminInputPanel = new()
        {
            Margin = new Thickness(20, 0, 0, 10),
            IsEnabled = _activateAdminTask.IsSelected   // initial status from task
        };
        if (!_activateAdminTask.IsSelected)
        {
            adminPasswordWarningText.Visibility = Visibility.Hidden;
        }

        //---add input and text to input panel---
        adminInputPanel.Children.Add(new TextBlock { Text = "Passwort:" });
        adminInputPanel.Children.Add(_adminPasswordBox);
        adminInputPanel.Children.Add(adminPasswordWarningText);
        //---add checkbox and corresponding input panel to taskpanel---
        UserTaskPanel.Children.Add(_activateAdminCheckbox);
        UserTaskPanel.Children.Add(adminInputPanel);

        //---eventhandler switches input form depending on initial user checkbox---
        _activateAdminCheckbox.Checked += (s, e) =>
        {
            adminInputPanel.IsEnabled = true;

            if (InputValidator.ValidateSinglePasswordBox(_adminPasswordBox))
            {
                adminPasswordWarningText.Visibility = Visibility.Hidden;
            }
            else
            {
                adminPasswordWarningText.Visibility = Visibility.Visible;
            }

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };
        _activateAdminCheckbox.Unchecked += (s, e) =>
        { 
            adminInputPanel.IsEnabled = false;
            adminPasswordWarningText.Visibility = Visibility.Hidden;

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };
        _adminPasswordBox.PasswordChanged += (s, e) =>
        {
            if (InputValidator.ValidateSinglePasswordBox(_adminPasswordBox))
            {
                adminPasswordWarningText.Visibility = Visibility.Hidden;
            }
            else
            {
                adminPasswordWarningText.Visibility = Visibility.Visible;
            }

            startSetupButton.IsEnabled = InputValidator.ValidateAll(this);
        };


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


        //=====SOFTWARE INSTALLS=====
        SoftwareTaskPanel.Children.Clear();

        foreach (SetupTask task in _softwareInstallTasks)
        {
            CheckBox checkBox = new()
            {
                Content = CreateTaskContent(task),
                IsChecked = task.IsSelected,
                Tag = task,
                Margin = new Thickness(0, 5, 0, 5)
            };

            SoftwareTaskPanel.Children.Add(checkBox);
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

    //---start setup---
    private async void StartSetup_Click(
        object sender,
        RoutedEventArgs e)
    {
        //---deactivate button until finished---
        startSetupButton.IsEnabled = false;

        //---reset Logger fixes if run again without closing---
        Logger.ResetCounters();

        //---reset progressbar color and text fixes visual glitch out if run again without closing---
        SetupProgress.Foreground = (SolidColorBrush)Application.Current.Resources["AccentBrush"];
        StatusText.Text = string.Empty;
        StatusText.Foreground = (SolidColorBrush)Application.Current.Resources["AccentBrush"];

        //---create list for all selected tasks---
        List<SetupTask> selectedTasks = [];

        //---system tasks define execute---
        if (_setHostnameCheckBox.IsChecked ?? false)
        {
            _setHostnameTask.IsSelected = true;
            _setHostnameTask.Execute = () => SystemSettings.SetHostname(_setHostnameTextBox.Text, _progressHelper);

            selectedTasks.Add(_setHostnameTask);
        }
        else
        {
            _setHostnameTask.IsSelected = false;
            _setHostnameTask.Execute = null;
        }

        //---user tasks define execute with current values---
        if (_createUserCheckbox.IsChecked ?? false)
        {
            _createNewLocalUserTask.IsSelected = true;
            _createNewLocalUserTask.Execute = () =>
                UserSettings.CreateLocalUser(
                    _usernameBox.Text,
                    _userPasswordBox.Password,
                    _userIsAdminCheckBox.IsChecked ?? false,
                    _progressHelper
                );

            selectedTasks.Add(_createNewLocalUserTask);
        }
        else
        {
            _createNewLocalUserTask.IsSelected = false;
            _createNewLocalUserTask.Execute = null;
        }

        // ---admin tasks define execute with current values---
        if (_activateAdminCheckbox.IsChecked ?? false)
        {
            _activateAdminTask.IsSelected = true;
            _activateAdminTask.Execute = () =>
                UserSettings.ActivateLocalAdmin(
                    _adminPasswordBox.Password,
                    _progressHelper
                );

            selectedTasks.Add(_activateAdminTask);
        }
        else
        {
            _activateAdminTask.IsSelected = false;
            _activateAdminTask.Execute = null;
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

        //---get selected tasks from software task panel---
        foreach (CheckBox checkBox in SoftwareTaskPanel.Children.OfType<CheckBox>())
        {
            if (checkBox.Tag is SetupTask task &&
                checkBox.IsChecked == true)
            {
                selectedTasks.Add(task);
            }
        }


        //---msg box when no tasks are selected---
        if (selectedTasks.Count == 0)
        {
            MessageBox.Show(
                "Bitte mindestens eine Einstellung auswählen.",
                "Keine Auswahl",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        //---sets total tasks for progress helper class---
        _progressHelper.TotalTasks = selectedTasks.Count;

        //---start all selected tasks---
        foreach (SetupTask task in selectedTasks)
        {
            if (task.Execute == null)
            {
                //---if nothing to execute is defined in a task, report it as done---
                //---otherwise the helper class would never reach total task count---
                _progressHelper.ReportTaskCompleted();
                continue;
            }

            //---start task - dont await -> parallel---
            //---error handling and progress updates are called in each class corresponding to the task---
            _ = task.Execute();
        }
    }

    //---method is called on progress changed event from handler class---
    private void OnProgressChanged(SetupProgressInfo info)
    {
        SetupProgress.Maximum = info.Total;
        SetupProgress.Value = info.Done;
        StatusText.Text = $"Einrichtung läuft... ({info.Done}/{info.Total})";
    }

    //---method is called on event all tasks completed---
    private void OnAllTasksCompleted(int failed)
    {
        startSetupButton.IsEnabled = true;

        //---summarize failed tasks with warnings and errors---
        //---changes progressbar text and color---
        if (failed > 0 || Logger.ErrorCount > 0)
        {
            StatusText.Text = $"✘ Setup exited with {failed} failed tasks [{Logger.WarningCount} Warning(s) / {Logger.ErrorCount} Error(s)]";
            StatusText.Foreground = new SolidColorBrush(Color.FromRgb(244, 45, 12));
            SetupProgress.Foreground = new SolidColorBrush(Color.FromRgb(244, 45, 12));
        }
        else if (failed > 0 || Logger.WarningCount > 0)
        {
            StatusText.Text = $"✘ Setup exited with {failed} failed tasks [{Logger.WarningCount} Warning(s) / {Logger.ErrorCount} Error(s)]";
            StatusText.Foreground = new SolidColorBrush(Color.FromRgb(244, 179, 12));
            SetupProgress.Foreground = new SolidColorBrush(Color.FromRgb(244, 179, 12));
        }
        else
        {
            StatusText.Text = $"✔ Setup completed successfully!";
            StatusText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            SetupProgress.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
        }


        //---popup msgbox when done with error level summary---
        var dialog = new ShowLogDialog($"Setup exited with: {Logger.GetSummary()}")
        {
            Owner = this // centers the dialog over MainWindow
        };
        dialog.ShowDialog();
    }
}