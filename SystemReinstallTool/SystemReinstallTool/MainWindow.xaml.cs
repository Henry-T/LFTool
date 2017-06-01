using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Forms;

namespace SystemReinstallTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string backupPath;

        private void btnStartBackup_Click(object sender, RoutedEventArgs e)
        {
            SystemReinstaller.ProcessAll(backupPath, true);
        }

        private void btnStartRestore_Click(object sender, RoutedEventArgs e)
        {
            SystemReinstaller.ProcessAll(backupPath, false);
        }

        private void btnChooseBackupPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                tbBackupPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void tbBackupPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            backupPath = tbBackupPath.Text;
        }
    }
}
