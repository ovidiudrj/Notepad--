using System;
using System.Collections.Generic;
using System.Text;
using Tema_MAP.ViewModels;

namespace Tema_MAP.Models
{
    public class DocumentModel : ViewModelBase
    {
        private string _fileName;
        private string _content;
        private bool _isDirty;
        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set { _filePath = value; OnPropertyChanged(); }
        }

        public string FileName
        {
            get => _fileName;
            set { _fileName = value; OnPropertyChanged(); }
        }

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged();
                IsDirty = true; 
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            set { _isDirty = value; OnPropertyChanged(); }
        }
    }
}
