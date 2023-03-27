using KdlDotNet;

namespace WebMagic
{
    internal class KDLFile
    {
        KDLParser parser = new KDLParser();
        KDLDocument? doc = null;

        public KDLFile(string kdlFileName)
        {
            this.KdlFileName = kdlFileName;
            try
            {
                StreamReader streamReader = new StreamReader(kdlFileName);
                string kdlString = streamReader.ReadToEnd();
                streamReader.Close();
                this.doc = parser.Parse(kdlString);
            }
            catch (KDLParserException e)
            {
                throw new Exception($"Error parsing {kdlFileName}: {e.Message}");
            }
        }

        public string KdlFileName { get; private set; }
        public KDLDocument Doc { get => doc; }
    }
}