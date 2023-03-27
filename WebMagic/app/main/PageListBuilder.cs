namespace WebMagic
{
    public class PageListBuilder
    {
        private string rootFolder;

        public PageListBuilder(string folderName)
        {
            rootFolder = folderName;
        }

        public List<string> BuildPageList()
        {
            List<string> pageList = new List<string>();
            CollectPageFiles(rootFolder, pageList);
            return pageList;
        }

        private void CollectPageFiles(string currentFolder, List<string> pageList)
        {
            try
            {
                // Collect all files with extension ".page"
                foreach (string file in Directory.GetFiles(currentFolder, "*.page"))
                {
                    string relativePath = Path.GetRelativePath(rootFolder, file);
                    pageList.Add(relativePath);
                }

                // Recursively collect files in subdirectories
                foreach (string subFolder in Directory.GetDirectories(currentFolder))
                {
                    CollectPageFiles(subFolder, pageList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error collecting files in folder {currentFolder}: {ex.Message}");
            }
        }
    }
}