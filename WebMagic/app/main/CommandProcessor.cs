

using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace WebMagic
{
    internal class CommandProcessor
    {
        private string _folderPath;
        private bool _compile = true;
        private bool _upload = true;
        private bool _compileAndWatch = true;
        private bool _uploadAndWatch = false;
        DirectoryObserver observer;

        public CommandProcessor(){
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            this._folderPath = configuration.GetValue<string>("default_project_folder");
        }
        public CommandProcessor(string[] args)
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
            initGlobalPaths();

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

        public void initGlobalPaths(){
            string project_folder = _folderPath;
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string system_folder = configuration.GetValue<string>("system_folder");
            string log_folder = configuration.GetValue<string>("log_folder");
            string output_folder = Path.Combine(Directory.GetParent(project_folder).FullName, "Axonator Website");

            GlobalPaths.SystemFolder = system_folder;
            GlobalPaths.ProjectFolder = project_folder;
            GlobalPaths.OutputFolder = output_folder;
            GlobalPaths.LogFolder = log_folder;
            GlobalPaths.AssetsFolder = Path.Combine(GlobalPaths.SystemFolder, "assets_to_copy");
            GlobalPaths.GPTFolder = Path.Combine(GlobalPaths.SystemFolder, "GPTJsonPageGenFiles");
        }

        public static void LogJsonParsingError(Exception ex, string error_msg, string fileName = "")
        {
            string logFilePath = Path.Combine(GlobalPaths.LogFolder,"AppStore.errors.txt");

            string logMessage = $"An error occurred while parsing the following JSON: {error_msg}\n\nError details:\n{ex}";

            File.AppendAllText(logFilePath, fileName + "\n");
            File.AppendAllText(logFilePath, logMessage);
            File.AppendAllText(logFilePath, "\n\n ----------------------------- \n\n");
        }
        
        public void processCommand()
        {
            Console.WriteLine($"Started processing compile command for: {_folderPath}");
            try
            {
                processChanges();
                watchChanges();
            }
            catch (Exception e)
            {
                LogJsonParsingError(e, e.Message);
            }
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
                try {
                    KDLFile inputFile = new KDLFile(Path.Combine(GlobalPaths.ProjectFolder, file));
                    string insidePath = Path.GetRelativePath(Path.Combine(GlobalPaths.ProjectFolder),Path.GetDirectoryName(file));
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName.StartsWith(insidePath.Replace(" ","_"))) {
                        fileName = fileName.Substring(insidePath.Length + 1);
                    }
                    insidePath = insidePath.Replace("_","-").Replace(" ","-");
                    string outputFile = Path.Combine(GlobalPaths.OutputFolder,insidePath=="."?"":insidePath, fileName).Replace("_","-").TrimStart('-').TrimEnd('-');
                    string errorFile = outputFile + ".errors.txt";
                    c.Compile(inputFile, outputFile, errorFile);

                    // outputFile = RemoveHtmlExtension(outputFile);
                    compiledFiles.Add(outputFile);
                } catch (Exception e){
                    LogJsonParsingError(e, e.Message, file);
                }
            }
            return compiledFiles;
        }
        public static string RemoveHtmlExtension(string filepath)
        {
            // Get the file name without extension
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filepath);

            // Get the file path without the file name
            string pathWithoutFileName = Path.GetDirectoryName(filepath);

            // Combine the path and updated file name with no extension
            string updatedFilePath = Path.Combine(pathWithoutFileName, fileNameWithoutExt);

            // Rename the file
            File.Move(filepath, updatedFilePath);

            // Return the updated file path
            return updatedFilePath;
        }
        private void uploadChanges(List<string> compiledFiles)
        {
            if ((_upload || _uploadAndWatch) && compiledFiles.Count > 0)
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