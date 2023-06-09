using System.Text.RegularExpressions;
using Cottle;
using KdlDotNet;
using System.Web;

namespace WebMagic
{
    internal abstract class SectionHandler : ISectionHandler
    {
        abstract protected string GetTemplateName();
            string ISectionHandler.RenderSection(KDLNode node)
        {
            string templateName = GetTemplateName();
            if (node.Child == null)
            {
                Dictionary<string, object> dict0 = new Dictionary<string, object>();
                Dictionary<Value, Value> valueDict0 = ConvertDict2ValueDict(dict0);
                return RenderTemplate(valueDict0, templateName);
            }
            IReadOnlyList<KDLNode> nodes = node.Child.Nodes;
            
            Dictionary<string, object> dict = ConvertKDLNodes2Dictionary(nodes);
            Dictionary<Value, Value> valueDict = ConvertDict2ValueDict(dict);
            return RenderTemplate(valueDict, templateName);
        }

        private Dictionary<Value, Value> ConvertDict2ValueDict(Dictionary<string, object> propertyCollection)
        {
            Dictionary<Value, Value> valueDict = new Dictionary<Value, Value>();
            foreach (var item in propertyCollection)
            {
                if (item.Value is Dictionary<string, object>)
                {
                    valueDict[item.Key] = ConvertDict2ValueDict((Dictionary<string, object>)item.Value);
                }
                else if (item.Value is List<object>)
                {
                    List<Value> list = new List<Value>();
                    foreach (var v in (List<object>)item.Value)
                    {
                        if (v is Dictionary<string, object>)
                        {
                            list.Add(ConvertDict2ValueDict((Dictionary<string, object>)v));
                        }
                        else
                        {
                            list.Add((Value)v);
                        }
                    }
                    valueDict[item.Key] = list;
                }
                else
                {
                    KDLValue kdlValue = (KDLValue)item.Value;
                    if(kdlValue.IsNumber)
                        valueDict[item.Key] = kdlValue.AsNumber().ToString();
                    else
                    if(kdlValue.IsString){
                        string kdl_val = HttpUtility.HtmlEncode(kdlValue.AsString().Value);
                        valueDict[item.Key] = ReplaceNewLines(ReplaceHyperlinks(kdl_val));
                    }
                    else if(kdlValue.IsBoolean)
                        valueDict[item.Key] = kdlValue.AsBoolean().Value;


                }
            }
            return valueDict;
        }
        private static string ReplaceHyperlinks(string input)
        {
            var hyperlinkRegex = new Regex(@"\[(.*?)\]\((.*?)\)");
            var output = input;
            
            foreach (Match match in hyperlinkRegex.Matches(input))
            {
                var hyperlinkText = match.Groups[1].Value;
                var hyperlinkUrl = match.Groups[2].Value;
                var hyperlinkTag = $" <a class=\"text-link\" href=\"{hyperlinkUrl}\">{hyperlinkText}</a>";
                output = output.Replace(match.Value, hyperlinkTag);
            }
            
            return output;
        }
        private static string ReplaceNewLines(string input)
        {
            var output = input;
            output = output.Replace("\n", "<br>");
            return output;
        }

        private Dictionary<string, object> ConvertKDLNodes2Dictionary(IReadOnlyList<KDLNode> nodes)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var n in nodes)
            {
                if (n.Args.Count > 0)
                {
                        dict[n.Identifier.ToString()] = n.Args[0];
                }
                else
                {
                    if (n.Child != null)
                    {
                        Dictionary<string, object> heroAttributes_child;
                        string key;
                        heroAttributes_child = new Dictionary<string, object>();
                        key = n.Identifier.ToString();
                        if (dict.ContainsKey(key))
                        {
                            object old_value = (object)dict[key];
                            string new_key = key + "s";
                            List<object> list;
                            if (dict.ContainsKey(new_key))
                            {
                                list = (List<object>)dict[new_key];
                                list.Add(old_value);
                            }
                            else
                            {
                                list = new List<object>();
                                list.Add(old_value);
                            }
                            dict[new_key] = list;
                        }
                        dict[key] = ConvertKDLNodes2Dictionary(n.Child.Nodes);
                    }
                }
            }
            return dict;
        }

        private static string RenderTemplate(Dictionary<Value, Value> heroAttributes, string templateName)
        {
            string template = TemplateLoader.LoadTemplate(templateName);
            var documentResult = Document.CreateDefault(template); // Create from template string
            IDocument? document = null;
            string output;
            try {
                document = documentResult.DocumentOrThrow; // Throws ParseException on error
                IContext context = Context.CreateBuiltin(heroAttributes);
                output = document.Render(context);
            }
            catch (Exception e) {
                output = "";
                CommandProcessor.LogJsonParsingError(e, e.Message + template);
            }
            return output;
        }
    }
}