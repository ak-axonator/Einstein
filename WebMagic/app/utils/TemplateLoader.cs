namespace WebMagic
{
    internal class TemplateLoader
    {
        internal static string LoadTemplate(string templateName)
        {
            string templatesPath = Path.Combine(GlobalPaths.SystemFolder, "html_templates");
            string fileName = Path.Combine(templatesPath, templateName + ".html");
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string template = sr.ReadToEnd();
            sr.Close();
            return template;
        }
    }
}