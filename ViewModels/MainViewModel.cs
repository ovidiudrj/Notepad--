using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Tema_MAP.Models;

namespace Tema_MAP.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private int _newFileCounter = 1;
        private DocumentModel _selectedDocument;
        public ObservableCollection<DocumentModel> Documents { get; set; } 
        public DocumentModel SelectedDocument 
        {
            get => _selectedDocument;
            set { _selectedDocument = value; OnPropertyChanged(); }
        }
        public ObservableCollection<NodeModel> Workspace { get; set; }


        public RelayCommand NewFileCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand SaveFileCommand { get; set; }
        public RelayCommand SaveFileAsCommand { get; set; }
        public RelayCommand CloseFileCommand { get; set; }
        public RelayCommand CloseAllFilesCommand { get; set; }
        public RelayCommand OpenWorkspaceCommand { get; set; }
        public RelayCommand OpenTreeFileCommand { get; set; }


        private string _copiedFolderPath;
        public RelayCommand NewFileInTreeCommand { get; set; }
        public RelayCommand CopyPathCommand { get; set; }
        public RelayCommand CopyFolderCommand { get; set; }
        public RelayCommand PasteFolderCommand { get; set; }
        public RelayCommand AboutCommand { get; set; }
        
        private Visibility _folderExplorerVisibility = Visibility.Visible;
        public Visibility FolderExplorerVisibility
        {
            get => _folderExplorerVisibility;
            set { _folderExplorerVisibility = value; OnPropertyChanged(); }
        }

        public RelayCommand ViewStandardCommand { get; set; }
        public RelayCommand ViewFolderExplorerCommand { get; set; }
        public RelayCommand OpenSearchCommand { get; }
        public MainViewModel()
        {
            Documents = new ObservableCollection<DocumentModel>();
           
            NewFileCommand = new RelayCommand(o => AddNewFile());
            OpenFileCommand = new RelayCommand(o => OpenFile());           
            SaveFileCommand = new RelayCommand(o => SaveDocument(SelectedDocument));
            SaveFileAsCommand = new RelayCommand(o => SaveFileAs(SelectedDocument));
            CloseFileCommand = new RelayCommand(o => CloseFile());
            CloseAllFilesCommand = new RelayCommand(o => CloseAllFiles());

            AddNewFile();

            Workspace = new ObservableCollection<NodeModel>();

            OpenWorkspaceCommand = new RelayCommand(o => OpenWorkspace());
            OpenTreeFileCommand = new RelayCommand(OpenTreeFile);
            ViewStandardCommand = new RelayCommand(o => FolderExplorerVisibility = Visibility.Collapsed);
            ViewFolderExplorerCommand = new RelayCommand(o => FolderExplorerVisibility = Visibility.Visible);
            NewFileInTreeCommand = new RelayCommand(NewFileInTree);
            CopyPathCommand = new RelayCommand(CopyPath);
            CopyFolderCommand = new RelayCommand(CopyFolder);
            PasteFolderCommand = new RelayCommand(PasteFolder, CanPasteFolder);
            AboutCommand = new RelayCommand(o => { new AboutWindow().ShowDialog(); });
            OpenSearchCommand = new RelayCommand(ExecuteOpenSearch);
        }

        private void AddNewFile()
        {
            var newDoc = new DocumentModel
            {
                FileName = $"File {_newFileCounter++}",
                Content = "",
                IsDirty = false,
                FilePath = string.Empty 
            };
            Documents.Add(newDoc);
            SelectedDocument = newDoc;
        }

        private void OpenFile()
        {  
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {               
                string content = File.ReadAllText(openFileDialog.FileName);

                var newDoc = new DocumentModel
                {
                    FileName = Path.GetFileName(openFileDialog.FileName), 
                    FilePath = openFileDialog.FileName, 
                    Content = content,
                    IsDirty = false
                };
                Documents.Add(newDoc);
                SelectedDocument = newDoc; 
            }
        }

        private void SaveDocument(DocumentModel doc)
        {
            if (doc == null) return; 

            if (string.IsNullOrEmpty(doc.FilePath))
            {
                SaveFileAs(doc);
            }
            else
            {                
                File.WriteAllText(doc.FilePath, doc.Content);
                doc.IsDirty = false; 
            }
        }

        private void SaveFileAs(DocumentModel doc)
        {
            if (doc == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";  
            saveFileDialog.FileName = doc.FileName;

            if (saveFileDialog.ShowDialog() == true)
            {
                doc.FilePath = saveFileDialog.FileName;
                doc.FileName = Path.GetFileName(saveFileDialog.FileName); 

                File.WriteAllText(doc.FilePath, doc.Content);
                doc.IsDirty = false;
            }
        }

        private bool AskToSave(DocumentModel doc)
        {
            if (!doc.IsDirty) return true;

            SelectedDocument = doc; 

            var result = MessageBox.Show($"Salvezi modificarile pentru {doc.FileName}?", "WARNING", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                SaveDocument(doc);
                return !doc.IsDirty;
            }
            else if (result == MessageBoxResult.No)
            {
                return true; 
            }
            return false; 
        }

        private void CloseFile()
        {
            if (SelectedDocument == null) return;

            if (AskToSave(SelectedDocument))
            {
                Documents.Remove(SelectedDocument);
            }
        }

        private void CloseAllFiles()
        {
            for (int i = Documents.Count - 1; i >= 0; i--)
            {
                var doc = Documents[i];
                if (AskToSave(doc))
                {
                    Documents.Remove(doc);
                }
                else
                {
                    break; 
                }
            }
        }

        private void OpenWorkspace()
        {  
            var dialog = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "nume_fals123", 
                Title = "Selecteaza folderul!"
            };

            if (dialog.ShowDialog() == true)
            {   
                string folderPath = Path.GetDirectoryName(dialog.FileName);

                Workspace.Clear();

                var rootNode = new NodeModel
                {
                    Name = Path.GetFileName(folderPath),
                    FullPath = folderPath,
                    IsFile = false
                };
                rootNode.Children.Add(new NodeModel { Name = "Loading..." }); 

                Workspace.Add(rootNode);
                rootNode.IsExpanded = true; 
            }
        }

        private void OpenTreeFile(object param)
        {
            if (param is string filePath && File.Exists(filePath))
            {  
                foreach (var doc in Documents)
                {
                    if (doc.FilePath == filePath)
                    {
                        SelectedDocument = doc;
                        return;
                    }
                }

                string content = File.ReadAllText(filePath);
                var newDoc = new DocumentModel
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    Content = content,
                    IsDirty = false
                };
                Documents.Add(newDoc);
                SelectedDocument = newDoc;
            }
        }

        private void NewFileInTree(object param)
        {
            if (param is NodeModel node && !node.IsFile)
            {
                string baseName = "new_file.txt";
                string fullPath = Path.Combine(node.FullPath, baseName);
                int count = 1;

                while (File.Exists(fullPath))
                {
                    fullPath = Path.Combine(node.FullPath, $"new_file_{count}.txt");
                    count++;
                }
                File.WriteAllText(fullPath, "");
                node.LoadChildren();
            }
        }

        private void CopyPath(object param)
        {
            if (param is NodeModel node)
            {
                Clipboard.SetText(node.FullPath);
            }
        }

        private void CopyFolder(object param)
        {
            if (param is NodeModel node && !node.IsFile)
            {
                _copiedFolderPath = node.FullPath;
            }
        }

        private bool CanPasteFolder(object param)
        {
            return !string.IsNullOrEmpty(_copiedFolderPath) && Directory.Exists(_copiedFolderPath);
        }

        private void PasteFolder(object param)
        {
            if (param is NodeModel targetNode && !targetNode.IsFile)
            {
                string sourceDirName = new DirectoryInfo(_copiedFolderPath).Name;
                string targetPath = Path.Combine(targetNode.FullPath, sourceDirName);

                if (!Directory.Exists(targetPath))
                {
                    CopyDirectoryRecursively(_copiedFolderPath, targetPath);
                    targetNode.LoadChildren(); 
                }
                else
                {
                    MessageBox.Show("Un folder cu acest nume exista deja in aceasta locatie!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void CopyDirectoryRecursively(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string dest = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, dest);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string dest = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectoryRecursively(dir, dest);
            }
        }

        private void ExecuteOpenSearch(object parameter)
        {           
            var searchVM = new SearchViewModel(this);
            var searchWindow = new Views.SearchWindow 
            {
                DataContext = searchVM,
                Owner = Application.Current.MainWindow 
            };
            searchWindow.Show();
        }
    }
}