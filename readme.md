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

## Phase 4 - IN PROGRESS
For each workflow run, we get details on each job and store this data

TODO
* Loop through the workflow runs and implement paging
* When starting the processing, pick up where we left off by checking the last workflow run processed
* Have a registration of workflows that we process and keep track of last checked for each workflow
* Link the WorkflowRun and WorkflowRunJob tables through the Id column on WorkflowRun.
* Add unit tests and run the unit tests in the workflow for GitHubActionsDataCollector
* Add audit date columns
* Write everything for a workflow run in one go. That way if something goes wrong we don't write the records and we pick it up again next time

## Phase 5
* We start interrogating the workflow run logs to get details on test results. We make this flexible so people can write their own custom log processor. Built in support for processing logs form XUnit.

## Phase 6
* Set this up to run as a lambda in AWS on a regular schedule