# GitHubActions Data Collector
This .NET console application can process data related to GitHubActions workflows.
This project is a work in progress with multiple phases. We are currently in phase 1.

## Phase 1
* No built in GHA API calls at this stage. Instead we manually call the API, save the results to a folder and then the console application will process these files.
* Data will be written to the console and not saved anywhere.
* Objective is processing the data returned by the API

## Phase 2
* We write data related to each workflow run to a relational DB. Still no GHA API calls at this stage.
* Save some basic queries to get useful reporting out of the DB

## Phase 3
* We start calling the GHA API in order to get this data

## Phase 4
* For each workflow run, we get details on each job and store this data

## Phase 5
* We start interrogating the workflow run logs to get details on test results. We make this flexible so people can write their own custom log processor. Built in support for processing logs form XUnit.

## Phase 6
* Set this up to run as a lambda in AWS on a regular schedule