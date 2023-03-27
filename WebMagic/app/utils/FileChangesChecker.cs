using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace WebMagic
{
    public class FileChangesChecker
    {
        private readonly string _hashFilePath;
        private readonly string _directoryPath;
        private readonly string _fileExtension;

        private readonly List<string> _addedFiles;
        private readonly List<string> _modifiedFiles;
        private readonly List<string> _deletedFiles;

        public FileChangesChecker(string directoryPath = null, string fileExtension = null)
        {
            _directoryPath = directoryPath ?? @"E:\JK\watchthisfolder";
            _fileExtension = fileExtension ?? "*.*";

            _hashFilePath = Path.Combine(_directoryPath, ".hashes", "hashes.xyz");

            _addedFiles = new List<string>();
            _modifiedFiles = new List<string>();
            _deletedFiles = new List<string>();
        }

        public void CheckChanges()
        {
            var currentFileHashes = GetFileHashes(_directoryPath);

            var previousFileHashes = new Dictionary<string, string>();
            if (File.Exists(_hashFilePath))
            {
                var previousFileHashLines = File.ReadAllLines(_hashFilePath);
                foreach (var line in previousFileHashLines)
                {
                    var hashParts = line.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (hashParts.Length != 2) continue;

                    previousFileHashes[hashParts[1]] = hashParts[0];
                }
            }

            foreach (var currentHash in currentFileHashes)
            {
                if (previousFileHashes.TryGetValue(currentHash.Key, out var previousHash))
                {
                    if (currentHash.Value != previousHash)
                    {
                        _modifiedFiles.Add(currentHash.Key);
                    }
                }
                else
                {
                    _addedFiles.Add(currentHash.Key);
                }
            }

            foreach (var previousHash in previousFileHashes)
            {
                if (!currentFileHashes.ContainsKey(previousHash.Key))
                {
                    _deletedFiles.Add(previousHash.Key);
                }
            }

            WriteHashesToFile(currentFileHashes);
        }

        private Dictionary<string, string> GetFileHashes(string directoryPath)
        {
            var fileHashes = new Dictionary<string, string>();

            foreach (var filePath in Directory.GetFiles(directoryPath, _fileExtension, SearchOption.AllDirectories))
            {
                if (filePath.StartsWith(Path.Combine(directoryPath, ".hashes"))) continue;

                using (var stream = File.OpenRead(filePath))
                {
                    var hash = SHA256.Create().ComputeHash(stream);
                    var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                    fileHashes[filePath] = hashString;
                }
            }

            return fileHashes;
        }

        private void WriteHashesToFile(Dictionary<string, string> fileHashes)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_hashFilePath));

            using (var writer = new StreamWriter(_hashFilePath, false))
            {
                foreach (var fileHash in fileHashes)
                {
                    writer.WriteLine($"{fileHash.Value} {fileHash.Key}");
                }
            }
        }

        public List<string> GetAddedFiles()
        {
            return _addedFiles;
        }

        public List<string> GetModifiedFiles()
        {
            return _modifiedFiles;
        }

        public List<string> GetDeletedFiles()
        {
            return _deletedFiles;
        }

        internal void Reset()
        {
            _addedFiles.Clear();
            _modifiedFiles.Clear();
            _deletedFiles.Clear();

        }
    }
}
