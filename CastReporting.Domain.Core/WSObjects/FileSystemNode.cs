using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace CastReporting.Domain
{
    public abstract class FileSystemNode
    {
        public FileSystemNode(FileSystemInfo info) {
            _Info = info;
        }

        protected readonly FileSystemInfo _Info;

        public string Name => _Info.Name;
        public string FullName => _Info.FullName;
        public string Extension => _Info.Extension;

        public abstract IList<FileSystemNode> Children { get; }

        public IEnumerable<T> AllChildren<T>() where T : FileSystemNode {
            foreach (var child in Children) {
                if (child is T) {
                    yield return (T)child;
                }
                foreach (var grandChild in child.AllChildren<T>()) {
                    yield return grandChild;
                }
            }
        }
    }

    public class FileLeaf : FileSystemNode
    {
        public FileLeaf(FileInfo file) : base(file) {
        }

        static readonly IList<FileSystemNode> _None = new ReadOnlyCollection<FileSystemNode>(new List<FileSystemNode>());
        public override IList<FileSystemNode> Children => _None;

        public FileInfo FileInfo => _Info as FileInfo;
    }

    public class DirNode : FileSystemNode
    {
        public DirNode(DirectoryInfo dir) : base(dir) {
        }

        private readonly List<FileSystemNode> _Children = new List<FileSystemNode>();
        public override IList<FileSystemNode> Children => _Children;

        public DirectoryInfo DirectoryInfo => _Info as DirectoryInfo;
    }
}