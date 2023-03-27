using KdlDotNet;

namespace WebMagic
{
    internal class ImportSectionHandler : ISectionHandler
    {
        string ISectionHandler.RenderSection(KDLNode node)
        {
           if (node.Args.Count == 0)
               throw new Exception("Import section requires a path argument");
              
              var path = node.Args[0].AsString().Value;
              //return "<Import path=\"" + path + "\"></Import> <!-- Todo-->";
              return "";
        }
    }
}