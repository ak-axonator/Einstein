using KdlDotNet;

namespace WebMagic
{
    internal class AutoAlignImageSectionsHandler : SectionHandler
    {
        // string ISectionHandler.RenderSection(KDLNode node)
        // {
        //     return "AutoAlignImageSectionsHandler";
        // }
        protected override string GetTemplateName()
        {
            return "AutoAlignImageSectionsHandler";
        }
    }
}