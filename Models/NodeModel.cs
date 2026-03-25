using System.Collections.ObjectModel;
using System.IO;
using Tema_MAP.ViewModels;

namespace Tema_MAP.Models
{
    public class NodeModel : ViewModelBase
    {
        private bool _isExpanded;
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsFile { get; set; }
        public ObservableCollection<NodeModel> Children { get; set; }

        public NodeModel()
        {
            Children = new ObservableCollection<NodeModel>();
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();

                if (_isExpanded && Children.Count == 1 && Children[0].Name == "Loading...")
                {
                    LoadChildren();
                }
            }
        }

        public void LoadChildren()
        {
            Children.Clear();
            if (IsFile) return;

            try
            {
                foreach (var dir in Directory.GetDirectories(FullPath))
                {
                    var dirNode = new NodeModel { Name = Path.GetFileName(dir), FullPath = dir, IsFile = false };
                    
                    dirNode.Children.Add(new NodeModel { Name = "Loading..." });
                    Children.Add(dirNode);
                }

                foreach (var file in Directory.GetFiles(FullPath))
                {
                    Children.Add(new NodeModel { Name = Path.GetFileName(file), FullPath = file, IsFile = true });
                }
            }
            catch{}
        }
    }
}