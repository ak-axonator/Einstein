using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MarkdownSharp;
using System.Globalization;


namespace WebMagic
{
    public class ArtifactParser
    {
        public static string formPreviewCss = @"<!DOCTYPE html><html><head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'><title>Form</title><style>
                            /* Add your custom styles here */
                            .container {width: 60%;margin-left: auto;margin-right: auto;}
                            .form {width: 60%;margin-left: auto!important;margin-right: auto!important;}
                            .form-group {margin-bottom: 10px;margin-left: auto;}.form-label {font-weight: bold;margin-bottom: 5px;}
                            .checkpoint-hint {width: 18px;height: 22px;display: inline-block;border: 1px solid lightgrey;border-radius: 50%;}
                            .submit-btn {width: 50%;}
                            .form-control {width: 50%;padding: 10px;font-size: 16px;border-radius: 4px;border: 1px solid #ccc;box-sizing: border-box;}
                            .form-control:focus {border-color: #007bff;outline: none;box-shadow: none;}
                            .btn {display: inline-block;padding: 6px 12px;margin-bottom: 0;font-size: 14px;font-weight: 400;line-height: 1.42857143;text-align: center;white-space: nowrap;vertical-align: middle;-ms-touch-action: manipulation;touch-action: manipulation;cursor: pointer;border: 1px solid transparent;border-radius: 4px;}
                            .btn-primary {color: #fff;background-color: #007bff;border-color: #007bff;}
                            .btn-primary:hover {color: #fff;background-color: #0069d9;border-color: #0062cc;}
                        </style>
                    </head>
                    <body>
                        <div class='container'>";
        public static void ConvertToKDL()
        {
            var inputPath = GlobalPaths.LogFolder;
            var jsonFiles = Directory.GetFiles(inputPath, "*.jsonc")
                                    .Concat(Directory.GetFiles(inputPath, "*.md"))
                                    .ToArray();
            
            // Create App Store KDLs folder if doesn't already exists
            var outputPath = Path.Combine(GlobalPaths.LogFolder, "../App Store KDLs");
            var dummyFile = "dummy.page";
            var dummyFilePath = Path.Combine(outputPath, dummyFile);
            Compiler.CreateOutputDirectory(dummyFilePath);
            
            // Create App Store Docs folder if doesn't already exists
            var outputDocsPath = Path.Combine(outputPath, "App Store Docs");
            dummyFilePath = Path.Combine(outputDocsPath, dummyFile);
            Compiler.CreateOutputDirectory(dummyFilePath);


            foreach (var jsonFile in jsonFiles)
            {
                char[] trimChars = { '_', '-' };
                string outputFilePath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(jsonFile).ToLower().Replace(" ","-").TrimStart(trimChars).TrimEnd(trimChars) + ".page");
                
                if(File.Exists(outputFilePath))
                {
                    Console.WriteLine($"Skipping {outputFilePath} because it already exists...");
                    continue;
                }

                var kdl_lines = new List<string>();

                if (Path.GetExtension(jsonFile) == ".md")
                {
                    // if(jsonFile.Contains("dashboard"))
                    //     Console.WriteLine("Dashboard");
                    GeneratePreviewFile(outputDocsPath, jsonFile, kdl_lines);
                }
                else
                {
                    // if filename starts with "GPTAppPageContents", deserialize it to AppDetails class
                    if (Path.GetFileNameWithoutExtension(jsonFile).StartsWith("GPTAppPageContents"))
                    {
                        var json = File.ReadAllText(jsonFile);
                        var appDetails = JsonConvert.DeserializeObject<AppDetails>(json);
                        kdl_lines.Add("app_summary_section {");
                        kdl_lines.Add($"\tname \"{appDetails.Name}\"");
                        kdl_lines.Add($"\tdescription \"{appDetails.Description}\"");
                        kdl_lines.Add($"\tindustry \"{appDetails.Industry}\"");
                        kdl_lines.Add($"\tcategory \"{appDetails.Category}\"");
                        kdl_lines.Add($"\ticon \"{appDetails.Icon}\"");
                        kdl_lines.Add($"\tform1 \"{appDetails.Form_Names[0]}\"");
                        kdl_lines.Add($"\tform2 \"{appDetails.Form_Names[1]}\"");
                        kdl_lines.Add($"\tform3 \"{appDetails.Form_Names[2]}\"");
                        kdl_lines.Add($"\treport \"{appDetails.Report_Names[0]}\"");
                        kdl_lines.Add($"\tdashboard \"{appDetails.Dashboard_Names[0]}\"");
                        if (appDetails.Integration_Names != null && appDetails.Integration_Names.Count > 0)
                        {
                            kdl_lines.Add($"\tintegration1 \"{appDetails.Integration_Names[0]}\"");
                            kdl_lines.Add($"\tintegration2 \"{appDetails.Integration_Names[1]}\"");
                            kdl_lines.Add($"\tintegration3 \"{appDetails.Integration_Names[2]}\"");
                        }
                        else
                        {
                            kdl_lines.Add($"\tintegration1 \"\"");
                            kdl_lines.Add($"\tintegration2 \"\"");
                            kdl_lines.Add($"\tintegration3 \"\"");
                        }
                        kdl_lines.Add("}");

                        // add 6 features grid with title and description for each feature
                        AddFeaturesSection(kdl_lines, appDetails.Product_Features);
                        // add 6 benefits grid with title and description for each benefit
                        AddBenefitsSection(kdl_lines, appDetails.Product_Benefits);
                        // add 4 users persona section
                        // AddPersonaSection(kdl_lines, appDetails.Users);

                        kdl_lines.Add("app_forms_section {");
                        kdl_lines.Add($"\tform1 \"{appDetails.Form_Names[0]}\"");
                        kdl_lines.Add($"\tform2 \"{appDetails.Form_Names[1]}\"");
                        kdl_lines.Add($"\tform3 \"{appDetails.Form_Names[2]}\"");
                        kdl_lines.Add("}");

                        kdl_lines.Add("app_reports_section {");
                        kdl_lines.Add($"\treport1 \"{appDetails.Report_Names[0]}\"");
                        kdl_lines.Add($"\treport2 \"{appDetails.Report_Names[1]}\"");
                        kdl_lines.Add($"\treport3 \"{appDetails.Report_Names[2]}\"");
                        kdl_lines.Add("}");

                        //if appDetails.Integration_Names is not null or empty, add integration section
                        if (appDetails.Integration_Names != null && appDetails.Integration_Names.Count > 0)
                        {
                            kdl_lines.Add("app_integrations_section {");
                            kdl_lines.Add($"\tintegration1 \"{appDetails.Integration_Names[0]}\"");
                            kdl_lines.Add($"\tintegration2 \"{appDetails.Integration_Names[1]}\"");
                            kdl_lines.Add($"\tintegration3 \"{appDetails.Integration_Names[2]}\"");
                            kdl_lines.Add("}");
                        }

                        // if appDetails.Dashboard_Names is not null or empty, add dashboard section
                        if (appDetails.Dashboard_Names != null && appDetails.Dashboard_Names.Count > 0)
                        {
                            kdl_lines.Add("app_dashboards_section {");
                            kdl_lines.Add($"\tdashboard1 \"{appDetails.Dashboard_Names[0]}\"");
                            kdl_lines.Add($"\tdashboard2 \"{appDetails.Dashboard_Names[1]}\"");
                            kdl_lines.Add($"\tdashboard3 \"{appDetails.Dashboard_Names[2]}\"");
                            kdl_lines.Add("}");
                        }
                    }
                    else
                    {
                        // Artifact artifact = JsonConvert.DeserializeObject<Artifact>(File.ReadAllText(jsonFile));
                        var form = new Form();
                        if (Path.GetFileNameWithoutExtension(jsonFile).StartsWith("form",StringComparison.InvariantCultureIgnoreCase))
                        {
                            try{
                                form = JsonConvert.DeserializeObject<Form>(File.ReadAllText(jsonFile));
                                kdl_lines.Add("artifact_hero_section {");
                                kdl_lines.Add($"\tname \"{form.Form_Title}\"");
                                kdl_lines.Add($"\tdescription \"{form.Form_Description}\"");
                                kdl_lines.Add($"\ttype \"form\"");
                                kdl_lines.Add("}");

                                // add 6 features grid with title and description for each feature
                                AddFeaturesSection(kdl_lines, form.Features);
                                // add 6 benefits grid with title and description for each benefit
                                AddBenefitsSection(kdl_lines, form.Benefits);
                                // add 4 users persona section
                                AddPersonaSection(kdl_lines, form.Users);
                                
                                GeneratePreviewFile(outputDocsPath, jsonFile, kdl_lines, "form");
                                
                                // add 10 faqs section
                                AddFAQsSection(kdl_lines, form.Faqs);
                            }
                            catch (Exception e){
                                CommandProcessor.LogJsonParsingError(e, e.Message, jsonFile);
                            }
                        }
                        else if(Path.GetFileNameWithoutExtension(jsonFile).StartsWith("checklist",StringComparison.InvariantCultureIgnoreCase))
                        {
                            var checklist = new Checklist();
                            try{
                                checklist = JsonConvert.DeserializeObject<Checklist>(File.ReadAllText(jsonFile));
                                kdl_lines.Add("artifact_hero_section {");
                                kdl_lines.Add($"\tname \"{checklist.Checklist_Title}\"");
                                kdl_lines.Add($"\tdescription \"{checklist.Checklist_Description}\"");
                                kdl_lines.Add($"\ttype \"checklist\"");
                                kdl_lines.Add("}");

                                // add 6 features grid with title and description for each feature
                                AddFeaturesSection(kdl_lines, checklist.Features);
                                // add 6 benefits grid with title and description for each benefit
                                AddBenefitsSection(kdl_lines, checklist.Benefits);
                                // add 4 users persona section
                                AddPersonaSection(kdl_lines, checklist.Users);
                                
                                GeneratePreviewFile(outputDocsPath, jsonFile, kdl_lines, "checklist");
                                
                                // add 10 faqs section
                                AddFAQsSection(kdl_lines, checklist.Faqs);
                            }
                            catch (Exception e){
                                CommandProcessor.LogJsonParsingError(e, e.Message, jsonFile);
                            }
                        }
                        else if(Path.GetFileNameWithoutExtension(jsonFile).StartsWith("audit-checklist",StringComparison.InvariantCultureIgnoreCase))
                        {
                            var checklist = new AuditChecklist();
                            try{
                                checklist = JsonConvert.DeserializeObject<AuditChecklist>(File.ReadAllText(jsonFile));
                                kdl_lines.Add("artifact_hero_section {");
                                kdl_lines.Add($"\tname \"{checklist.Audit_Checklist_Title}\"");
                                kdl_lines.Add($"\tdescription \"{checklist.Audit_Checklist_Description}\"");
                                kdl_lines.Add($"\ttype \"checklist\"");
                                kdl_lines.Add("}");

                                // add 6 features grid with title and description for each feature
                                AddFeaturesSection(kdl_lines, checklist.Features);
                                // add 6 benefits grid with title and description for each benefit
                                AddBenefitsSection(kdl_lines, checklist.Benefits);
                                // add 4 users persona section
                                AddPersonaSection(kdl_lines, checklist.Users);
                                
                                GeneratePreviewFile(outputDocsPath, jsonFile, kdl_lines, "checklist");
                                
                                // add 10 faqs section
                                AddFAQsSection(kdl_lines, checklist.Faqs);
                            }
                            catch (Exception e){
                                CommandProcessor.LogJsonParsingError(e, e.Message, jsonFile);
                            }
                        }
                        // kdl.AddSection("persona_section", section =>
                        // {
                        //     section.Add("user1", appDetails.User_Names[0]);
                        // });
                        // kdl.AddSection("how_to_use_artifact_section", section =>
                        // {
                        //     if (jsonObject.ContainsKey("use"))
                        //     {
                        //         foreach (var useObject in jsonObject["use"])
                        //         {
                        //             section.Add("use", useSection =>
                        //             {
                        //                 useSection.Add("description", (string)useObject["description"]);
                        //             });
                        //         }
                        //     }
                        // });
                    }
                }
                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    // prepend Page start lines to kdl_lines
                    kdl_lines.InsertRange(0, new string[] {
                        "//Page start",
                        "page_begin_section",
                        "navigation_section"
                    });
                    // append Page end lines to kdl_lines
                    kdl_lines.AddRange(new string[] {
                        "default_footer_section",
                        "page_end_section"
                    });
                    foreach (string line in kdl_lines)
                    {
                        Console.WriteLine(line);
                        writer.WriteLine(line.Replace("https://axonator.com/", "/"));
                    }
                    // VerifyAndCorrectKdlFile(outputFilePath);
                }
            }
        }
        private static void AddFAQsSection(List<string> kdl_lines, List<FAQ> faqs)
        {
            kdl_lines.Add("faq_section {");
            for (int i = 0; i < faqs.Count; i++)
            {
                kdl_lines.Add("\n\tquestion_answer {");
                kdl_lines.Add($"\t\tquestion \"{faqs[i].Question}\"");
                kdl_lines.Add($"\t\tanswer \"{faqs[i].Answer}\"");
                kdl_lines.Add("\n\t}");
            }
            kdl_lines.Add("}");
        }
        private static void AddPersonaSection(List<string> kdl_lines, List<string> users)
        {
            kdl_lines.Add("persona_section {");
            for (int i = 0; i < 4; i++)
            {
                if (i < users.Count)
                {
                    kdl_lines.Add($"\tuser{i + 1} \"{users[i]}\"");
                }
                else
                {
                    kdl_lines.Add($"\tuser{i + 1} \"\"");
                }
            }
            kdl_lines.Add("}");
        }
        private static void AddFeaturesSection(List<string> kdl_lines, List<Feature> features)
        {
            kdl_lines.Add("grid_section {");
            kdl_lines.Add("\ttitle \"Features\"");
            for (int i = 0; i < 6; i++)
            {
                if (i < features.Count)
                {
                    kdl_lines.Add($"\tcard {{");
                    kdl_lines.Add($"\t\ttitle \"{features[i].Features_Title}\"");
                    kdl_lines.Add($"\t\tdescription \"{features[i].Features_Description}\"");
                    kdl_lines.Add("\t}");
                }
                else
                {
                    kdl_lines.Add($"\tcard {{");
                    kdl_lines.Add($"\t\ttitle \"\"");
                    kdl_lines.Add($"\t\tdescription \"\"");
                    kdl_lines.Add("\t}");
                }
            }
            kdl_lines.Add("\tcard {}");
            kdl_lines.Add("}");
        }
        private static void AddBenefitsSection(List<string> kdl_lines, List<Benefit> benefits)
        {
            kdl_lines.Add("grid_section {");
            kdl_lines.Add("\ttitle \"Benefits\"");
            for (int i = 0; i < 6; i++)
            {
                if (i < benefits.Count)
                {
                    kdl_lines.Add($"\tcard {{");
                    kdl_lines.Add($"\t\ttitle \"{benefits[i].Benefits_Title}\"");
                    kdl_lines.Add($"\t\tdescription \"{benefits[i].Benefits_Description}\"");
                    kdl_lines.Add("\t}");
                }
                else
                {
                    kdl_lines.Add($"\tcard {{");
                    kdl_lines.Add($"\t\ttitle \"\"");
                    kdl_lines.Add($"\t\tdescription \"\"");
                    kdl_lines.Add("\t}");
                }
            }
            kdl_lines.Add("\tcard {}");
            kdl_lines.Add("}");
        }

        private static void GeneratePreviewFile(string outputDocsPath, string jsonFile, List<string> kdl_lines, string artifact_type = "")
        {
            var _fileName = Path.GetFileNameWithoutExtension(jsonFile);
            var previewUrl = _fileName + ".html";
            var fileContents = File.ReadAllText(jsonFile);
            if (artifact_type == "form")
            {
                var form = JsonConvert.DeserializeObject<Form>(fileContents);
                var html = GenerateFormHtml(form);
                File.WriteAllText(Path.Combine(outputDocsPath, previewUrl), html);
            }
            else if (artifact_type == "checklist")
            {
                var checklist = JsonConvert.DeserializeObject<Checklist>(fileContents);
                var html = GenerateChecklistHtml(checklist);
                File.WriteAllText(Path.Combine(outputDocsPath, previewUrl), html);
            }
            else
            {
                var title = GetArtifactTitle(_fileName);
                var html = ConvertMarkdownToHtml(fileContents, title);
                // var parser = new TextParser();
                // try
                // {
                //     parser.Convert(jsonFile, Path.Combine("/App Store Docs", previewUrl));
                // }
                // catch (Exception e)
                // {
                //     CommandProcessor.LogJsonParsingError(e, e.Message, jsonFile);
                // }
                File.WriteAllText(Path.Combine(outputDocsPath, previewUrl), html);
            }

            kdl_lines.Add("artifact_preview_section {");
            kdl_lines.Add($"\theading \"{GetArtifactTitle(_fileName)}\"");
            kdl_lines.Add($"\tsub_heading \"{_fileName}\"");
            kdl_lines.Add($"\timg \"\"");
            kdl_lines.Add($"\turl \"{Path.Combine("/App Store Docs", previewUrl)}\"");
            kdl_lines.Add("}");
        }

        private static string GenerateFormHtml(Form form)
        {
            // Parse the JSON string into a list of form fields
            List<FormField> formFields = form.Form_Fields;

            // Start building the HTML form
            List<string> lines = new List<string>();
            lines.Add(formPreviewCss);
            lines.Add("<h1> "+form.Form_Title+" </h1>");
            lines.Add("<form>");

            // Loop through each form field and add the appropriate HTML
            foreach (FormField field in formFields)
            {
                // Start building the HTML for this field
                lines.Add("<div class=\"form-group\">");
                lines.Add($"<label for=\"{field.Form_Field.Replace(" ", "")}\">{field.Form_Field}</label>");
                if (!string.IsNullOrEmpty(field.Hint)){
                    lines.Add($"<span class=\"fa fa-info-circle\" title=\"{field.Hint}\"></span>");
                }
                var required_flag = false;
                // if field.Validation is string and is equal to "Required" or if field.Validation is json and contains "required" : true
                try{
                    if ((field.Validation != null && field.Validation.Contains("Required") || field.Validation.Contains("Mandatory")) || (field.Validation != null && field.Validation.Contains("required")))
                    {
                        required_flag = true;
                    }
                }
                catch (Exception e)
                {
                    CommandProcessor.LogJsonParsingError(e, e.Message, "validation parsing error in " + form.Form_Title);
                }
                // Add the appropriate HTML based on the field type
                switch (field.Field_Type.ToLower())
                {
                    case "textbox": case "text":
                        lines.Add($"<input type=\"text\" class=\"form-control\" id=\"{field.Form_Field.Replace(" ", "")}\" placeholder=\"{field.Description}\" {(required_flag ? "required" : "")}>");
                        break;
                    case "Choices":
                        lines.Add("<select class=\"form-control\" id=\"" + field.Form_Field.Replace(" ", "") + "\" " + (required_flag ? "required" : "") + ">");
                        lines.Add("<option value=\"\">" + field.Description + "</option>");
                        foreach (string option in field.Options)
                        {
                            lines.Add("<option value=\"" + option + "\">" + option + "</option>");
                        }
                        lines.Add("</select>");
                        break;
                    case "number":
                        lines.Add($"<input type=\"number\" class=\"form-control\" id=\"{field.Form_Field.Replace(" ", "")}\" placeholder=\"{field.Description}\" {(required_flag ? "required" : "")}>");
                        break;
                    case "date picker": case "date": case "datepicker":
                        lines.Add($"<input type=\"date\" class=\"form-control\" id=\"{field.Form_Field.Replace(" ", "")}\" {(required_flag ? "required" : "")}>");
                        break;
                    case "time picker": case "time": case "timepicker":
                        lines.Add($"<input type=\"time\" class=\"form-control\" id=\"{field.Form_Field.Replace(" ", "")}\" {(required_flag ? "required" : "")}>");
                        break;
                    case "textarea": case "text area":
                        lines.Add($"<textarea class=\"form-control\" id=\"{field.Form_Field.Replace(" ", "")}\" rows=\"3\" placeholder=\"{field.Description}\" {(required_flag ? "required" : "")}></textarea>");
                        break;
                    case "picture upload": case "picture": case "image": case "image upload":
                        lines.Add($"<input type=\"file\" class=\"form-control-file\" id=\"{field.Form_Field.Replace(" ", "")}\">");
                        break;
                    case "video capture": case "video": case "video upload":
                        lines.Add($"<input type=\"file\" accept=\"video/*\" capture=\"camera\" class=\"form-control-file\" id=\"{field.Form_Field.Replace(" ", "")}\">");
                        break;
                    case "audio capture": case "audio": case "audio upload":
                        lines.Add($"<input type=\"file\" accept=\"audio/*\" capture=\"microphone\" class=\"form-control-file\" id=\"{field.Form_Field.Replace(" ", "")}\">");
                        break;
                    case "gps location": case "location": case "gps": case "geolocation":
                        lines.Add($"<input type=\"text\" class=\"form-control\" id=\"{field.Form_Field.Replace(" ", "")}\" placeholder=\"{field.Description}\" {(required_flag ? "required" : "")}>");
                        break;
                    case "file upload": case "file": case "file picker": case "filepicker": case "document": case "document upload":
                        lines.Add($"<input type=\"file\" class=\"form-control-file\" id=\"{field.Form_Field.Replace(" ", "")}\">");
                        break;
                    case "default":
                        lines.Add($"<input type=\"text\" class=\"form-control\" id=\"{field.Form_Field.Replace(" ", "")}\" placeholder=\"{field.Description}\" {(required_flag ? "required" : "")}>");
                        break;
                }
                lines.Add("</div>");
            }
            lines.Add("</div>");
            lines.Add("<div class='form-group submit-btn'>");
            lines.Add("<input type='submit' value='Submit' class='btn btn-primary' />");
            lines.Add("</div>");
            lines.Add("</form>");
            lines.Add("</div>");
            lines.Add("</body>");
            lines.Add("</html>");

            return string.Join("\n", lines);
        }
        private static string GenerateChecklistHtml(Checklist checklist)
        {
            // Parse the JSON string into a list of checkpoints
            List<ChecklistPoint> checkpoints = checklist.Checkpoints;

            // Start building the HTML form
            List<string> lines = new List<string>();
            lines.Add(formPreviewCss);
            lines.Add("<h1> "+checklist.Checklist_Title+" </h1>");
            lines.Add("<p> "+checklist.Checklist_Description+" </p>");
            lines.Add("<form>");

            // Loop through each form field and add the appropriate HTML
            foreach (ChecklistPoint checkpoint in checkpoints)
            {
                // Start building the HTML for this field
                lines.Add("<div class=\"form-group\">");
                lines.Add($"<label for=\"{checkpoint.Checkpoint.Replace(" ", "")}\">{checkpoint.Checkpoint}</label>");
                if (!string.IsNullOrEmpty(checkpoint.Hint)){
                    lines.Add($"<span class=\"fa fa-info-circle checkpoint-hint\" title=\"{checkpoint.Hint}\"></span>");
                }
                var required_flag = false;
                // if field.Validation is string and is equal to "Required" or if field.Validation is json and contains "required" : true
                try{
                    if(checkpoint.Validation != null)
                    {
                        if (checkpoint.Validation.Contains("Required") || checkpoint.Validation.Contains("Mandatory") || checkpoint.Validation.Contains("required"))
                        {
                            required_flag = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    CommandProcessor.LogJsonParsingError(e, e.Message, "validation parsing error in " + checklist.Checklist_Title);
                }
                
                lines.Add("<select class=\"form-control\" id=\"" + checkpoint.Checkpoint.Replace(" ", "") + "\" " + (required_flag ? "required" : "") + ">");
                //if checkpoint.Options is null, set options as Ok, Not Ok and N/A
                if (checkpoint.Options == null)
                {
                    lines.Add("<option value=\"Ok\">Ok</option>");
                    lines.Add("<option value=\"Not Ok\">Not Ok</option>");
                    lines.Add("<option value=\"N/A\">N/A</option>");
                }
                else
                {
                    foreach (string option in checkpoint.Options)
                    {
                        lines.Add("<option value=\"" + option + "\">" + option + "</option>");
                    }
                }
                lines.Add("</select>");

                lines.Add("</div>");
            }
            lines.Add("<div class='form-group submit-btn'>");
            lines.Add("<input type='submit' value='Submit' class='btn btn-primary' />");
            lines.Add("</div>");
            lines.Add("</form>");
            lines.Add("</div>");
            lines.Add("</body>");
            lines.Add("</html>");

            return string.Join("\n", lines);
        }
        public static string ConvertMarkdownToHtml(string markdown, string title = "")
        {
            string[] lines = markdown.Split('\n');
            string html = "";

            html += $"<h1>{title}</h1>";
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.StartsWith("#"))
                {
                    int headingLevel = 1;

                    while (line.StartsWith("#"))
                    {
                        headingLevel++;
                        line = line.Substring(1);
                    }

                    html += $"<h{headingLevel}>{line.Trim()}</h{headingLevel}>";
                }
                else if (line.StartsWith("|"))
                {
                    string[] cells = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    html += "<table><tr>";

                    for (int j = 0; j < cells.Length; j++)
                    {
                        html += $"<th>{cells[j].Trim()}</th>";
                    }

                    html += "</tr>";
                    i++;
                    i++; // Skip the separator line

                    while (i < lines.Length && lines[i].StartsWith("|"))
                    {
                        cells = lines[i].Split('|', StringSplitOptions.RemoveEmptyEntries);
                        html += "<tr>";

                        for (int j = 0; j < cells.Length; j++)
                        {
                            html += $"<td>{cells[j].Trim()}</td>";
                        }

                        html += "</tr>";
                        i++;
                    }

                    html += "</table>";
                    i--;
                }
                else if (line.StartsWith("*"))
                {
                    html += $"<ul><li>{line.Substring(1).Trim()}</li>";

                    while (i + 1 < lines.Length && lines[i + 1].StartsWith("*"))
                    {
                        i++;
                        html += $"<li>{lines[i].Substring(1).Trim()}</li>";
                    }

                    html += "</ul>";
                }
                else
                {
                    html += $"<p>{line.Trim()}</p>";
                }
            }

            return html;
        }
        public static string GetArtifactTitle(string filename)
        {
            // Split the filename into parts using hyphen as delimiter
            var parts = filename.Split('-');

            // Determine the artifact type based on the second part of the filename
            var type = parts[0].Trim().ToLowerInvariant();

            // Determine the artifact title based on the remaining parts of the filename
            var titleParts = parts.Skip(1).Select(p => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(p.Trim().ToLowerInvariant()));
            if(type == "audit"){
                titleParts = parts.Skip(2).Select(p => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(p.Trim().ToLowerInvariant()));
            }
            var title = string.Join(" ", titleParts);

            return title;
        }
        // private static string ToTitleCase(this string str)
        // {
        //     return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLowerInvariant());
        // }


        private static object getArtifactObject(string artifactFile)
        {
            if (artifactFile.StartsWith("Form")){
                return JsonConvert.DeserializeObject<Form>(File.ReadAllText(artifactFile));
            }
            else if (artifactFile.StartsWith("Checklist")){
                return JsonConvert.DeserializeObject<Checklist>(File.ReadAllText(artifactFile));
            }
            else if (artifactFile.StartsWith("Report")){
                return JsonConvert.DeserializeObject<Report>(File.ReadAllText(artifactFile));
            }
            else if (artifactFile.StartsWith("Dashboard")){
                return JsonConvert.DeserializeObject<Dashboard>(File.ReadAllText(artifactFile));
            }
            else if (artifactFile.StartsWith("Integration")){
                return JsonConvert.DeserializeObject<Integration>(File.ReadAllText(artifactFile));
            }
            else if (artifactFile.StartsWith("AuditChecklist")){
                return JsonConvert.DeserializeObject<AuditChecklist>(File.ReadAllText(artifactFile));
            }
            else if (artifactFile.StartsWith("Workflow")){
                return JsonConvert.DeserializeObject<Workflow>(File.ReadAllText(artifactFile));
            }
            else if (artifactFile.StartsWith("Guideline")){
                return JsonConvert.DeserializeObject<Guideline>(File.ReadAllText(artifactFile));
            }
            else if (artifactFile.StartsWith("Standard")){
                return JsonConvert.DeserializeObject<Standard>(File.ReadAllText(artifactFile));
            }
            else{
                return null;
            }
        }
    }
}