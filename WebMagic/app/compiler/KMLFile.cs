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
            catch (Exception e)
            {
                CommandProcessor.LogJsonParsingError(e, e.Message, kdlFileName);
            }
        }

        public string KdlFileName { get; private set; }
        public KDLDocument Doc { get => doc; }
    }
}