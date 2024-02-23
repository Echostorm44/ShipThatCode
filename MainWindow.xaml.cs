using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using Wpf.Ui.Controls;

namespace ShipThatCode;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : UiWindow
{
    MainViewModel vm;

    public MainWindow()
    {
        vm = new MainViewModel
        {
            SettingsPath = Path.Combine(AppContext.BaseDirectory, "profiles.json")
        };
        if(!File.Exists(vm.SettingsPath))
        {
            var sample = new DeployProfile
            {
                ID = 1,
                Title = "WhatHuh CPU",
                DefaultInstallFolderName = "WhatHuh",
                ExeFileName = "WhatHuh.exe",
                IconFileName = "appIcon.ico",
                UseLauncher = true,
                GithubOwner = "Echostorm44",
                GithubRepo = "WhatHuh",
                ZipName = "WhatHuhCPU.zip"
            };
            var sample2 = new DeployProfile
            {
                ID = 2,
                Title = "WhatHuh CUDA",
                DefaultInstallFolderName = "WhatHuh",
                ExeFileName = "WhatHuh.exe",
                IconFileName = "appIcon.ico",
                UseLauncher = true,
                GithubOwner = "Echostorm44",
                GithubRepo = "WhatHuh",
                ZipName = "WhatHuhCUDA.zip"
            };
            List<DeployProfile> sampleList = [sample, sample2];
            var fooo = JsonSerializer.Serialize(sampleList);
            File.WriteAllText(vm.SettingsPath, fooo);
        }
        var savedProfiles = JsonSerializer.Deserialize<DeployProfile[]>(File.ReadAllText(vm.SettingsPath));
        foreach(var item in savedProfiles)
        {
            vm.Profiles.Add(item);
        }
        InitializeComponent();
        DataContext = vm;
    }

    private void btnAddNewProfile_Click(object sender, RoutedEventArgs e)
    {
        var moo = new DeployProfile() { ID = 3, Title = "New Profile" };
        vm.Profiles.Add(moo);
        lvProfiles.SelectedItem = moo;
    }

    private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var roo = e.Source;
        var foo = e.AddedItems;
    }

    private void btnLocalPubPath_Click(object sender, RoutedEventArgs e)
    {
        var foo = new OpenFolderDialog();
        if(foo.ShowDialog().Value)
        {
            vm.ActiveProfile.LocalPublishPath = foo.FolderName;
        }
    }

    private void btnResultsPath_Click(object sender, RoutedEventArgs e)
    {
        var foo = new OpenFolderDialog();
        if(foo.ShowDialog().Value)
        {
            vm.ActiveProfile.ResultsPath = foo.FolderName;
        }
    }

    private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
    {
        File.WriteAllText(vm.SettingsPath, JsonSerializer.Serialize(vm.Profiles));
    }

    // TODO add a delete button, something for the EULA, the backdrop && the icon.

    private void btnGenerateResults_Click(object sender, RoutedEventArgs e)
    {
        // So we need to a bunch of things here.
        // 1. Make a config.json file that contains the settings for the installer && launcher
        //    copy that into the local publish path so it gets picked up into the zip in step 3 && 4
        // 2. Copy launcher.exe into the local publish path so it gets picked up into the zip in step 3 && 4
        // 3. Make a zip file out of the local publish path that does not contain the setup files
        //    this is for the auto update stuff. Use the ZipName property for the name of the zip
        //    then copy the zip into the ResultsPath folder
        // 4. Copy the setup file from the SetupFiles folder into the local publish path
        // 4. Make a zip file called Payload.zip out of the local publish path that also contains the
        //    setup files from SetupFiles && then copy that zip into the InstallerFiles 
        //    then execute this command in the InstallFiles folder
        //    dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -f net8.0 -o ./publishCL/ --sc true
        //    then copy the contents of the publishCL folder into the ResultsPath which should just be installer.exe
        //    Then we can delete the publishCL folder and the Payload.zip
    }
}

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<DeployProfile> Profiles { get; set; }
    public string SettingsPath { get; set; }

    DeployProfile activeProfile;

    public DeployProfile ActiveProfile
    {
        get => activeProfile;
        set
        {
            activeProfile = value;
            NotifyPropertyChanged();
        }
    }

    public MainViewModel()
    {
        Profiles = [];
        ActiveProfile = new DeployProfile();
    }

    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class DeployProfile : INotifyPropertyChanged
{
    public int ID { get; set; }
    string title;
    public string Title
    {
        get => title;
        set
        {
            if(title == value)
            {
                return;
            }

            title = value;
            NotifyPropertyChanged();
        }
    }
    public string DefaultInstallFolderName { get; set; }
    public string ExeFileName { get; set; }
    public string IconFileName { get; set; }
    public bool UseLauncher { get; set; }
    public string GithubOwner { get; set; }
    public string GithubRepo { get; set; }
    public string ZipName { get; set; }
    string localPublishPath;
    public string LocalPublishPath
    {
        get => localPublishPath;
        set
        {
            if(localPublishPath == value)
            {
                return;
            }

            localPublishPath = value;
            NotifyPropertyChanged();
        }
    }
    string resultsPath;
    public string ResultsPath
    {
        get => resultsPath;
        set
        {
            if(resultsPath == value)
            {
                return;
            }

            resultsPath = value;
            NotifyPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}