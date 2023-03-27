

using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace WebMagic
{
    internal class ComandProcessor
    {
        private string _folderPath;
        private bool _compile = true;
        private bool _upload = false;
        private bool _compileAndWatch = false;
        private bool _uploadAndWatch = false;
        DirectoryObserver observer;

        public ComandProcessor(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: FolderWatcherExample.exe <folder> [-c|-u|-cw|-uw]");
                return;
            }
            
            _folderPath = args[0];
            if (!Directory.Exists(_folderPath))
            {
                Console.WriteLine($"Folder not found: {_folderPath}");
                return;
            }

            //If folder found, set global paths relative to the folder location
            setGlobalPaths();

            //Set compile, upload and watch flags
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-c":
                        _compile = true;
                        break;
                    case "-u":
                        _upload = true;
                        break;
                    case "-cw":
                        _compileAndWatch = true;
                        break;
                    case "-uw":
                        _uploadAndWatch = true;
                        break;
                    default:
                        Console.WriteLine($"Unknown option: {args[i]}");
                        return;
                }
            }
        }

        private void setGlobalPaths(){
            string project_folder = _folderPath;
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string system_folder = configuration.GetValue<string>("system_folder");
            string output_folder = Path.Combine(Directory.GetParent(project_folder).FullName, "S3Folder");

            GlobalPaths.SystemFolder = system_folder;
            GlobalPaths.ProjectFolder = project_folder;
            GlobalPaths.OutputFolder = output_folder;
            GlobalPaths.AssetsFolder = Path.Combine(GlobalPaths.SystemFolder, "assets_to_copy");
        }

        public void processCommand()
        {
            Console.WriteLine($"Started processing compile command for: {_folderPath}");
            processChanges();
            watchChanges();
        }

        public void OnSourceChange(object sender, EventArgs e){
            Console.WriteLine("Souce change detected...");
            processChanges();
        }
        private void processChanges()
        {
            var (updatedFiles, deletedFiles) = getFileChanges();
            List<string> compiledFiles = compileFileChanges(updatedFiles);
            uploadChanges(compiledFiles);
            deleteFiles(deletedFiles);
        }

        private (List<string>, List<string>) getFileChanges()
        {
            Console.WriteLine($"Looking for changes in {_folderPath}");
            FileChangesChecker checker = new FileChangesChecker(_folderPath, "*.page");
            checker.CheckChanges();
            List<string> filesAdded = checker.GetAddedFiles();
            List<string> filesModified = checker.GetModifiedFiles();
            List<string> filesDeleted = checker.GetDeletedFiles();

            List<string> updatedFiles = filesAdded.Concat(filesModified).ToList();
            return (updatedFiles, filesDeleted);
        }

        private static List<string> compileFileChanges(List<string> updatedFiles)
        {
            List<string> compiledFiles = new List<string>();
            Compiler c = new Compiler(GlobalPaths.SystemFolder);

            foreach (string file in updatedFiles)
            {
                Console.WriteLine($"Compiling {file}...");
                KDLFile inputFile = new KDLFile(Path.Combine(GlobalPaths.ProjectFolder, file));
                string insidePath = Path.GetRelativePath(Path.Combine(GlobalPaths.ProjectFolder,"pages"),Path.GetDirectoryName(file));
                string outputFile = Path.Combine(GlobalPaths.OutputFolder,insidePath=="."?"":insidePath, Path.GetFileNameWithoutExtension(file) + ".html");
                string errorFile = outputFile + ".errors.txt";
                c.Compile(inputFile, outputFile, errorFile);
                compiledFiles.Add(outputFile);
            }
            return compiledFiles;
        }
        private void uploadChanges(List<string> compiledFiles)
        {
            if (_upload || _uploadAndWatch || compiledFiles.Count > 0)
            {
                Console.WriteLine($"Uploading {compiledFiles.Count} files...");
                S3Uploader uploader = new S3Uploader("us-west-2", "axonator.co");
                uploader.uploadFiles(compiledFiles);
            }
        }

        private void deleteFiles(List<string> deletedFiles)
        {
            S3Uploader uploader = new S3Uploader("us-west-2", "axonator.co");
            uploader.deleteFiles(deletedFiles);
        }
        private void watchChanges()
        {
            if (_compileAndWatch || _uploadAndWatch)
            {
                // Start the watcher and wait for events
                Console.WriteLine($"Starting watch on {_folderPath}...");
                observer = new DirectoryObserver(_folderPath, "page");
                observer.FilesChanged += new EventHandler(this.OnSourceChange);
                observer.StartObserving();
                Console.ReadLine();
                observer.StopObserving();
            }
        }
    }
}