using System.Dynamic;
using Cottle;
using KdlDotNet;
using SimpleJinja2DotNet;

namespace WebMagic
{
    internal class ArtifactHeroSectionHandler : SectionHandler
    {
        protected override string GetTemplateName()
        {
            return "ArtifactHeroSectionHandler";
        }
    }
}