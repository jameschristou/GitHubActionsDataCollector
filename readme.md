# GitHubActions Data Collector
This .NET console application can process data related to GitHubActions workflows.
This project is a work in progress with multiple phases. We are currently in phase 1.

## Phase 1 - DONE
* No built in GHA API calls at this stage. Instead we manually call the API, save the results to a folder and then the console application will process these files.
* Data will be written to the console and not saved anywhere.
* Objective is processing the data returned by the API

## Phase 2 - DONE
* We write data related to each workflow run to a relational DB. Still no GHA API calls at this stage.
* Save some basic queries to get useful reporting out of the DB

## Phase 3 - DONE
* We start calling the GHA API in order to get this data

## Phase 4 - DONE
For each workflow run, we get details on each job and store this data

## Phase 5 - DONE
Drive the workflows to process from a SQL table and enable the process to restart from where it last got to
* Loop through the workflow runs and implement paging (DONE)
* Have a registration of workflows that we process rather than having this hardcoded (DONE)
* Link the WorkflowRun and WorkflowRunJob tables through the Id column on WorkflowRun (DONE)
* When starting the processing, pick up where we left off by checking the last workflow run processed - we'll need to link WorkflowRun and RegsiteredWorkflow for this (DONE)
* Ignore skipped jobs (DONE)
* Ignore runs where all jobs are either skipped or cancelled (DONE)
* Some runs which are not complete will time out and the updated date for the run is updated at this point. It would be better to use the run start time and the completed time of the last non skipped job to determine the completion time of the run rather than updated date. (DONE)
* Add audit date columns onto the WorkflowRun table to help track of when processing occurred (DONE)

## Phase 6
Introduce job groups for grouping related jobs. Job groups should also have duration and conclusion info
* Introduce job groups
* Each run has job groups and each job belongs to a job group (is this true in simple workflows??)

## Phase 7
We start interrogating the workflow run logs to get details on test results. We make this flexible so people can write their own custom log processor
* Add XUnit tests and run the unit tests in the workflow for GitHubActionsDataCollector
* Add the ability to record individual test results (status, duration, error messages, etc)
* Extract and record test results for XUnit tests
* Extract and record test results for Cypress tests
* 

## Phase 8
Look at defending against exceeding Github API limits
* Because of (GHA API usage limits)[https://docs.github.com/en/actions/administering-github-actions/usage-limits-billing-and-administration] we want to make sure we limit how many calls are made in an hour. The X-RateLimit-Remaining response header could be useful.

## Phase 9
Set this up to run as a container in AWS Lambda on a regular schedule