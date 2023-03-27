using KdlDotNet;

namespace WebMagic
{
    internal class Compiler
    {
        private List<Section> definedSections;
        private readonly string system_Folder;

        public Compiler(string system_folder)
        {
            system_Folder = system_folder;
            definedSections = DefineSystemSections();
        }

        internal void Compile(KDLFile file, string outputFileName, string errorFileName)
        {
            //ValidateKDL(file, outputFileName, errorFileName);
            CreateOutputDirectory(outputFileName);
            RenderKDL(file, outputFileName, errorFileName);
        }

        private void CreateOutputDirectory(string outputFileName)
        {
            string outputDirectory = Path.GetDirectoryName(outputFileName);
            // Create the output directory if it doesn't exist
            if (!System.IO.Directory.Exists(outputDirectory)) {
                if(outputDirectory == GlobalPaths.OutputFolder){
                    Console.WriteLine($"Copying assets...");
                    CopyFolder(GlobalPaths.AssetsFolder, GlobalPaths.OutputFolder);
                }
                Console.WriteLine($"Creating new folder {outputDirectory}...");
                System.IO.Directory.CreateDirectory(outputDirectory);
            }
        }

        public static void CopyFolder(string sourceFolderPath, string destinationFolderPath)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo sourceDir = new DirectoryInfo(sourceFolderPath);
            DirectoryInfo[] sourceSubDirs = sourceDir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            // Copy the files in the source directory to the destination directory.
            FileInfo[] sourceFiles = sourceDir.GetFiles();
            foreach (FileInfo file in sourceFiles)
            {
                string destinationFilePath = Path.Combine(destinationFolderPath, file.Name);
                file.CopyTo(destinationFilePath, true);
            }

            // Upload the files in the source directory to S3
            Console.WriteLine($"Uploading assets...");
            string key = "";
            List<string> _sourceFiles = new List<string>();
            foreach (FileInfo file in sourceFiles)
            {
                key = Path.GetRelativePath(GlobalPaths.AssetsFolder,file.DirectoryName);
                _sourceFiles.Add(file.FullName);
            }
            S3Uploader uploader = new S3Uploader("us-west-2", "axonator.co",key=="."?"":key);
            uploader.uploadFiles(_sourceFiles);
            
            // Copy the subdirectories in the source directory to the destination directory.
            foreach (DirectoryInfo subDir in sourceSubDirs)
            {
                string destinationSubDirPath = Path.Combine(destinationFolderPath, subDir.Name);
                CopyFolder(subDir.FullName, destinationSubDirPath);
            }
        }

        /// <summary> This code validates sections in a KDL file by looping through all 
        /// the nodes and checking whether each section has been defined. If a section 
        /// has not been defined or is abstract, an error message is written to a file. 
        /// The code uses a StreamWriter instance to write to the file and closes the 
        /// instance after finishing writing.
        /// </summary>
        private void ValidateKDL(KDLFile file, string outputFileName, string errorFileName)
        {
            StreamWriter sw = new StreamWriter(new FileStream(errorFileName, FileMode.Create));
            List<string> errors = new List<string>();
            // Traverse the KDL document
            foreach (KDLNode node in file.Doc.Nodes)
            {
                // Find the section in the list of defined sections
                Section? section = definedSections.Find(section => section.HandlerName.Value == node.Identifier);
                if (section == null)
                {
                    // If the section is not defined, write an error message to the file
                    sw.WriteLine($"Error: Section {node.Identifier} is not defined.");
                }
                else
                {
                    // If the section is abstract, write an error message to the file
                    if (section.isAbstract.Value)
                        sw.WriteLine($"Error: Section {node.Identifier} is abstract and cannot be used.");
                    else
                    {
                        List<string> list = section.Validate(node);
                        //errors.Append(list);
                    }
                }
            }
            // Close the StreamWriter instance
            sw.Close();
        }
        /// <summary>
        /// Render the KDL file to the output file
        /// </summary>
        private void RenderKDL(KDLFile file, string outputFileName, string errorFileName)
        {
            // Create a StreamWriter instance to write to the output file
            StreamWriter sw = new StreamWriter(new FileStream(outputFileName, FileMode.Create));
            // Traverse the KDL document
            foreach (KDLNode node in file.Doc.Nodes)
            {
                // Find the section in the list of defined sections
                definedSections.Where(section => section.HandlerName.Value == node.Identifier).ToList().ForEach(section =>
                {
                    // Render the section
                    string value = section.Handler.RenderSection(node);
                    sw.WriteLine(value);
                });
            }
            // Close the StreamWriter instance
            sw.Close();
        }
        /// <summary>
        /// This code defines the sections that are available in the system. It reads
        /// the KDL file that defines the sections and creates a Section instance for
        /// each section. It returns a list of Section instances.
        /// </summary>
        private List<Section> DefineSystemSections()
        {
            // Read the KDL file that defines the sections
            string kdlFileName = Path.Combine(system_Folder, "kdl_templates/system_sections.kdl");
            KDLFile kdlFile = new KDLFile(kdlFileName);

            List<Section> definedSections = new List<Section>();
            // Traverse the KDL document
            foreach (KDLNode node in kdlFile.Doc.Nodes)
            {
                // Create a Section instance for each section
                if (node.Props != null)
                {
                    // Get the properties of the section
                    KDLString handlerClassName = node.Props["class"].AsString();
                    KDLString handlerName = node.Props["name"].AsString();
                    KDLBoolean isAbstract = KDLBoolean.False;
                    if (node.Props.ContainsKey("abstract") && node.Props["abstract"] != KDLNull.Instance)
                        isAbstract = node.Props["abstract"].AsBoolean();
                    // Create a Section instance
                    Section s = new Section(node.Identifier, handlerClassName, handlerName, isAbstract);
                    // Add an Instruction instance to the list of instructions
                    if (node.Child != null)
                    foreach (KDLNode n in node.Child.Nodes)
                    {
                        Instruction i = new Instruction(n.Identifier);
                        i.validator = new Validator();
                        if (n.Props.ContainsKey("style") && n.Props["style"] != KDLNull.Instance)
                            i.validator.InstructionStyle.FromString(n.Props["style"].AsString().Value);
                        if (n.Props.ContainsKey("type") && n.Props["type"] != KDLNull.Instance)
                            i.validator.InstructionType.FromString(n.Props["type"].AsString().Value);
                        if (n.Props.ContainsKey("required") && n.Props["required"] != KDLNull.Instance)
                            i.validator.Required=n.Props["required"].AsBoolean().Value;

                        if (n.Props.ContainsKey("min") && n.Props["min"] != KDLNull.Instance)
                            i.validator.Min_Words = 10; //TODO: n.Props["min_words"].AsNumber();
                        if (n.Props.ContainsKey("max") && n.Props["max"] != KDLNull.Instance)
                            i.validator.Max_Words = 20; //TODO: n.Props["max_words"].AsNumber();


                        s.DefinedInstructions.Add(i);
                    }
                    // Add the Section instance to the list
                    definedSections.Add(s);
                }
            }
            // Return the list of Section instances
            return definedSections;
        }
    }
}