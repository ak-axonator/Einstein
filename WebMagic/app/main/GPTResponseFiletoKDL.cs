using System.Text.Json;
using Cottle;
using KdlDotNet;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

namespace WebMagic
{
    internal class GPTResponseFiletoKDL
    {
        internal static void Run(string inputFilePath, string outputFilePath, string promptGeneratorFilePath)
        {
            Console.WriteLine($"Generating KDL output for {inputFilePath}");
            string jsonString = File.ReadAllText(inputFilePath);
            var responseFile = JsonConvert.DeserializeObject<GPTResponseFile>(jsonString);

            string promptGeneratorJsonString = File.ReadAllText(promptGeneratorFilePath);
            var promptGeneratorFile = JsonConvert.DeserializeObject<Input>(promptGeneratorJsonString);
            List<string> outputBuffer = new List<string>();
            
            outputBuffer.AddRange(promptGeneratorFile.PageStart);

            foreach (var response in responseFile.Responses)
            {
                string section = response.Artifact;
                string responseJSON = response.Response.Replace("{'","{\"").Replace(" '"," \"").Replace("\n'","\n\"").Replace("':","\":").Replace("'}","\"}").Replace("',","\",").TrimEnd('.');
                //read file into a string with file name as section appended with _kdl_template.kdl
                string kdlTemplate = File.ReadAllText(Path.Combine(GlobalPaths.SystemFolder, "kdl_templates",section + "_kdl_template.kdl"));
                //replace the response text in the template
                Console.WriteLine(responseJSON);
                JsonDocumentOptions options = new JsonDocumentOptions { AllowTrailingCommas = true };
                try{

                    JsonDocument jsonDocument = JsonDocument.Parse(responseJSON, options);

                    Dictionary<string, object> dict = ConvertJSONDoc2Dict(jsonDocument);
                    Dictionary<Value, Value> valueDict = ConvertDict2ValueDict(dict);
                    string kdlString;
                    kdlString = RenderTemplate(valueDict, kdlTemplate);
                    outputBuffer.Add(kdlString);
                }
                catch (JsonException ex)
                {
                    LogJsonParsingError(ex, responseJSON);
                }
            
            }

            outputBuffer.AddRange(promptGeneratorFile.PageEnd);
            File.WriteAllLines(outputFilePath, outputBuffer);
            Console.WriteLine($"KDL generated in {outputFilePath}");
        }

        static void LogJsonParsingError(JsonException ex, string responseJSON)
        {
            string logFilePath = Path.Combine(GlobalPaths.GPTFolder,"GPTJsonParsing.errors.txt");

            string logMessage = $"An error occurred while parsing the following JSON: {responseJSON}\n\nError details:\n{ex}";

            File.AppendAllText(logFilePath, logMessage);
        }

        private static Dictionary<string, object> ConvertJSONDoc2Dict(JsonDocument jsonDocument)
        {
            
            JsonElement rootElement = jsonDocument.RootElement;
            Dictionary<string, object> valueDict = new Dictionary<string, object>();

            foreach (JsonProperty property in rootElement.EnumerateObject())
            {
                string key = property.Name;
                JsonElement value = property.Value;
                if (value.ValueKind == JsonValueKind.Array)
                {
                    List<object> valueList = new List<object>();
                    foreach (JsonElement element in value.EnumerateArray())
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>();
                        foreach (JsonProperty prop in element.EnumerateObject())
                        {
                            string key2 = prop.Name;
                            JsonElement value2 = prop.Value;
                            dict.Add(key2, value2.ToString());
                        }
                        valueList.Add(dict);
                    }
                    valueDict.Add(key, valueList);
                }
                else if (value.ValueKind == JsonValueKind.Object)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (JsonProperty prop in value.EnumerateObject())
                    {
                        string key2 = prop.Name;
                        JsonElement value2 = prop.Value;
                        dict.Add(key2, value2.ToString());
                    }
                    valueDict.Add(key, dict);
                }
                else
                {
                    valueDict.Add(key, value.ToString());
                }
            }
            return valueDict;
        }
  private static Dictionary<Value, Value> ConvertDict2ValueDict(Dictionary<string, object> propertyCollection)
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
                    KDLValue kdlValue = KDLValue.From(item.Value);
                    if(kdlValue.IsNumber)
                        valueDict[item.Key] = kdlValue.AsNumber().ToString();
                    else
                    if(kdlValue.IsString)
                        valueDict[item.Key] = kdlValue.AsString().Value; 
                    else if(kdlValue.IsBoolean)
                        valueDict[item.Key] = kdlValue.AsBoolean().Value;


                }
            }
            return valueDict;
        }

        private static string RenderTemplate(Dictionary<Value, Value> valueDict, string kdlTemplate)
        {
            var documentResult = Document.CreateDefault(kdlTemplate); // Create from template string
            IDocument? document = null;
            string output;
            try {
                document = documentResult.DocumentOrThrow; // Throws ParseException on error
                IContext context = Context.CreateBuiltin(valueDict);
                output = document.Render(context);
            } 
            catch (Exception e) {
                output = e.Message + kdlTemplate;

            }
            return output;
        }
        private static string RenderImageTemplate(Dictionary<Value, Value> valueDict, string kdlTemplate)
        {
            var documentResult = Document.CreateDefault(kdlTemplate); // Create from template string
            IDocument? document = null;
            string output = "";
            // valueDict["Points"].Fields[0].Fields["headline"].AsString
            List<Value> list = new List<Value>();
            // foreach (var point in valueDict["Points"].Fields)
            // {
            //     try {
            //         document = documentResult.DocumentOrThrow; // Throws ParseException on error
            //         IContext context = Context.CreateBuiltin(point);
            //         output += document.Render(context);
            //     } 
            //     catch (Exception e) {
            //         output = e.Message + kdlTemplate;
            //     }
            // }
            
            return output;
        }
    }
}