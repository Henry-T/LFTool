using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using NETExtLib;

namespace SystemReinstallTool
{
    class SystemReinstaller
    {
        private static void processChecking(string backupPath, bool isBackup)
        {
            if (!isBackup)
                return;

            string userProfileDirPath = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            DirectoryInfo userProfileDirInfo = new DirectoryInfo(userProfileDirPath);

            string defaultVBoxUserHomeDir = Environment.ExpandEnvironmentVariables("%USERPROFILE%\\.VirtualBox");
            if (Directory.Exists(defaultVBoxUserHomeDir))
            {
                Console.WriteLine("please move {0} to {1} with envrionment variable name: VBOX_USER_HOME", defaultVBoxUserHomeDir, @"D:\Sync\Sync\ProgramConfig\VBOX_USER_HOME");
            }

            var androidStudioDirs = userProfileDirInfo.EnumerateDirectories(".AndroidStudio*", SearchOption.TopDirectoryOnly);
            foreach(var androidStudioDir in androidStudioDirs)
            {
                Console.WriteLine("please move {0} to {1}, and config 'idea.config.path' in <android_studio_dir>/bin/idea.properties");
            }

            string gradleDir = Path.Combine(userProfileDirInfo.FullName, ".gradle");
            if (Directory.Exists(gradleDir))
            {
                Console.WriteLine("please move {0} to {1} with envrionment variable name: GRADLE_USER_HOME", gradleDir, @"D:\Development\GradleUserHome");
            }

            Regex reg = new Regex(@"Visual Studio[\d ]+\Settings\CurrentSettings.vssettings");
            var vssettingsFileInfos = userProfileDirInfo.GetFiles("*", SearchOption.AllDirectories).Where(path => reg.IsMatch(path.FullName));
            foreach(var vssettingsFileInfo in vssettingsFileInfos)
            {
                Console.WriteLine("please move {0} to {1}. Then set Tools > Options > Environment > Import and Export Settings > Automatically save my settings to this file: of Visual Studio", vssettingsFileInfo.FullName, @"D:\Sync\Sync\ProgramConfig\Visual Studio <version>\Settings\CurrentSettings.vssettings");
            }
            
        }

        private static void processPowerShellProfile(string backupDirPath, bool isBackup)
        {
            string powerShellProfilePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Documents\WindowPowerShell\Microsoft.PowerShell_profile.ps1");
            string backupFilePath = Path.Combine(backupDirPath, "Microsoft.PowerShell_profile.ps1");
            if (isBackup && File.Exists(powerShellProfilePath))
            {
                File.Copy(powerShellProfilePath, backupFilePath);
            }
            else if (!isBackup && File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, powerShellProfilePath);
            }
        }

        private static void processHosts(string backupDirPath, bool isBackup)
        {
            string hostsPath = Environment.ExpandEnvironmentVariables(@"C:\Windows\System32\drivers\etc\hosts");
            string backupFilePath = Path.Combine(backupDirPath, "hosts");
            if (isBackup && File.Exists(hostsPath))
            {
                File.Copy(hostsPath, backupFilePath);
            }
            else if (!isBackup && File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, hostsPath);
            }
        }

        private static void processEnvironmentVariables(string backupDirPath, bool isBackup)
        {
            string systemVariableBackupFilePath = Path.Combine(backupDirPath, "SystemVariables.bat");
            string userVariableBackupFilePath = Path.Combine(backupDirPath, "UserVariables.bat");
            if (isBackup)
            {
                RegistryUtil.Export(systemVariableBackupFilePath, @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                RegistryUtil.Export(userVariableBackupFilePath, @"HKEY_CURRENT_USER\Environment");
            }
            else
            {
                if (File.Exists(systemVariableBackupFilePath))
                    Util.StartProcessToEnd(systemVariableBackupFilePath, "");
                if (File.Exists(userVariableBackupFilePath))
                    Util.StartProcessToEnd(userVariableBackupFilePath, "");
            }
        }

        private static void processTaskScheduler(string backupDirPath, bool isBackup)
        {
            string myTasksPath = "C:/Windows/System32/Tasks/MyTask";
            string myTasksBackupPath = Path.Combine(backupDirPath, "MyTask");
            if (isBackup)
            {
                Util.RoboCopy(myTasksPath, myTasksBackupPath);
            }
            else
            {
                Util.RoboCopy(myTasksBackupPath, myTasksPath);
            }
        }

        private static void processFilezilla(string backupDirPath, bool isBackup)
        {
            string filezillaSettingsPath = Path.Combine(backupDirPath, "filezilla.xml");
            if (isBackup)
            {
                Util.StartProcessToEnd("filezilla.exe", "--export " + filezillaSettingsPath);
            }
            else
            {
                Util.StartProcessToEnd("filezilla.exe", "--import " + filezillaSettingsPath);
            }
        }

        public static void ProcessAll(string backupDirPath, bool isBackup)
        {
            processChecking(backupDirPath, isBackup);
            processPowerShellProfile(backupDirPath, isBackup);
            processHosts(backupDirPath, isBackup);
            processEnvironmentVariables(backupDirPath, isBackup);
            processTaskScheduler(backupDirPath, isBackup);
            processFilezilla(backupDirPath, isBackup);
        }
    }
}
