using System;
using System.Collections.Generic;
using System.Linq;

namespace WebMagic
{

    public class BlogSection
    {
        public string Type { get; set; }
    }

    public class ParaHeadingSection : BlogSection
    {
        public string Heading { get; set; }
    }

    public class ParaSection : BlogSection
    {
        public string Contents { get; set; }
    }

    public class BulletsSection : BlogSection
    {
        public List<string> Bullets { get; set; }
    }

    public class TableSection : BlogSection
    {
        public List<string> Header { get; set; }
        public List<List<string>> Rows { get; set; }
    }

    public class TextParser
    {
        public List<BlogSection> Parse(string inputFilePath)
        {
            // Load MD from file
            string input = File.ReadAllText(inputFilePath);
            var lines = input.Split('\n');

            var sections = new List<BlogSection>();

            var currentType = "";
            var currentHeading = "";
            var currentBullets = new List<string>();
            var currentTableHeader = new List<string>();
            var currentTableRows = new List<List<string>>();

            foreach (var line in lines)
            {
                if (line.StartsWith("----"))
                {
                    currentType = "heading";
                }
                else if (line.StartsWith("*"))
                {
                    if (currentType != "bullets")
                    {
                        if (currentType != "")
                        {
                            sections.Add(CreateSection(currentType, currentHeading, currentBullets, currentTableHeader, currentTableRows));
                        }
                        currentType = "bullets";
                        currentHeading = "";
                        currentBullets = new List<string>();
                        currentTableHeader = new List<string>();
                        currentTableRows = new List<List<string>>();
                    }
                    currentBullets.Add(line.Trim());
                }
                else if (line.StartsWith("|"))
                {
                    if (currentType != "table")
                    {
                        if (currentType != "")
                        {
                            sections.Add(CreateSection(currentType, currentHeading, currentBullets, currentTableHeader, currentTableRows));
                        }
                        currentType = "table";
                        currentHeading = "";
                        currentBullets = new List<string>();
                        currentTableHeader = new List<string>();
                        currentTableRows = new List<List<string>>();
                    }
                    var row = line.Split('|').Skip(1).TakeWhile(s => s.Trim() != "").Select(s => s.Trim()).ToList();
                    if (currentTableHeader.Count == 0)
                    {
                        currentTableHeader = row;
                    }
                    else
                    {
                        currentTableRows.Add(row);
                    }
                }
                else
                {
                    if (currentType != "paragraph")
                    {
                        if (currentType != "")
                        {
                            sections.Add(CreateSection(currentType, currentHeading, currentBullets, currentTableHeader, currentTableRows));
                        }
                        currentType = "paragraph";
                        currentHeading = line.Trim();
                        currentBullets = new List<string>();
                        currentTableHeader = new List<string>();
                        currentTableRows = new List<List<string>>();
                    }
                    else
                    {
                        currentHeading += "\n" + line.Trim();
                    }
                }
            }

            if (currentType != "")
            {
                sections.Add(CreateSection(currentType, currentHeading, currentBullets, currentTableHeader, currentTableRows));
            }
            
            return sections;
        }

        private BlogSection CreateSection(string type, string heading, List<string> bullets, List<string> tableHeader, List<List<string>> tableRows)
        {
            switch (type)
            {
                case "heading":
                    return new ParaHeadingSection { Type = type, Heading = heading };
                case "bullets":
                    return new BulletsSection { Type = type, Bullets = bullets };
                case "table":
                    return new TableSection { Type = type, Header = tableHeader, Rows = tableRows };
                default:
                    return new ParaSection { Type = type, Contents = heading };
            }
        }

        public void Convert(string inputFilePath)
        {
            var sections = Parse(inputFilePath);
            var kdl_lines = new List<string>();

            foreach (var section in sections)
            {
                switch (section)
                {
                    case ParaHeadingSection phs:
                        kdl_lines.Add("para_heading_section {\n\theading \"" + phs.Heading.Trim() + "\"\n}");
                        break;
                    case ParaSection ps:
                        kdl_lines.Add("para_section {\n\tcontents \"" + ps.Contents.Trim() + "\" }");
                        break;
                    case BulletsSection bs:
                        kdl_lines.Add("\nbullets_section {");
                        foreach (var bullet in bs.Bullets)
                        {
                            kdl_lines.Add($"\n\tbullet \"{bullet.Trim()}\"");
                        }
                        kdl_lines.Add("\n}");
                        break;
                    case TableSection ts:
                        kdl_lines.Add("\ntable_section {");
                        kdl_lines.Add($"\n\theader {string.Join(" ", ts.Header.Select(str => $"\"{str}\""))}");
                        foreach (var row in ts.Rows)
                        {
                            kdl_lines.Add($"\n\trow {{ {string.Join(" ", ts.Header.Select(str => $"\"{str}\""))} }}");
                        }
                        kdl_lines.Add("\n}");
                        break;
                }
            }
            using (StreamWriter writer = new StreamWriter("/Users/arohikulkarni/Work/Einstein/SiteExtraction/output.page"))
            {
                foreach (string line in kdl_lines)
                {
                    Console.WriteLine(line);
                    writer.WriteLine(line);
                }
            }
        }
    
    }


}