using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector.Models
{
    public class WorkflowRunModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime DateStartedUtc { get; set; }
        public DateTime DateCompletedUtc { get; set; }
        public int NumAttempts { get; set; } // will this end up being a computed value instead?
        public string Conclusion { get; set; }
    }
}
