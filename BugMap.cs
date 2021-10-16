using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCsvParser
{
    public sealed class BugMap : ClassMap<BugRow>
    {
        public BugMap()
        {
            Map(m => m.Key).Name("Key");
            Map(m => m.Summary).Name("Summary");

            Map(m => m.Status).Name("Status");

            Map(m => m.Assignee).Name("Assignee");

            Map(m => m.Labels).Name("Labels");

            Map(m => m.FixVersions).Name("Fix Version/s");

            Map(m => m.Reporter).Name("Reporter");

            Map(m => m.IssueType).Name("Issue Type");

            Map(m => m.OriginalEstimate).Name("? Original Estimate");

            Map(m => m.Priority).Name("Priority");

            Map(m => m.Sprint).Name("Sprint");

            Map(m => m.DueDate).Name("Due Date");

            Map(m => m.Created).Name("Created");

            Map(m => m.QADueDate).Name("QA Due Date");
        }
    }
}
