using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector.GitHubActionsApi
{
    public class WorkflowRunArtifactsDto
    {
        public int total_count { get; set; }
        public List<WorkflowRunArtifactDto> artifacts { get; set; }
    }

    public class WorkflowRunArtifactDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public string archive_download_url { get; set; }
    }
}
