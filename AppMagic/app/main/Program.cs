using System.Text.RegularExpressions;
using System.Xml;

namespace AppMagic
{
    public class Program
    {
        static void Main(string[] args)
        {
            importAXP();

        }

        public static void importAXP(){
            string axpPath = "/Users/arohikulkarni/Work/Einstein/SystemFiles/AXPs/Safety Audit_14_03_23_14_31_16_739928 (only form).axp";
            // string appName = "Housekeeping HSE Audit";
            
            importAXPinAxonator(axpPath);
            // CreatePageFileFromOutputKDL();
        }

        private static void importAXPinAxonator(string axpPath)
        {
            Console.WriteLine("AXP imported..");
        }

    }
}