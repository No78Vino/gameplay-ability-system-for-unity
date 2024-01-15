#if UNITY_EDITOR
namespace GAS.Editor.General
{
    using System;
    using System.Collections.Generic;
    
    public class DirectoryInfo
    {
        public string RootDirectory { get; }

        public DirectoryInfo(string rootMenuName, string rootDirectory, string directory, Type assetType, bool isRoot)
        {
            RootDirectory = rootDirectory;
            Directory = directory;
            SubDirectory = new List<string>();
            AssetType = assetType;
            Root = isRoot;

            var dirs = directory.Replace("\\", "/");
            MenuName = dirs.Replace(RootDirectory + '/', "");
            MenuName = $"{rootMenuName}/{MenuName}";

            GetAllSubDir(Directory, SubDirectory);
        }

        public bool Root { get; }

        public string MenuName { get; }

        public Type AssetType { get; }

        public string Directory { get; }

        public List<string> SubDirectory { get; }

        private void GetAllSubDir(string path, List<string> subDirs)
        {
            var dirs = System.IO.Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                subDirs.Add(dir);
                GetAllSubDir(dir, subDirs);
            }
        }
    }
}
#endif