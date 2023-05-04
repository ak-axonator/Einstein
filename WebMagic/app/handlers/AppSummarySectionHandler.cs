using System.Dynamic;
using Cottle;
using KdlDotNet;
using SimpleJinja2DotNet;

namespace WebMagic
{
    internal class AppSummarySectionHandler : SectionHandler
    {
        protected override string GetTemplateName()
        {
            return "AppSummarySectionHandler";
        }
    }
}