using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public interface IJobProcessorMatchingService
    {
        public Type GetMatchingJobProcessor(string jobName, WorkflowRunSettings settings);
    }

    public class JobProcessorMatchingService : IJobProcessorMatchingService
    {
        private readonly IServiceProvider _serviceProvider;

        public JobProcessorMatchingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Type GetMatchingJobProcessor(string jobName, WorkflowRunSettings settings)
        {
            // for each defined matching rule, check against this job
            var matchedProcessor = settings.JobProcessingSettings?.FirstOrDefault(x => IsMatch(jobName, x.MatchingType, x.MatchString));

            if (string.IsNullOrEmpty(matchedProcessor?.ProcessorName)) return null;

            return Type.GetType($"{GetType().Namespace}.{matchedProcessor.ProcessorName}");
        }

        private bool IsMatch(string jobName, JobProcessingMatchingType matchingType, string matchingString)
        {
            var matchProvider = GetMatchProvider(matchingType);

            if (matchProvider == null) return false;

            return matchProvider.IsMatch(jobName, matchingString);
        }

        private IMatchProvider GetMatchProvider(JobProcessingMatchingType matchingType)
        {
            switch (matchingType)
            {
                case JobProcessingMatchingType.Regex:
                    return _serviceProvider.GetService<RegExMatchProvider>();
                case JobProcessingMatchingType.Exact:
                    return _serviceProvider.GetService<ExactMatchProvider>();
                default:
                    return null;
            }
        }
    }

    public class RegExMatchProvider : IMatchProvider
    {
        public bool IsMatch(string jobName, string matchingString)
        {
            var regEx = new Regex(matchingString, RegexOptions.IgnoreCase);
            var matches = regEx.Matches(jobName);
            
            return matches.Any();
        }
    }

    public class ExactMatchProvider : IMatchProvider
    {
        public bool IsMatch(string jobName, string matchingString)
        {
            return jobName.Equals(matchingString, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public interface IMatchProvider
    {
        public bool IsMatch(string jobName, string matchingString);
    }
}
