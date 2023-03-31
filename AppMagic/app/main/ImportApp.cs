using System.Text.RegularExpressions;
using System.Xml;
// using RestSharp;

namespace AppMagic
{
    public class ImportApp
    {
        public static void importAXP(){
            string axpPath = "/Users/arohikulkarni/Work/Einstein/SystemFiles/AXPs/Safety Audit_14_03_23_14_31_16_739928 (only form).axp";
            // string appName = "Housekeeping HSE Audit";
            
            importAXPinAxonator(axpPath);
            // CreatePageFileFromOutputKDL();
        }

        private static void importAXPinAxonator(string axpPath)
        {
            // var client = new RestClient("https://app.axonator.com/importnewApp/");
            // client.Timeout = -1;
            // var request = new RestRequest(Method.POST);
            // request.AddHeader("X-CSRFTOKEN", "MKXpKLBzu7gU1bKpRaSiUHaYcfwcMkzR");
            // request.AddHeader("Cookie", "csrftoken=MKXpKLBzu7gU1bKpRaSiUHaYcfwcMkzR; ishoutToken=17863|585f0e083665441cb9f0d2d7df0756f0; sessionid=ihqtfbtyxm9fcy5xj61psmm0u99bccwt");
            // request.AddFile("file", axpPath);
            // IRestResponse response = client.Execute(request);
            // Console.WriteLine(response.Content);
            // Console.WriteLine("AXP imported..");
        }

    }
}