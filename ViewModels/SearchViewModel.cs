using System;
using System.Collections.ObjectModel;
using System.Windows;
using Tema_MAP.Models;

namespace Tema_MAP.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private string _findText;
        private string _replaceText;
        private bool _isCurrentTabSelected = true; 
        private bool _isAllTabsSelected;

        private MainViewModel _mainViewModel;

        public string FindText { get => _findText; set { _findText = value; OnPropertyChanged(); } }
        public string ReplaceText { get => _replaceText; set { _replaceText = value; OnPropertyChanged(); } }
        public bool IsCurrentTabSelected { get => _isCurrentTabSelected; set { _isCurrentTabSelected = value; OnPropertyChanged(); } }
        public bool IsAllTabsSelected { get => _isAllTabsSelected; set { _isAllTabsSelected = value; OnPropertyChanged(); } }

        public RelayCommand FindCommand { get; }
        public RelayCommand ReplaceCommand { get; }
        public RelayCommand ReplaceAllCommand { get; }

        public SearchViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            FindCommand = new RelayCommand(ExecuteFind);
            ReplaceCommand = new RelayCommand(ExecuteReplace);
            ReplaceAllCommand = new RelayCommand(ExecuteReplaceAll);
        }

        private void ExecuteFind(object parameter)
        {
            if (string.IsNullOrEmpty(FindText)) return;
            MessageBox.Show($"Căutare declanșată pentru: '{FindText}'.");
        }

        private void ExecuteReplace(object parameter)
        {
            if (string.IsNullOrEmpty(FindText) || _mainViewModel.SelectedDocument == null) return;

            if (IsCurrentTabSelected)
            {
                var doc = _mainViewModel.SelectedDocument;
                if (doc.Content != null && doc.Content.Contains(FindText))
                {
                    int index = doc.Content.IndexOf(FindText);
                    doc.Content = doc.Content.Remove(index, FindText.Length).Insert(index, ReplaceText ?? "");
                }
            }
        }

        private void ExecuteReplaceAll(object parameter)
        {
            if (string.IsNullOrEmpty(FindText)) return;

            if (IsCurrentTabSelected)
            {
                if (_mainViewModel.SelectedDocument != null && _mainViewModel.SelectedDocument.Content != null)
                {
                    _mainViewModel.SelectedDocument.Content = _mainViewModel.SelectedDocument.Content.Replace(FindText, ReplaceText ?? "");
                }
            }
            else if (IsAllTabsSelected)
            {  
                foreach (var doc in _mainViewModel.Documents) 
                {
                    if (doc.Content != null)
                    {
                        doc.Content = doc.Content.Replace(FindText, ReplaceText ?? "");
                    }
                }
            }
        }
    }
}