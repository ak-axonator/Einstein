using System;
using System.Collections.Generic;
using System.IO;

namespace WebMagic
{
    public class DirectoryObserver
    {
        private readonly FileSystemWatcher watcher;
        private readonly string extension;
        private readonly FileChangesChecker integrityChecker;

        public event EventHandler FilesChanged;

        public DirectoryObserver(string path, string extension)
        {
            this.watcher = new FileSystemWatcher();
            this.watcher.Path = path;
            this.watcher.IncludeSubdirectories = true;
            this.watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            this.watcher.Filter = $"*.{extension}";
            this.watcher.EnableRaisingEvents = true;
            this.watcher.Changed += new FileSystemEventHandler(this.OnFileChanged);
            this.watcher.Created += new FileSystemEventHandler(this.OnFileChanged);
            this.watcher.Deleted += new FileSystemEventHandler(this.OnFileChanged);
            this.extension = extension;
            this.integrityChecker = new FileChangesChecker(path,extension);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File {e.FullPath} {e.ChangeType}");

            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                //this.integrityChecker.RemoveHash(e.FullPath);
            }
            else
            {
                //this.integrityChecker.UpdateHash(e.FullPath);
            }
            this.FilesChanged?.Invoke(this, e);
        }

        public void StartObserving()
        {
            this.watcher.EnableRaisingEvents = true;
            Console.WriteLine("Started watching...");
        }

        public void StopObserving()
        {
            this.watcher.EnableRaisingEvents = false;
        }
    }
}
