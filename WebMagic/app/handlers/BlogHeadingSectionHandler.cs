using System.Dynamic;
using Cottle;
using KdlDotNet;
using SimpleJinja2DotNet;

namespace WebMagic
{
    internal class BlogHeadingSectionHandler : SectionHandler
    {
        protected override string GetTemplateName()
        {
            return "BlogHeadingSectionHandler";
        }
    }
}