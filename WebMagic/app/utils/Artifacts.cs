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
        public string Icon { get; set; }
        public List<string> Keywords { get; set; }
        public List<Benefit> Product_Benefits { get; set; }
        public List<Feature> Product_Features { get; set; }
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
        public List<string> Integration_Names { get; set; }
        public List<Integration> Integrations { get; set; }
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
        public string Form_Title { get; set; }
        public string Form_Description { get; set; }
        public List<string> Users { get; set; }
        public List<Benefit> Benefits { get; set; }
        public List<Feature> Features { get; set; }
        public List<string> Use { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<FormField> Form_Fields { get; set; }
    }

    public class FormField
    {
        public string Form_Field { get; set; }
        public string Field_Type { get; set; }
        public string Description { get; set; }
        public List<string> Options { get; set; }
        public string Validation { get; set; }
        public string Hint { get; set; }
    }

    public class Checklist
    {
        public string Checklist_Title { get; set; }
        public string Checklist_Description { get; set; }
        public List<string> Users { get; set; }
        public List<Benefit> Benefits { get; set; }
        public List<Feature> Features { get; set; }
        public List<string> Use { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<ChecklistPoint> Checkpoints { get; set; }
    }

    public class Workflow
    {
        public string Workflow_Title { get; set; }
        public string Workflow_Description { get; set; }
        public List<string> Users { get; set; }
        public List<Benefit> Benefits { get; set; }
        public List<Feature> Features { get; set; }
        public string Use { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<Step> Steps { get; set; }
    }

    public class AuditChecklist
    {
        public string Audit_Checklist_Title { get; set; }
        public string Audit_Checklist_Description { get; set; }
        public List<string> Users { get; set; }
        public List<Benefit> Benefits { get; set; }
        public List<Feature> Features { get; set; }
        public List<string> Use { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<ChecklistPoint> Checkpoints { get; set; }
    }
    public class Feature
    {
        public string Features_Title { get; set; }
        public string Features_Description { get; set; }
    }
    public class FAQ
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public class Step
    {
        public string Step_Title { get; set; }
        public string Step_Description { get; set; }
        public string Step_Type { get; set; }
    }

    public class ChecklistPoint
    {
        public string Checkpoint { get; set; }
        public List<string> Options { get; set; }
        public string Hint { get; set; }
        public string Validation { get; set; }
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
    public class Integration
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
    public class Artifact
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}