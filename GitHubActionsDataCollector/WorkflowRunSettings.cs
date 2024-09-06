using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector
{
    public class WorkflowRunSettings
    {
        public string Token { get; set; }

        // if this job name succeeds then that means the workflow run has succeeded regardless of the run conclusion
        // leave this blank to just rely on the run conclusion
        public string JobNameRequiredForRunSuccess { get; set; }

        public List<JobProcessing> JobProcessingSettings { get; set; }
    }

    public class JobProcessing
    {
        public string JobName { get; set; }
        public string JobNameRegex { get; set; }
        public string ProcessorName { get; set; }
    }
}
