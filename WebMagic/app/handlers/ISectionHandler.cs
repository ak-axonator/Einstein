using KdlDotNet;

namespace WebMagic
{
    internal interface ISectionHandler
    {
        string RenderSection(KDLNode node);
    }
}