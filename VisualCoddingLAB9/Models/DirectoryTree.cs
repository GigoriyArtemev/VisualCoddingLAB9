using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

namespace VisualCoddingLAB9.Models
{
    public class DirectoryTree : ReactiveObject
    {
        #region FIELDS

        #region PRIVATE

        private string __path;
        private string __name;
        private FileSystemWatcher? __watcher;
        private ObservableCollection<DirectoryTree>? __children;
        private bool __isExpanded;

        #endregion

        #region PUBLIC

        public bool IsChecked { get; set; }
        public DirectoryTree(string path,
        bool isRoot = false)
        {
            __path = path;
            if (!path.Contains(".jpg"))
            {
                var t = new DirectoryInfo(path);
                __name = t.Name;
            }
            else
            {
                __name = isRoot ? path : System.IO.Path.GetFileName(Path);
            }
            __isExpanded = isRoot;
        }
        public string Path
        {
            get => __path;
            private set => this.RaiseAndSetIfChanged(ref __path, value);
        }
        public string Name
        {
            get => __name;
            private set => this.RaiseAndSetIfChanged(ref __name, value);
        }
        public bool IsExpanded
        {
            get => __isExpanded;
            set => this.RaiseAndSetIfChanged(ref __isExpanded, value);
        }
        public bool IsDirectory { get; }
        public IReadOnlyList<DirectoryTree> Children => __children ??= LoadChildren();

        #endregion

        #endregion

        #region METHODS

        #region PRIVATE

        private ObservableCollection<DirectoryTree> LoadChildren()
        {
            var options = new EnumerationOptions { IgnoreInaccessible = true };
            var result = new ObservableCollection<DirectoryTree>();

            foreach (var d in Directory.EnumerateDirectories(Path, "*", options))
            {
                result.Add(new DirectoryTree(d, true));
            }

            foreach (var f in Directory.EnumerateFiles(Path, "*.jpg", options))
            {
                result.Add(new DirectoryTree(f, false));
            }
            __watcher = new FileSystemWatcher
            {
                Path = Path,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite,
            };
            __watcher.EnableRaisingEvents = true;
            return result;
        }

        #endregion

        #endregion
    }
}
