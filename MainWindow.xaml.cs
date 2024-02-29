using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        try
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
        catch(Exception ex)
        {
            lblResults.Text = ex.ToString();
        }
    }

    private void btnAddNewProfile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var newID = vm.Profiles.Max(x => x.ID) + 1;
            var moo = new DeployProfile() { ID = newID, Title = "New Profile" };
            vm.Profiles.Add(moo);
            lvProfiles.SelectedItem = moo;
        }
        catch(Exception ex)
        {
            lblResults.Text = ex.ToString();
        }
    }

    private void btnLocalPubPath_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var foo = new OpenFolderDialog();
            if(foo.ShowDialog().Value)
            {
                vm.ActiveProfile.LocalPublishPath = foo.FolderName;
            }
        }
        catch(Exception ex)
        {
            lblResults.Text = ex.ToString();
        }
    }

    private void btnResultsPath_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var foo = new OpenFolderDialog();
            if(foo.ShowDialog().Value)
            {
                vm.ActiveProfile.ResultsPath = foo.FolderName;
            }
        }
        catch(Exception ex)
        {
            lblResults.Text = ex.ToString();
        }
    }

    private void btnBackDropPath_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var foo = new OpenFileDialog();
            if(foo.ShowDialog().Value && File.Exists(foo.FileName))
            {
                vm.ActiveProfile.BackdropPath = foo.FileName;
            }
        }
        catch(Exception ex)
        {
            lblResults.Text = ex.ToString();
        }
    }

    private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
    {
        SaveProfiles();
    }

    private void SaveProfiles()
    {
        try
        {
            File.WriteAllText(vm.SettingsPath, JsonSerializer.Serialize(vm.Profiles));
            RootSnackbar.Show("Saved!", "Settings Saved To File", Wpf.Ui.Common.SymbolRegular.Warning16);
        }
        catch(Exception ex)
        {
            lblResults.Text = ex.ToString();
        }
    }

    private void btnDeleteProfile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if(lvProfiles.SelectedItem == null)
            {
                return;
            }
            var profile = (DeployProfile)lvProfiles.SelectedItem;
            vm.Profiles.Remove(profile);
            SaveProfiles();
        }
        catch(Exception ex)
        {
            lblResults.Text = ex.ToString();
        }
    }

    // TODO add something for the EULA.

    string GenerateResults(IProgress<string> status)
    {
        try
        {
            status.Report("Copying icon, config and backdrop files");
            var publishIconPath = Path.Combine(vm.ActiveProfile.LocalPublishPath, vm.ActiveProfile.IconFileName);
            var installerIconPath = Path.Combine(AppContext.BaseDirectory, "InstallerFiles\\icon.ico");
            var setupIconPath = Path.Combine(AppContext.BaseDirectory, "SetupFiles\\appIcon.ico");
            File.Copy(publishIconPath, installerIconPath, true);
            File.Copy(publishIconPath, setupIconPath, true);

            var backdropPath = Path.Combine(AppContext.BaseDirectory, "SetupFiles\\backdrop.png");
            File.Copy(vm.ActiveProfile.BackdropPath, backdropPath, true);

            Directory.CreateDirectory(vm.ActiveProfile.ResultsPath);
            var config = new SetupSettings
            {
                Title = vm.ActiveProfile.Title,
                DefaultInstallFolderName = vm.ActiveProfile.DefaultInstallFolderName,
                ExeFileName = vm.ActiveProfile.ExeFileName,
                IconFileName = vm.ActiveProfile.IconFileName,
                UseLauncher = vm.ActiveProfile.UseLauncher,
                GithubOwner = vm.ActiveProfile.GithubOwner,
                GithubRepo = vm.ActiveProfile.GithubRepo,
                ZipName = vm.ActiveProfile.ZipName
            };
            File.WriteAllText(vm.ActiveProfile.LocalPublishPath + "\\config.json", JsonSerializer.Serialize(config));

            status.Report("Copying launcher file");
            var launcherPath = Path.Combine(AppContext.BaseDirectory, "launcher.exe");
            var launcherDest = Path.Combine(vm.ActiveProfile.LocalPublishPath, "launcher.exe");
            File.Copy(launcherPath, launcherDest, true);

            var zipPath = Path.Combine(vm.ActiveProfile.ResultsPath, vm.ActiveProfile.ZipName);
            File.Delete(zipPath);
            status.Report("Creating zip file");
            ZipFile.CreateFromDirectory(vm.ActiveProfile.LocalPublishPath, zipPath, CompressionLevel.Optimal, false);

            status.Report("Copying setup files");
            var setupFilesPath = Path.Combine(AppContext.BaseDirectory, "SetupFiles");
            var sourceDir = new DirectoryInfo(setupFilesPath);
            var files = sourceDir.GetFiles();
            foreach(FileInfo file in files)
            {
                string targetFilePath = Path.Combine(vm.ActiveProfile.LocalPublishPath, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            var payloadZipPath = Path.Combine(AppContext.BaseDirectory, "InstallerFiles\\Payload.zip");
            File.Delete(payloadZipPath);
            status.Report("Creating payload zip file");
            ZipFile.CreateFromDirectory(vm.ActiveProfile.LocalPublishPath, payloadZipPath, CompressionLevel.Optimal, false);

            status.Report("Publishing installer");
            var installerPath = Path.Combine(vm.ActiveProfile.ResultsPath, "installer.exe");
            var installerFilesPath = Path.Combine(AppContext.BaseDirectory, "InstallerFiles");
            var installerPublishPath = Path.Combine(AppContext.BaseDirectory, "InstallerFiles\\publishCL");
            var installerExePath = Path.Combine(AppContext.BaseDirectory, "InstallerFiles\\publishCL\\installer.exe");
            var publishCmd = "dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -f net8.0 -o ./publishCL/ --sc true";
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {publishCmd}",
                    WorkingDirectory = installerFilesPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();

            status.Report("Moving installer");
            File.Move(installerExePath, installerPath, true);
            status.Report("Cleaning up");
            Directory.Delete(installerPublishPath, true);
            File.Delete(payloadZipPath);

            var procOpenWeb = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = $"https://www.github.com/{vm.ActiveProfile.GithubOwner}/{vm.ActiveProfile.GithubRepo}/releases",
                    UseShellExecute = true,
                    CreateNoWindow = true
                }
            };
            procOpenWeb.Start();

            Process.Start(Environment.GetEnvironmentVariable("WINDIR") +
                @"\explorer.exe", vm.ActiveProfile.ResultsPath);
            status.Report("Done");
        }
        catch(Exception ex)
        {
            return ex.ToString();
        }
        return "Success";
    }

    private void btnGenerateResults_Click(object sender, RoutedEventArgs e)
    {
        if(!Directory.Exists(vm.ActiveProfile.LocalPublishPath))
        {
            lblStatus.Text = "Local Publish Path does not exist";
            return;
        }
        if(!File.Exists(vm.ActiveProfile.LocalPublishPath + "\\" + vm.ActiveProfile.ExeFileName))
        {
            lblStatus.Text = "Exe file does not exist";
            return;
        }
        var publishIconPath = Path.Combine(vm.ActiveProfile.LocalPublishPath, vm.ActiveProfile.IconFileName);
        if(!File.Exists(publishIconPath) || !vm.ActiveProfile.IconFileName.EndsWith("ico"))
        {
            lblStatus.Text = "Icon file does not exist or does not end with ico";
            return;
        }
        if(!File.Exists(vm.ActiveProfile.BackdropPath))
        {
            lblResults.Text = "Backdrop file does not exist";
            return;
        }
        progRing.Visibility = Visibility.Visible;
        btnAddNewProfile.IsEnabled = false;
        btnSaveChanges.IsEnabled = false;
        btnLocalPubPath.IsEnabled = false;
        btnResultsPath.IsEnabled = false;
        btnBackDropPath.IsEnabled = false;
        btnGenerateResults.IsEnabled = false;
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
        // 5. We've now got both files needed to upload to the github release page, so
        //    pop open the github page for the repo release page && also open the ResultsPath folder in explorer
        Progress<string> statusUpdate = new Progress<string>();
        statusUpdate.ProgressChanged += (a, b) =>
        {
            lblStatus.Text = b;
        };
        IProgress<string> status = statusUpdate;

        Task.Factory.StartNew(() =>
        {
            return GenerateResults(status);
        }).ContinueWith(a =>
        {
            if(a.Result != "Success")
            {
                lblResults.Text = a.Result;
            }
            progRing.Visibility = Visibility.Collapsed;
            btnAddNewProfile.IsEnabled = true;
            btnSaveChanges.IsEnabled = true;
            btnLocalPubPath.IsEnabled = true;
            btnResultsPath.IsEnabled = true;
            btnBackDropPath.IsEnabled = true;
            btnGenerateResults.IsEnabled = true;
        }, TaskScheduler.FromCurrentSynchronizationContext());
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
    string backdropPath;
    public string BackdropPath
    {
        get => backdropPath;
        set
        {
            if(backdropPath == value)
            {
                return;
            }

            backdropPath = value;
            NotifyPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class SetupSettings
{
    public string Title { get; set; }
    public string DefaultInstallFolderName { get; set; }
    public string ExeFileName { get; set; }
    public string IconFileName { get; set; }
    public bool UseLauncher { get; set; }
    public string GithubOwner { get; set; }
    public string GithubRepo { get; set; }
    public string ZipName { get; set; }
}