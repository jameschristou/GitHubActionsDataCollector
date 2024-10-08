# GitHubActions Data Collector
This .NET console application can process data related to GitHubActions workflows.
This project is a work in progress with multiple phases. See Phases.

## Using the GitHubActions Data Collector
### Creating the DB
First you will need a SQL Server database. Then you can run the GitHubActionsDataCollector with the following code in [Program.cs](GitHubActionsDataCollector/Program.cs) uncommented
`.ExposeConfiguration(BuildSchema`. This will create the required schema in the database. Currently the database connection string is just hard coded in [Program.cs](GitHubActionsDataCollector/Program.cs).

### Creating an Entry in the RegisteredWorkflow Table
Each workflow you wish to collect data on needs a record in the `RegisteredWorkflow` table. This is how the GitHubActionsDataCollector knows which workflows to process.
If the workflow is for a private repository, you will need to provide a [GitHub personal access token](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-personal-access-token-classic) in the Settings column. See the SQL script [insert workflow.sql](GitHubActionsDataCollector/Repositories/Sql%20Scripts/HelperScripts/insert%20workflow.sql) for an example of how to do this.

| Column Name | Example |
|-------------|---------|
| Owner | `jameschristou` in https://github.com/jameschristou/GitHubActionsDataCollector/actions/workflows/build.yml |
| Repo | `GitHubActionsDataCollector` in https://github.com/jameschristou/GitHubActionsDataCollector/actions/workflows/build.yml |
| WorkflowId | Numerical id for the workflow. This can be obtained using https://docs.github.com/en/rest/actions/workflows?apiVersion=2022-11-28#get-a-workflow. With this endpoint use build.yml as the workflow_id and it will return data about the workflow, including the numeric workflowid. |
| WorkflowName | The name of the workflow, can also be obtained using the above endpoint |
| LastCheckedAtUtc | This datetime value is used to keep track of when the workflow was last checked for new runs. Initially set this to a date in the past |
| ProcessedUntilUtc | Keeps track of the datetime that we have processed workflow runs until. The processor will look for new workflow runs that were started after this datetime |
| IsActive | If set to 1, this workflow will be processed, otherwise it will be ignored.|
| Settings | This is json configuration data. See the below section for details |

#### Settings Json
The settings column contains a json configuration string.

```json
{
    "token": "your_token",
    "jobNameRequiredForRunSuccess": "build",
    "jobProcessingSettings":[
        {
            "matchingType": "Regex",
            "matchString": "Test",
            "processorName": "DotNetXmlTestResultsProcessor"
        }
    ]
}
```

If the repository you are trying to process workflows on is private then you must provide a [GitHub personal access token](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-personal-access-token-classic) in the token property.

## Phases
### Phase 1 - DONE
* No built in GHA API calls at this stage. Instead we manually call the API, save the results to a folder and then the console application will process these files.
* Data will be written to the console and not saved anywhere.
* Objective is processing the data returned by the API

### Phase 2 - DONE
* We write data related to each workflow run to a relational DB. Still no GHA API calls at this stage.
* Save some basic queries to get useful reporting out of the DB

### Phase 3 - DONE
* We start calling the GHA API in order to get this data

### Phase 4 - DONE
For each workflow run, we get details on each job and store this data

### Phase 5 - DONE
Drive the workflows to process from a SQL table and enable the process to restart from where it last got to
* Loop through the workflow runs and implement paging (DONE)
* Have a registration of workflows that we process rather than having this hardcoded (DONE)
* Link the WorkflowRun and WorkflowRunJob tables through the Id column on WorkflowRun (DONE)
* When starting the processing, pick up where we left off by checking the last workflow run processed - we'll need to link WorkflowRun and RegsiteredWorkflow for this (DONE)
* Ignore skipped jobs (DONE)
* Ignore runs where all jobs are either skipped or cancelled (DONE)
* Some runs which are not complete will time out and the updated date for the run is updated at this point. It would be better to use the run start time and the completed time of the last non skipped job to determine the completion time of the run rather than updated date. (DONE)
* Add audit date columns onto the WorkflowRun table to help track of when processing occurred (DONE)

### Phase 6 - DONE
We start interrogating the workflow run logs to get details on test results. We make this flexible so people can write their own custom log processor
* Add XUnit tests (DONE)
* Run the unit tests in the workflow for GitHubActionsDataCollector (DONE)
* Add the ability to record individual test results (status, duration, error messages, etc) (DONE)
* Extract and record test results for XUnit tests (DONE)
* Extract and record test results for Cypress tests (DONE)

### Phase 7 - DONE
Configurability. Make the following configurable so this can work as a solution for more use cases:
* Have config for determining which jobs indicate the workflow has succeeded even if the workflows Conclusion is failed (DONE)
* Move token into config (DONE)
* Have config to determine which steps are tests and which job processor to run on them (DONE)

### Phase 8 - DONE
Look at defending against exceeding Github API limits
* Because of (GHA API usage limits)[https://docs.github.com/en/actions/administering-github-actions/usage-limits-billing-and-administration] we want to make sure we limit how many calls are made in an hour. The X-RateLimit-Remaining response header could be useful. (DONE)

### Phase 9 - Reporting & Dashboards
* Create some basic SQL scripts to get reports (DONE)

### Phase 10
Introduce job groups for grouping related jobs. Job groups should also have duration and conclusion info
* Introduce job groups - grouping could be configured through regex on job name
* Each run has job groups and each job belongs to a job group (is this true in simple workflows??)

### Phase 11
Set this up to run as a container in AWS Lambda on a regular schedule