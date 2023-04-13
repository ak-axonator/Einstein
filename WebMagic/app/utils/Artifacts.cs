using System;
using System.IO;
using System.Linq;

namespace WebMagic
{
    public class AppDetails
    {
        public string Name { get; set; }
        public string Industry { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<string> Keywords { get; set; }
        public List<ProductFeature> ProductFeatures { get; set; }
        public List<string> Document_Names { get; set; }
        public List<AppDocument> Documents { get; set; }
        public List<string> Form_Names { get; set; }
        public List<Form> Forms { get; set; }
        public List<string> Checklist_Names { get; set; }
        public List<Checklist> Checklists { get; set; }
        public List<string> Workflow_Names { get; set; }
        public List<Workflow> Workflows { get; set; }
        public List<string> Report_Names { get; set; }
        public List<Report> Reports { get; set; }
        public List<AuditChecklist> Audits { get; set; }
        public List<string> Standards_Names { get; set; }
        public List<Standard> Standards { get; set; }
        public List<string> Guidelines_Names { get; set; }
        public List<Guideline> Guidelines { get; set; }
        public List<string> Dashboard_Names { get; set; }
        public List<Dashboard> Dashboards { get; set; }
        public List<string> AuditChecklist_Names { get; set; }
        public List<AuditChecklist> AuditChecklists { get; set; }
    }

    public class ProductFeature
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Benefits { get; set; }
        public List<string> Features { get; set; }
    }

    public class AppDocument
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; } //markdown
    }

    public class Form
    {
        public string FormTitle { get; set; }
        public string FormDescription { get; set; }
        public List<string> Users { get; set; }
        public string BenefitsTitle { get; set; }
        public string BenefitsDescription { get; set; }
        public List<string> Benefits { get; set; }
        public string FeaturesTitle { get; set; }
        public string FeaturesDescription { get; set; }
        public List<string> Features { get; set; }
        public string Use { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<FormField> FormFields { get; set; }
    }

    public class FormField
    {
        public string FieldType { get; set; }
        public string Description { get; set; }
        public List<string> Options { get; set; }
        public string Validation { get; set; }
    }

    public class Checklist
    {
        public string ChecklistTitle { get; set; }
        public string ChecklistDescription { get; set; }
        public List<string> Users { get; set; }
        public string BenefitsTitle { get; set; }
        public string BenefitsDescription { get; set; }
        public List<string> Benefits { get; set; }
        public string FeaturesTitle { get; set; }
        public string FeaturesDescription { get; set; }
        public List<string> Features { get; set; }
        public string Use { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<Checkpoint> Checkpoints { get; set; }
    }

    public class Workflow
    {
        public string WorkflowTitle { get; set; }
        public string WorkflowDescription { get; set; }
        public List<string> Users { get; set; }
        public string BenefitsTitle { get; set; }
        public string BenefitsDescription { get; set; }
        public List<string> Benefits { get; set; }
        public string FeaturesTitle { get; set; }
        public string FeaturesDescription { get; set; }
        public List<string> Features { get; set; }
        public string Use { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<Step> Steps { get; set; }
    }

    public class AuditChecklist
    {
        public string AuditChecklistTitle { get; set; }
        public string AuditChecklistDescription { get; set; }
        public List<string> Users { get; set; }
        public string Benefits { get; set; }
        public string BenefitsTitle { get; set; }
        public string BenefitsDescription { get; set; }
        public List<Feature> Features { get; set; }
        public string Use { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<Checkpoint> Checkpoints { get; set; }
    }
    public class Feature
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
    public class FAQ
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public class Step
    {
        public string StepTitle { get; set; }
        public string StepDescription { get; set; }
        public string StepType { get; set; }
    }

    public class Checkpoint
    {
        public string Title { get; set; }
        public string Hint { get; set; }
    }

    public class Report
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; } //markdown
    }
    public class Guideline
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; } //markdown
    }
    public class Standard
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; } //markdown
    }
    public class Dashboard
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; } //markdown
    }

}