using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using System.Xml.Linq;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class RunItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();

            List<string> files = new List<string>();
            foreach (string arg in Environment.GetCommandLineArgs().Skip(1).Concat(new[] { "." }))
            {
                if (Directory.Exists(arg))
                    files.AddRange(Directory.EnumerateFiles(arg, "*.xaml"));

                else if (File.Exists(arg))
                    files.Add(arg);
            }

            foreach (string file in files)
            {
                XDocument xml;
                try
                {
                    xml = XDocument.Load(file);

                    if (xml.Root == null)
                        throw new XmlException("File does not contain any elements.");
                }
                catch (XmlException e) { AddInvalidFileEntry(file, e); continue; }

                string title = xml.Root.Attribute("Title")?.Value ?? Path.GetFileName(file);
                string description = xml.Root.Attribute("Description")?.Value;

                AddFileEntry(file, title, description);
            }
        }

        private void AddFileEntry(string file, string title, string description)
        {
            ListBoxItem entry = new ListBoxItem();
            entry.Content = new RunItem { Title = title, Description = description };
            entry.Tag = file;
            _menu.Items.Insert(_menu.Items.Count - 2, entry);
        }

        private void AddInvalidFileEntry(string file, XmlException e)
        {
            ListBoxItem entry = new ListBoxItem();
            entry.Content = Path.GetFileName(file) + " is not a valid XML file";
            entry.IsEnabled = false;
            _menu.Items.Insert(_menu.Items.Count - 2, entry);
        }

        private void OnMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Contains(_menuSave))
                _menuSave.Content = "Save data to flash drive";
        }

        private void OnMenuKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                if (e.OriginalSource is ListBoxItem menu)
            {

                if (menu == _menuExit)
                    Application.Current.Shutdown();

                else if (menu == _menuSave)
                    _ = Save();

                else if (menu is ListBoxItem { Tag: string file })
                {
                    MainWindow study = new MainWindow();
                    study.Show();
                    _ = study.Load(file);
                }
            }
        }

        private bool _saving;
        private async Task Save()
        {
            if (_saving)
                return;

            DriveInfo[] drives = DriveInfo.GetDrives();
            DriveInfo removable = drives.FirstOrDefault(d => d.DriveType == DriveType.Removable);

            if (removable == null)
            {
                _menuSave.Content = "No removable drives found!";
                return;
            }

            try
            {
                _saving = true;
                string path = Path.Combine(removable.RootDirectory.FullName, $"RunStudy {DateTime.Now:yyyyMMdd-HHmmss}");
                DirectoryInfo targetDir = Directory.CreateDirectory(path);

                string[] files = Directory.GetFiles(".", "Log*.txt");

                string label = string.IsNullOrWhiteSpace(removable.VolumeLabel) ? removable.Name : removable.VolumeLabel;
                _menuSave.Content = $"Copying to {label}...";

                await Task.Run(delegate
                {
                    foreach (string file in files)
                        File.Copy(file, Path.Combine(targetDir.FullName, Path.GetFileName(file)));
                });
                _menuSave.Content = $"Saved {files.Length} files to " + path;
            }
            catch (Exception e)
            {
                _menuSave.Content = "Error: " + e.Message;
            }
            finally
            {
                _saving = false;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _menu.Focus();
            _menu.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)); // select first element, focusing listboxitem does not seem to be working
        }
    }
}