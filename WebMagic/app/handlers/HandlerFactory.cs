namespace WebMagic
{
    internal class HandlerFactory
    {
        internal static ISectionHandler GetHandler(string handlerClassName)
        {
            switch (handlerClassName)
            {
                case "Import":
                    return new ImportSectionHandler();
                case "CTASection":
                    return new CTASectionHandler();
                case "DefaultNavigationSection":
                    return new DefaultNavigationSectionHandler(); 
                case "HeroSection":
                    return new HeroSectionHandler();
                case "ArtifactHeroSection":
                    return new ArtifactHeroSectionHandler();
                case "ClientLogosSection":
                    return new ClientLogosSection(); 
                case "GroupHeader":
                    return new GroupHeaderHandler();
                case "HeroWithBGSection":
                    return new HeroWithBGSectionHandler();
                case "ParaSection":
                    return new ParaSectionHandler();
                case "BlogHeadingSection":
                    return new BlogHeadingSectionHandler();
                case "ImageSection":
                    return new ImageSectionHandler();
                case "ImpactSection":
                    return new ImpactSectionHandler();
                case "ScheduleDemoSection":
                    return new ScheduleDemoSectionHandler(); 
                case "TestimonialsSection":
                    return new TestimonialsSectionHandler(); 
                case "DefaultFooterSection":
                	return new DefaultFooterSectionHandler(); 
                case "AutoAlignImageSections":
                	return new AutoAlignImageSectionsHandler();
                case "CTABoxRightImageFullPurpleBackSection":
                    return new CTABoxRightImageFullPurpleBackSectionHandler();
                case "CTANoBoxFullLightPurpleBackSection":
                    return new CTANoBoxFullLightPurpleBackSectionHandler();
                case "GridSection":
                    return new GridSectionHandler();
                case "BulletsSection":
                    return new BulletsSectionHandler();
                case "FAQSection":
                    return new FAQSectionHandler();
                case "FeaturesSection":
                    return new FeaturesSectionHandler();
                case "PersonaSection":
                    return new PersonaSectionHandler();
                case "HowToUseArtifactSection":
                    return new HowToUseArtifactSectionHandler();
                case "ArtifactPreviewSection":
                    return new ArtifactPreviewSectionHandler();
                case "AppSummarySection":
                    return new AppSummarySectionHandler();
                case "RatingSection":
                    return new RatingSectionHandler();
                case "RelatedResourcesSection":
                    return new RelatedResourcesSectionHandler();
                case "IndustriesSection":
                    return new IndustriesSectionHandler();
                case "IntegrationsSection":
                    return new IntegrationsSectionHandler();
                case "InfinitePosibilitiesSection":
                    return new InfinitePosibilitiesSectionHandler();
                case "AppsForIndustrySection":
                    return new PersonaSectionHandler();
                case "GetQuoteSection":
                    return new GetQuoteSectionHandler();
                case "SubHeadingSection":
                    return new SubHeadingSectionHandler();
                case "NavigationSection":
                    return new NavigationSectionHandler();
                case "PageBeginSection":
                    return new PageBeginSectionHandler();
                case "PageEndSection":
                    return new PageEndSectionHandler();
                case "WatchVideoSection":
                    return new WatchVideoSectionHandler();

                default:
                    throw new KDLParserException($"Unknown section type: {handlerClassName}");
            }
        }
    }
}