using System.Text.RegularExpressions;
using System;

public class LineParser
{
    public static void ParseLines(string text)
    {
        // Define the regular expression pattern
        string pattern = @"^(\d+[\.\)]\s*)?(?<heading>.+?)\s*[-:]\s*(?<description>.+)$";

        // Split the input text into separate lines
        string[] lines = text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // Loop through each line and check if it matches the pattern
        foreach (string line in lines)
        {
            Match match = Regex.Match(line, pattern);
            if (match.Success)
            {
                // Extract the heading and description from the captured groups
                string heading = match.Groups["heading"].Value;
                string description = match.Groups["description"].Value;

                // Print the results to the console
                Console.WriteLine($"Heading: {heading}");
                Console.WriteLine($"Description: {description}");
            }
        }
    }
}

public class GPTResponseValidator {
    private string pattern;

    public GPTResponseValidator() {
        // Define the regular expression pattern for a single bullet point item
        pattern = @"^(\d+\. .+:\s.+(\n\d+\. .+:\s.+)*)*";
    }

    public bool ValidateBulletResponse(string response) {
        LineParser.ParseLines(response);
        return true;
        // Test if the response is in the expected format
    //     Regex regex = new Regex(@"^(?:[\.\)]\d+[-:])\s*(.+)$", RegexOptions.Multiline);
    //     MatchCollection matches = regex.Matches(response);

    //     foreach (Match match in matches)
    //     {
    //         string text = match.Groups[1].Value;
    //         Console.WriteLine(text);
    //     }
    //     return Regex.IsMatch(response, pattern);
     }
}
