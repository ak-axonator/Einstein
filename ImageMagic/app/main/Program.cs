using System.Text.RegularExpressions;
using System.Xml;

namespace ImageMagic
{
    public class Program
    {
        static void Main(string[] args)
        {
            testDynamicSVGRender();

        }

        public static void testDynamicSVGRender(){
            string SVGFileName = "/Users/arohikulkarni/Work/Einstein/SystemFiles/assets_to_copy/assets/images/SVGs/Checklist-on-mobile-app.svg";
            // string appName = "Housekeeping HSE Audit";
            
            var data = new Dictionary<string, object>()
            {
                { "app_name", "Housekeeping HSE Audit" },
                { "form", new Dictionary<string, object>()
                    {
                        { "group1", "Specifications" },
                        { "group2", "Internal Audits" }
                    }
                }
            };
            string img_src = ReplaceVariablesInSvg(SVGFileName,data);
            // CreatePageFileFromOutputKDL();
        }

        private static string ReplaceVariablesInSvg(string svgPath, Dictionary<string, object> data)
        {
            // Load the SVG file into an XmlDocument
            var doc = new XmlDocument();
            doc.Load(svgPath);

            // Find all text nodes that contain variable annotations
            var variableRegex = new Regex(@"{([^{}]+)}");
            var textNodes = doc.SelectNodes("//text()");

            foreach (XmlNode textNode in textNodes)
            {
                // Replace variable annotations with their corresponding values
                var matches = variableRegex.Matches(textNode.Value);
                foreach (Match match in matches)
                {
                    var variableName = match.Groups[1].Value;
                    var variableValue = GetValueFromData(variableName, data);
                    textNode.Value = textNode.Value.Replace(match.Value, variableValue.ToString());
                }
            }

            // Save the modified SVG to a new file
            string newSvgPath = Path.Combine(Path.GetDirectoryName(svgPath), "new.svg");
            doc.Save(newSvgPath);
            return newSvgPath;
        }

        static object GetValueFromData(string variableName, Dictionary<string, object> data)
        {
            // Traverse nested objects using dot notation in variable name
            var parts = variableName.Split('.');
            object value = data;
            foreach (var part in parts)
            {
                if (!(value is Dictionary<string, object> dictionary) || !dictionary.ContainsKey(part))
                {
                    throw new ArgumentException($"Variable {variableName} not found in data.");
                }
                value = dictionary[part];
            }
            return value;
        }
    }
}