using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebMagic
{
    public class MDSection
    {
        public static string page_type = "landing";
        public string Type { get; set; }
    }

    public class HeadingSection : MDSection
    {
        public string Heading { get; set; }
        public string Date { get; set; }
        public string Author { get; set; }
    }
    public class MDClientLogosSection : MDSection
    {
    }
    public class MDRatingSection : MDSection
    {
    }
    public class MDIntegrationsSection : MDSection
    {
    }
    public class MDIndustriesSection : MDSection
    {
    }
    public class MDGetQuoteSection : MDSection
    {
    }
    public class MDScheduleDemoSection : MDSection
    {
    }
    public class MDRelatedResourcesSection : MDSection
    {
    }
    public class MDHeroSection : MDSection
    {
        public string Heading { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Alt { get; set; }
        public List<Link> links { get; set; }
    }
    public class MDHeroWithBgSection : MDSection
    {
        public string Heading { get; set; }
        public string Description { get; set; }
        public List<Link> links { get; set; }
    }

    public class Link
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public string Type { get; set; } //"primary" or "secondary"
    }
    public class SubHeadingSection : MDSection
    {
        public string SubHeading { get; set; }
    }
    public class MDGroupHeaderSection : MDSection
    {
        public string Heading { get; set; }
        public string SubHeading { get; set; }
    }

    public class ImageSection : MDSection
    {
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string Source { get; set; }
        public string Alt { get; set; }
        public string Align { get; internal set; }
    }

    public class ParaSection : MDSection
    {
        public string Contents { get; set; }
    }

    public class MDFAQSection : MDSection
    {
        public List<QuestionAndAnswer> QuestionsAndAnswers { get; set; }
    }

    public class QuestionAndAnswer
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public class MDAppsForIndustrySection : MDSection
    {
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public List<MDAppForIndustry> AppsForIndustry { get; set; }
    }
    public class MDAppForIndustry
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
    }
    public class BulletsSection : MDSection
    {
        public List<string> Bullets { get; set; }
    }

    public class TableSection : MDSection
    {
        public List<string> Header { get; set; }
        public List<List<string>> Rows { get; set; }
    }

    public class SectionObject
    {
        internal object app_names;

        public string type { get; set; }
        public string heading { get; set; }
        public string date { get; set; }
        public string author { get; set; }
        public string subheading { get; set; }
        public string source { get; set; }
        public string alt { get; set; }
        public string align { get; set; }
        public List<string> bullets { get; set; }
        public List<string> tableHeader { get; set; }
        public List<List<string>> tableRows { get; set; }
        public string contents { get; set; }
        public List<Link> links { get; set; }
        public List<QuestionAndAnswer> questions_and_answers { get; set; }
        public List<MDAppForIndustry> appsForIndustry { get; set; }
    }

    public class TextParser
    {
        public List<MDSection> Parse(string inputFilePath)
        {
            // Load MD from file
            string input = File.ReadAllText(inputFilePath);

            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            lines = DetectIndustriesSection(lines);
            lines = DetectAppsforIndustrySection(lines);

            var sections = new List<MDSection>();

            for (var i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                var section_obj = new SectionObject();
                if (line.StartsWith("[") && line.Contains("](") && line.Contains(")") || line.Contains("elementor"))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line) && !Regex.IsMatch(line, @"^[\s@<>=-]*$"))
                {
                    section_obj = DetectSection(lines, i);
                    if (section_obj.type == "undetected")
                    {
                        continue;
                    }
                    sections.Add(CreateSection(section_obj));
                    // if image section with detected, skip next 2 lines
                    if (section_obj.type == "image" && !string.IsNullOrEmpty(section_obj.heading))
                    {
                        i += 2;
                    }
                    if (section_obj.type == "heading")
                    {
                        i += 3;
                    }
                    if (section_obj.type == "bullets")
                    {
                        i += section_obj.bullets.Count;
                    }
                    if (section_obj.type == "table")
                    {
                        i += section_obj.tableRows.Count + 1;
                    }
                    if (section_obj.type == "hero_section")
                    {
                        if (section_obj.links.Count > 0)
                            i += section_obj.links.Count + 3;
                        else
                            i += 3;
                    }
                    if (section_obj.type == "hero_with_bg_section")
                    {
                        if (section_obj.links.Count > 0)
                            i += section_obj.links.Count + 2;
                        else
                            i += 2;
                    }
                    if (section_obj.type == "group_header" && !string.IsNullOrEmpty(section_obj.subheading))
                    { //skip ---- and subheading lines
                        i += 2;
                    }
                    if (section_obj.type == "integrations_section")
                    { // skip till next subheading
                        i += 5;
                    }
                    if (section_obj.type == "faq_section" && section_obj.questions_and_answers != null && section_obj.questions_and_answers.Count > 0)
                    { // skip for all q&as and one ---- line
                        i += section_obj.questions_and_answers.Count * 2 + 1;
                    }
                    if (section_obj.type == "related_resources_section")
                    { // skip 5 lines of 4 resource blocks each
                        i += 20 + 1;
                    }
                    if (section_obj.type == "schedule_demo_section")
                    { // skip 2 lines of subheading and link and 1 line of ----
                        i += 3;
                    }
                    if (section_obj.type == "rating_section")
                    { // skip 14 lines of award images and star images
                        i += 14;
                    }
                    if (section_obj.type == "apps_for_industry_section")
                    { // skip 2 lines of subheading and 3 of each app
                        i += 2 + section_obj.appsForIndustry.Count * 3;
                    }
                }
            }
            return sections;
        }

        private string[] DetectIndustriesSection(string[] lines)
        {
            string explore_industries = "Explore All Industries";
            List<string> linesList = new List<string>(lines);
            int exploreIndustriesIndex = Array.FindIndex(lines, line => line.Contains(explore_industries));
            if (exploreIndustriesIndex != -1)
            {
                // find start index by looking for the line having "-----" before the exploreIndustriesIndex
                int startIndex = Array.FindLastIndex(lines, exploreIndustriesIndex, line => line.Contains("-----"));
                if (startIndex != -1)
                {
                    // remove the lines from startIndex + 1 to exploreIndustriesIndex and add "industries_section" to lines
                    linesList.RemoveRange(startIndex + 1, exploreIndustriesIndex - startIndex - 1);
                    linesList.Insert(startIndex + 1, "industries_section");

                    lines = linesList.ToArray();
                }
            }
            return lines;
        }

        private string[] DetectAppsforIndustrySection(string[] lines)
        {
            string phone_img = "Group-1210.png";
            List<string> linesList = new List<string>(lines);
            int phoneImgIndex = Array.FindIndex(lines, line => line.Contains(phone_img));
            if (phoneImgIndex != -1)
            {
                int endIndex = Array.FindIndex(lines, phoneImgIndex, line => line.Contains("Explore Micro-App Store"));
                if (endIndex != -1)
                {
                    var heading = lines[phoneImgIndex - 3];
                    var subheading = lines[phoneImgIndex - 1];
                    
                    linesList.RemoveRange(phoneImgIndex - 4, 5);
                    linesList.Insert(phoneImgIndex - 4, "## Apps for Industry");
                    linesList.Insert(phoneImgIndex - 3, heading);
                    linesList.Insert(phoneImgIndex - 2, subheading);

                    lines = linesList.ToArray();
                }
            }
            return lines;
        }

        private static SectionObject DetectSection(string[] lines, int i)
        {
            var section_obj = new SectionObject();
            var _idx = i;
            var line = lines[_idx].Trim();
            char[] trimChars = { '_', '-', ' ', '*', '#' };

            List<string> single_line_sections = new List<string>();
            // single_line_sections.Add("subheading");
            single_line_sections.Add("paragraph");

            section_obj = DetectLine(lines, i);
            if(single_line_sections.Contains(section_obj.type)){
                return section_obj;
            }
            else if (section_obj.type == "heading"){
                // next line will be ======
                //if next line starts with bullet, then that's the author
                var next_section_obj = DetectLine(lines, i+2);
                //if next to next line starts with bullet, then that's the date
                var next_next_section_obj = DetectLine(lines, i+3);
                if (next_section_obj.type == "bullets" && next_next_section_obj.type == "bullets"){
                    section_obj.type = "heading";
                    section_obj.author = next_section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars);
                    section_obj.date = next_next_section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars);
                    MDSection.page_type = "blog";
                    return section_obj;
                }
                else
                {
                    section_obj = CheckIfHero(lines, i, section_obj, next_section_obj, next_next_section_obj);
                    if (section_obj.type == "hero_section"){
                        MDSection.page_type = "landing";
                        return section_obj;
                    }
                    else if(section_obj.type == "hero_with_bg_section"){
                        MDSection.page_type = "industry";
                        return section_obj;
                    }
                }

            }
            else if (section_obj.type == "subheading" && (MDSection.page_type == "landing" || MDSection.page_type == "industry")){
                // next line will be -----
                // This could be a group header or a grid section if more than 2 images found after this
                
                section_obj.type = "group_header";
                section_obj.heading = section_obj.subheading.TrimStart(trimChars).TrimEnd(trimChars);
                // if next next line is paragraph, then the groupheader also has subheading
                var next_section_obj = DetectLine(lines, i+2);
                if (next_section_obj.type == "paragraph"){
                    section_obj.subheading = next_section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars);
                    return section_obj;
                }
                else{
                    section_obj.subheading = "";
                    return section_obj;
                }
            }
            else if (section_obj.type == "bullets"){
                // while next line starts with bullet, add to bullets
                section_obj.bullets = new List<string>();
                section_obj.bullets.Add(section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars));
                var bullet_cnt = 1;
                var next_section_obj = DetectLine(lines, i);
                // while the next section is bullet or undetected and empty
                while (next_section_obj.type == "bullets"){// || (next_section_obj.type == "undetected" && string.IsNullOrEmpty(next_section_obj.contents))){
                    // skip adding if the content is empty
                    var bullet_contents = next_section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars);
                    if (!string.IsNullOrEmpty(bullet_contents)){
                        section_obj.bullets.Add(bullet_contents);
                    }
                    bullet_cnt++;
                    next_section_obj = DetectLine(lines, i+bullet_cnt);
                }
            }
            else if (section_obj.type == "h3"){
                var next_section_obj = DetectLine(lines, i+1);
                if (next_section_obj.type == "paragraph"){
                    var next_next_section_obj = DetectLine(lines, i+2);
                    if (next_next_section_obj.type == "image"){
                        //right image section found
                        section_obj.type = "image";
                        section_obj.source = next_next_section_obj.source;
                        section_obj.alt = next_next_section_obj.alt;
                        section_obj.align = "right";
                        section_obj.heading = section_obj.heading.TrimStart(trimChars).TrimEnd(trimChars);
                        section_obj.subheading = next_section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars);
                        return section_obj;
                    }
                }
            }
            else if(section_obj.type == "image"){
                var next_section_obj = DetectLine(lines, i+1);
                if (next_section_obj.type == "h3"){
                    var next_next_section_obj = DetectLine(lines, i+2);
                    if (next_next_section_obj.type == "paragraph"){
                        //left image section found
                        section_obj.type = "image";
                        section_obj.source = section_obj.source;
                        section_obj.alt = section_obj.alt;
                        section_obj.align = "left";
                        section_obj.heading = next_section_obj.heading.TrimStart(trimChars).TrimEnd(trimChars);
                        section_obj.subheading = next_next_section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars);
                        return section_obj;
                    }
                }
            }
            else if (section_obj.type == "table"){
                // while next line is table, add to table
                section_obj.tableHeader = new List<string>();
                section_obj.tableRows = new List<List<string>>();
                section_obj.tableHeader = section_obj.contents.Split('|').Skip(1).TakeWhile(s => s.Trim() != "").Select(s => s.Trim()).ToList();
                section_obj.tableRows = section_obj.tableRows;
                var row_cnt = 0;
                var next_section_obj = DetectLine(lines, i+row_cnt+1);
                while (next_section_obj.type == "table"){
                    // skip adding if the content is empty
                    var row_contents = next_section_obj.contents.Split('|').Skip(1).TakeWhile(s => s.Trim() != "").Select(s => s.Trim()).ToList();
                    if (!string.IsNullOrEmpty(row_contents[0]) && !row_contents[0].Contains("-")){
                        section_obj.tableRows.Add(next_section_obj.contents.Split('|').Skip(1).TakeWhile(s => s.Trim() != "").Select(s => s.Trim()).ToList());
                    }
                    row_cnt++;
                    next_section_obj = DetectLine(lines, i+row_cnt);
                }
            }
            else if (section_obj.type == "faq_section"){
                i += 1; // next will be ----
                while (true){
                    var question_and_answer = new QuestionAndAnswer();
                    var next_section_obj = DetectLine(lines, i+1);
                    if (next_section_obj.type == "question"){
                        question_and_answer.Question = next_section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars);
                        var next_next_section_obj = DetectLine(lines, i+2);
                        if (next_next_section_obj.type == "paragraph"){
                            // initialize the list of questions and answers if it is null
                            if (section_obj.questions_and_answers == null){
                                section_obj.questions_and_answers = new List<QuestionAndAnswer>();
                            }
                            question_and_answer.Answer = next_next_section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars);
                            section_obj.questions_and_answers.Add(question_and_answer);
                            i = i+2;
                        }
                        else{
                            break;
                        }
                    }
                    else{
                        break;
                    }
                }
            }
            else if (section_obj.type == "apps_for_industry_section"){
                section_obj.heading = lines[i+1];
                section_obj.subheading = lines[i+2];
                i += 3;
                // get four app name, url and image after phone image and insert them in the list
                var apps = new List<MDAppForIndustry>();
                while (!lines[i].Contains("Explore Micro-App Store"))
                {
                    var app = new MDAppForIndustry();
                    SectionObject img_obj = DetectLine(lines, i);
                    SectionObject link_obj = DetectLine(lines, i + 1);
                    var app_img = img_obj.source;
                    var app_url = link_obj.links[0].Url;
                    var app_name = link_obj.links[0].Text.TrimStart(trimChars).TrimEnd(trimChars);
                    app.Name = app_name;
                    app.Url = app_url;
                    app.Image = app_img;
                    apps.Add(app);
                    i += 4;
                }
                section_obj.appsForIndustry = apps;
            }
            return section_obj;
        }

        private static SectionObject CheckIfHero(string[] lines, int i, SectionObject section_obj, SectionObject next_section_obj, SectionObject next_next_section_obj)
        {
            string[] sectionTypes = { "paragraph", "image", "link" };
            bool[] sectionFound = { false, false, false };
            int linkCount = 0;
            SectionObject paragraphSection = null;
            SectionObject imageSection = null;
            var next_third_section_obj = DetectLine(lines, i + 4);
            var next_fourth_section_obj = DetectLine(lines, i + 5);

            var sections = new List<SectionObject>();
            sections.Add(next_section_obj);
            sections.Add(next_next_section_obj);
            sections.Add(next_third_section_obj);
            sections.Add(next_fourth_section_obj);
            foreach (var section in sections)
            {
                if (sectionTypes.Contains(section.type))
                {
                    switch (section.type)
                    {
                        case "paragraph":
                            sectionFound[0] = true;
                            paragraphSection = section;
                            break;

                        case "image":
                            sectionFound[1] = true;
                            imageSection = section;
                            break;

                        case "link":
                            sectionFound[2] = true;
                            linkCount++;

                            if (linkCount == 1)
                            {
                                section.type = "link";
                                section_obj.links = new List<Link>();
                                section.links[0].Type = "primary";
                                section_obj.links.Add(section.links[0]);
                            }
                            else if (linkCount == 2)
                            {
                                section.type = "link";
                                section.links[0].Type = "secondary";
                                section_obj.links.Add(section.links[0]);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            if (sectionFound.All(x => x == true) && linkCount > 0)
            {
                section_obj.type = "hero_section";
                // section_obj.heading = section_obj.heading;
                section_obj.subheading = paragraphSection.contents;
                section_obj.source = imageSection.source;
                section_obj.alt = imageSection.alt;
            }
            if (sectionFound[0]) { //Heading wiht description found
                section_obj.type = "hero_with_bg_section";
                section_obj.subheading = paragraphSection.contents;
            }
            return section_obj;
        }

        private static SectionObject DetectLine(string[] lines, int i)
        {
            var section_obj = new SectionObject();
            if (i >= lines.Length)
            {
                return section_obj;
            }
            var line = lines[i].Trim();
            char[] trimChars = new char[] { '"', ' ', '.', '#', '*' };

            Match img_match = new Regex(@"!\[(.*?)\]\((.*?)\)").Match(lines[i]);
            Match link_match = new Regex(@"^\s*(?:\[(.*?)\]\((.*?)\))?$").Match(lines[i]);
            // check if the next line contains multiple = or - characters
            if (i < lines.Length-1 && lines[i + 1].Trim().StartsWith("="))
            {
                // heading found
                section_obj.type = "heading";
                section_obj.heading = lines[i].TrimStart(trimChars).TrimEnd(trimChars);
            }
            else if ((lines[i].Trim().Contains("Integration") || lines[i].Trim().Contains("Integrate")) && MDSection.page_type != "blog"){
                section_obj.type = "integrations_section";
            }
            else if (lines[i].Trim().Contains("## Apps for Industry")){
                section_obj.type = "apps_for_industry_section";
            }
            else if (lines[i].Trim().Contains("industries_section")){
                section_obj.type = "industries_section";
            }
            else if (img_match.Success)
            {
                // image found
                section_obj.type = "image";
                section_obj.source = Path.Combine("/","assets","images",Path.GetFileName(img_match.Groups[2].Value));
                section_obj.alt = img_match.Groups[1].Value;
            }
            else if (link_match.Success)
            {
                // link found
                section_obj.type = "link";
                var link = new Link();
                link.Text = link_match.Groups[1].Value.TrimStart(trimChars).TrimEnd(trimChars);
                link.Url = link_match.Groups[2].Value.Replace("https://axonator.com/", "/");
                section_obj.links = new List<Link>();
                section_obj.links.Add(link);
            }
            else if ((i < lines.Length-1 && lines[i + 1].Trim().StartsWith("-")) || lines[i].Trim().StartsWith("**"))
            {
                if (lines[i].Trim().Contains("Fortune 500 Companies Trust Us")){
                    section_obj.type = "client_logos_section";
                }
                else if (lines[i].Trim().Contains("Consistently Rated Best-in-Class")){
                    section_obj.type = "rating_section";
                }
                else if (lines[i].Trim().Contains("Quick Estimate")){
                    section_obj.type = "get_quote_section";
                }
                else if (lines[i].Trim().Contains("Resources")){
                    section_obj.type = "related_resources_section";
                }
                else if (lines[i].Trim().Contains("Schedule Your Demo Today")){
                    section_obj.type = "schedule_demo_section";
                }
                else if (lines[i].Trim().Contains("FAQ") || lines[i].Trim().Contains("Frequently Asked Question")){
                    section_obj.type = "faq_section";
                }
                else{
                    // subheading found
                    section_obj.type = "subheading";
                    section_obj.subheading = lines[i].TrimStart(trimChars).TrimEnd(trimChars);
                }
            }
            else if (lines[i].Trim().StartsWith("### "))
            {
                // h3 section
                section_obj.type = "h3";
                section_obj.heading = lines[i].TrimStart(trimChars).TrimEnd(trimChars);
            }
            else if (lines[i].Trim().StartsWith("*"))
            {
                // bullet section
                section_obj.type = "bullets";
                section_obj.contents = lines[i].TrimStart(trimChars).TrimEnd(trimChars);
            }
            else if (lines[i].Trim().StartsWith("|"))
            {
                // table section
                section_obj.type = "table";
                section_obj.contents = lines[i].TrimStart(trimChars).TrimEnd(trimChars);
            }
            else if(lines[i].Length > 20 && lines[i].Trim().EndsWith("?")){
                // question section
                section_obj.type = "question";
                section_obj.contents = lines[i].TrimStart(trimChars).TrimEnd(trimChars);
            }
            else if(lines[i].Length > 70){
                // paragraph section
                section_obj.type = "paragraph";
                section_obj.contents = lines[i].TrimStart(trimChars).TrimEnd(trimChars);
            }
            else
            {
                // undetected section
                section_obj.type = "undetected";
                section_obj.contents = lines[i].TrimStart(trimChars).TrimEnd(trimChars);
            }
            return section_obj;
        }

        private MDSection CreateSection(SectionObject section_obj)
        {
            var type = section_obj.type;
            var heading = section_obj.heading ?? "";
            var subheading = section_obj.subheading ?? "";
            var author = section_obj.author ?? "";
            var date = section_obj.date ?? "";
            var source = section_obj.source;
            var alt = section_obj.alt;
            var align = section_obj.align ?? "";
            var bullets = section_obj.bullets;
            var tableHeader = section_obj.tableHeader;
            var tableRows = section_obj.tableRows;
            var links = section_obj.links;
            var questions_and_answers = section_obj.questions_and_answers;
            var appsForIndustry = section_obj.appsForIndustry;
            var contents = "";
            char[] trimChars = new char[] { '"', ' ', '.', '#', '*' };
            if (!string.IsNullOrEmpty(section_obj.contents))
            {
                contents = section_obj.contents.TrimStart(trimChars).TrimEnd(trimChars);
            }

            switch (type)
            {
                case "heading":
                    return new HeadingSection { Type = type, Heading = heading, Author = author, Date = date };
                case "hero_section":
                    return new MDHeroSection { Type = type, Heading = heading, Description = subheading, Source = source, Alt = alt, links = links };
                case "hero_with_bg_section":
                    return new MDHeroWithBgSection { Type = type, Heading = heading, Description = subheading, links = links };
                case "subheading":
                    return new SubHeadingSection { Type = type, SubHeading = subheading };
                case "image":
                    return new ImageSection { Type = type, Source = source, Alt = alt, Heading = heading, SubHeading = subheading, Align = align };
                case "bullets":
                    return new BulletsSection { Type = type, Bullets = bullets };
                case "table":
                    return new TableSection { Type = type, Header = tableHeader, Rows = tableRows };
                case "client_logos_section":
                    return new MDClientLogosSection { Type = type };
                case "rating_section":
                    return new MDRatingSection { Type = type };
                case "related_resources_section":
                    return new MDRelatedResourcesSection { Type = type };
                case "integrations_section":
                    return new MDIntegrationsSection { Type = type };
                case "industries_section":
                    return new MDIndustriesSection { Type = type };
                case "apps_for_industry_section":
                    return new MDAppsForIndustrySection { Type = type, Heading = heading, SubHeading = subheading, AppsForIndustry = appsForIndustry};
                case "get_quote_section":
                    return new MDGetQuoteSection { Type = type };
                case "schedule_demo_section":
                    return new MDScheduleDemoSection { Type = type };
                case "group_header":
                    return new MDGroupHeaderSection { Type = type, Heading = heading, SubHeading = subheading};
                case "faq_section":
                    return new MDFAQSection { Type = type, QuestionsAndAnswers = questions_and_answers };
                default:
                    return new ParaSection { Type = type, Contents = !string.IsNullOrEmpty(contents) ? contents : heading };
            }
        }

        public void Convert(string inputFilePath, string outputFilePath = "")
        {
            var sections = Parse(inputFilePath);
            var kdl_lines = new List<string>();

            foreach (var section in sections)
            {
                switch (section)
                {
                    case MDHeroSection hero:
                        kdl_lines.Add("hero_section {\n"+
                        "\theading \"" + hero.Heading.Trim() + "\"\n"+
                        "\tsub_heading \"" + hero.Description.Trim() + "\"\n"+
                        "\timg {\n"+
                        "\t\tsrc \"" + hero.Source.Trim() + "\"\n"+
                        "\t\talt \"" + hero.Alt.Trim() + "\"\n"+
                        "\t}\n"+
                        appendLinks(hero.links)+
                        "}");
                        break;
                    case MDHeroWithBgSection hero:
                        kdl_lines.Add("hero_with_bg_section {\n" +
                        "\theading \"" + hero.Heading.Trim() + "\"\n" +
                        "\tsub_heading \"" + hero.Description.Trim() + "\"\n" +
                        appendLinks(hero.links) +
                        "}");
                        break;
                    case HeadingSection hs:
                        kdl_lines.Add("blog_heading_section {\n" +
                        "\theading \"" + hs.Heading.Trim() + "\"\n" +
                        "\tauthor \"" + hs.Author.Trim() + "\"\n" +
                        "\tdate \"" + hs.Date.Trim() + "\"\n" +
                        "\n}");
                        break;
                    case SubHeadingSection shs:
                        kdl_lines.Add("sub_heading_section {\n\tsub_heading \"" + shs.SubHeading.Trim() + "\"\n}");
                        break;
                    case MDGroupHeaderSection ghs:
                        kdl_lines.Add("group_header {\n" +
                        "\theading \"" + ghs.Heading.Trim() + "\"\n" +
                        "\tsub_heading \"" + ghs.SubHeading.Trim() + "\"\n" +
                        "\n}");
                        break;
                    case ParaSection ps:
                        kdl_lines.Add("para_section {\n\tcontents \"" + ps.Contents.Trim() + "\" \n}");
                        break;
                    case ImageSection isec:
                        kdl_lines.Add("image_section {");
                        if(isec.Heading.Trim() != ""){
                            kdl_lines.Add("\theading \"" + isec.Heading.Trim() + "\"");
                        }
                        if(isec.SubHeading.Trim() != ""){
                            kdl_lines.Add("\tdescription \"" + isec.SubHeading.Trim() + "\"");
                        }
                        kdl_lines.Add("\timg {");
                        kdl_lines.Add("\t\tsrc \"" + isec.Source.Trim() + "\"");
                        kdl_lines.Add("\t\talt \"" + isec.Alt.Trim() + "\"");
                        kdl_lines.Add("\t}");
                        if(isec.Align.Trim() != ""){
                            kdl_lines.Add("\talign \"" + isec.Align.Trim() + "\"");
                        }
                        kdl_lines.Add("}");
                        break;
                    case BulletsSection bs:
                        kdl_lines.Add("\nbullets_section {");
                        foreach (var bullet in bs.Bullets)
                        {
                            kdl_lines.Add($"\n\tbullet {{\n");
                            kdl_lines.Add($"\t\tcontents \"{bullet.Trim()}\"\n");
                            kdl_lines.Add($"\t}}\n");
                        }
                        kdl_lines.Add("\tbullet {}");
                        kdl_lines.Add("\n}");
                        break;
                    case TableSection ts:
                        kdl_lines.Add("\ntable_section {");
                        kdl_lines.Add($"\n\theader {string.Join(" ", ts.Header.Select(str => $"\"{str}\""))}");
                        foreach (var row in ts.Rows)
                        {
                            kdl_lines.Add($"\n\trow {string.Join(" ", row.Select(str => $"\"{str}\""))}");
                        }
                        kdl_lines.Add("\n}");
                        break;
                    case MDClientLogosSection cls:
                        kdl_lines.Add("\nclient_logos_section");
                        break;
                    case MDRatingSection rs:
                        kdl_lines.Add("\nrating_section");
                        break;
                    case MDIntegrationsSection ints:
                        kdl_lines.Add("\nintegrations_section");
                        break;
                    case MDIndustriesSection inds:
                        kdl_lines.Add("\nindustries_section");
                        break;
                    case MDGetQuoteSection gqs:
                        kdl_lines.Add("\nget_quote_section");
                        break;
                    case MDScheduleDemoSection sds:
                        kdl_lines.Add("\nschedule_demo_section");
                        break;
                    case MDRelatedResourcesSection rrs:
                        kdl_lines.Add("\nrelated_resources_section");
                        break;
                    case MDFAQSection faqs:
                        kdl_lines.Add("\nfaq_section {");
                        foreach (var qa in faqs.QuestionsAndAnswers)
                        {
                            kdl_lines.Add("\n\tquestion_answer {");
                            kdl_lines.Add($"\n\t\tquestion \"{qa.Question.Trim()}\"");
                            kdl_lines.Add($"\n\t\tanswer \"{qa.Answer.Trim()}\"");
                            kdl_lines.Add("\n\t}");
                        }
                        kdl_lines.Add("\n\tquestion_answer {}");
                        kdl_lines.Add("\n}");
                        break;
                    case MDAppsForIndustrySection afis:
                        kdl_lines.Add("\napps_for_industry_section {");
                        kdl_lines.Add($"\theading \"{afis.Heading.Trim()}\"");
                        kdl_lines.Add($"\tsub_heading \"{afis.SubHeading.Trim()}\"");
                        foreach (var app in afis.AppsForIndustry)
                        {
                            kdl_lines.Add("\tapp {");
                            kdl_lines.Add($"\t\tname \"{app.Name.Trim()}\"");
                            kdl_lines.Add($"\t\timg \"{app.Image.Trim()}\"");
                            kdl_lines.Add($"\t\turl \"{app.Url.Trim()}\"");
                            kdl_lines.Add("\t}");
                        }
                        kdl_lines.Add("\tapp {}");
                        kdl_lines.Add("}");
                        break;
                }
            }
            if(outputFilePath == "")
            {
                string prefix, fileName;
                S3Uploader.GetFileNameAndPrefix(inputFilePath, out prefix, out fileName); //filename will contain hyphens, so not used here
                prefix = prefix.Replace("-", " ").ToLower();
                char[] trimChars = { '_', '-' };
                outputFilePath = Path.Combine(GlobalPaths.ProjectFolder, prefix, Path.GetFileNameWithoutExtension(inputFilePath).TrimStart(trimChars).TrimEnd(trimChars) + ".page");
                Compiler.CreateOutputDirectory(outputFilePath);
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
                        "cta_section",
                        "default_footer_section",
                        "page_end_section"
                    });
                    foreach (string line in kdl_lines)
                    {
                        Console.WriteLine(line);
                        writer.WriteLine(line.Replace("https://axonator.com/", "/"));
                    }
                    VerifyAndCorrectKdlFile(outputFilePath);
                }
            }
            else{
                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    foreach (string line in kdl_lines)
                    {
                        Console.WriteLine(line);
                        writer.WriteLine(line.Replace("https://axonator.com/", "/"));
                    }
                }
            }
        }

        private string appendLinks(object links)
        {
            string links_kdl = "";
            if(links != null){
                foreach (var link in (List<Link>)links)
                {
                    if (link.Type == "primary")
                        links_kdl += "\tprimary_button {\n";
                    else
                        links_kdl += "\tsecondary_button {\n";
                    links_kdl += "\t\tlabel \"" + link.Text + "\"\n";
                    links_kdl += "\t\turl \"" + link.Url + "\"\n";
                    links_kdl += "\t}\n";
                }
            }
            return links_kdl;
        }

        private static void VerifyAndCorrectKdlFile(string kdlFilePath)
        {
            // read the kdl file content
            string kdlContent = File.ReadAllText(kdlFilePath);
            
            // remove all "### " from the kdl content
            kdlContent = kdlContent.Replace("### ", "");

            // split the kdl content into lines
            string[] lines = kdlContent.Split('\n');

            // create a new list to hold the updated lines
            List<string> updatedLines = new List<string>();

            // loop through each line of the kdl content
            for (int i = 0; i < lines.Length; i++)
            {
                // check if the line is a group header section
                if (lines[i].Contains("group_header_section"))
                {
                    // check if the next 6 sections are left image sections
                    bool isLeftImageSection = true;
                    for (int j = i + 1; j < i + 7; j++)
                    {
                        if (!lines[j].Contains("image_section") || !lines[j].Contains("align \"left\""))
                        {
                            isLeftImageSection = false;
                            break;
                        }
                    }

                    // check if the next 6 sections are paragraph sections
                    bool isParaSection = true;
                    for (int j = i + 1; j < i + 7; j++)
                    {
                        if (!lines[j].Contains("para_section"))
                        {
                            isParaSection = false;
                            break;
                        }
                    }

                    // check if the next 7 to 12 sections are paragraph sections
                    bool isGridSection = false;
                    if (!isLeftImageSection && !isParaSection && i + 12 < lines.Length)
                    {
                        int paraCount = 0;
                        for (int j = i + 1; j < i + 13; j++)
                        {
                            if (lines[j].Contains("para_section"))
                            {
                                paraCount++;
                            }
                        }
                        if (paraCount >= 7 && paraCount <= 12)
                        {
                            isGridSection = true;
                        }
                    }

                    // if any of the conditions match, create a grid section and add it to the updated lines
                    if (isLeftImageSection || isParaSection || isGridSection)
                    {
                        updatedLines.Add("grid_section {");
                        updatedLines.Add("  rows {");
                        for (int j = i + 1; j < i + 7; j++)
                        {
                            updatedLines.Add("    " + lines[j].TrimEnd());
                        }
                        updatedLines.Add("  }");
                        updatedLines.Add("}");
                        i += 6;
                        continue;
                    }
                }

                // if the line is not a group header section or doesn't match any of the conditions,
                // add it to the updated lines as is
                updatedLines.Add(lines[i]);
            }

            // join the updated lines back into a string
            string updatedKdlContent = string.Join("\n", updatedLines);

            // overwrite the kdl file with the updated content
            File.WriteAllText(kdlFilePath, updatedKdlContent);
        }

    }
}


