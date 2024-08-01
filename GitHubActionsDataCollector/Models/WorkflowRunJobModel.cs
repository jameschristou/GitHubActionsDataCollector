﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector.Models
{
    public class WorkflowRunJobModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Conclusion { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime CompletedAtUtc { get; set; }
        public string Url { get; set; }
        public int RunAttempt { get; set; }
    }
}
