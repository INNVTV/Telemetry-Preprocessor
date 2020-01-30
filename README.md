# Telemetry Preprocessor
Preprocessor/ETL component for raw telemetry data on Azure Table Storage and Azure Data Lake. Implements a "Data Factory" role to clean and prepare data for application use as well as for downstream data engineering and machine learning tasks.

# Preprocessor Workflow
![AuthenticationFlow](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/processing-workflow.png)

# Temporal State
![TemporalState](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/temporal-state.png)
The preprocessor determines the next temporal state to process from the available raw telemetry data. Once cleaned and prepared for processing it determines next temporal state to process until it is caught up to the current buffer before sleeping. In this sample code each table stores an hour of data partitioned by the minute. The main preprocessor runs batches of telemetry data by the minute until it reaches the buffer limit. The buffer limit is set to avoid processing a minute block that still has telemetry data coming in.

# Deployment Options
![DeploymentOptions](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/deployment-options.png)

Each preprocessor can be deployed as a serverless function app, as a container within a kubernetes cluster, within a service fabric cluster or any number of similar deplyment architectures. You can also choose to run these on premises as no incoming traffic needs to be handled.

# Consider running data preprocessing tasks on premesis
Since you will not need to handle external traffic consider handling your data preprocessing and machine learning tasks on premises. At the very least stack and pack a VM on Azure. This will give you greater control and you will avoid additional costs and upkeep required with managed services such as serverless functions or cluster managers.

* Process locally. 
* Send results to cloud storage, data lakes & databases. 

### Task Scheduler vs Windows Service
Building as a console application rather than a worker service and simply setting up a scheduled task using Task Scheduler to run the EXE on startup is a good bare bones solution. Windows Services comes with many additional requirements for development and debugging. They can also be challenging to manage when running important tasks in production.

John Galloway goes over many of these details in this post: https://weblogs.asp.net/jongalloway//428303

**Here are a few key takeaways**

* Windows Services are geared towards hard failures of the executable rather than business process failures.
* Windows Services offer no scheduling. Windows Scheduled Tasks have advanced scheduling features which can be accessed administratively.
* Attempting to manage your own timers in Windows Services will prove daunting. The task schedule has been tested across leap years, daylight savings time, and clock changes; most custom timer routines I've seen haven't.
* Scheduled console applications are easier to design, build, test, deploy, and install
* Scheduled console applications are easier to run in an ad hoc manner when necessary.
* Running separate Windows Services with their own timers is just plain inefficient with server resources.
* If I kept writing Windows Services with timers, eventually I'd start to think about writing a single host system with a timer. Then I'd want to add in some more advanced scheduling features; perhaps even some logging. Eventually, I'd have a Windows Service that handles scheduling of child processes, and if I devoted years to enhancements and testing, I'd eventually arrive at... the Windows Task Scheduler.

Since this project is written as a Worker Service you will need to migrate to a Console Application to take advantage of Scheduled Tasks.

# Managed services lock-in
This is really a replacement for the data factory pattern that Azure Data Factory and similar services provide. Again these services are costly both in budget and time. They will often present limitations when you are too far down the road to back track and will often require future updates and migrations to stay up to date. **I cannot stress the importance of staying out of this cloud computing trap!** You should only consider them for high performance, high tim epreference workloads such as Real-Time Stream Analytics.

![ManagedServices](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/managed-services.png)

# Consider running machine learning tasks on premises
For the same reasons stated above this is a no-brainer. Even avoiding services such as DataBricks/Spark has advantages as you can more easily manage your ML pipleline in code, back them up and version them in repositories, manage seperate enviornments and decentralize development tasks. 

* Process locally. 
* Send results to cloud storage, data lakes & databases. 

# Workers And Queues
![WorkersAndQueues](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/workers-queues.png)

A queing system is implemented to ensure that any issues that arise within a particular task can be isolated and it's messages can be picked up and processed later. In production scenarios health checks should be implemented and alert messages should be fired to relevant engineers.

The MainWorker is responsible for parsing out blocks of temporal state from our source telemetry data. It then cleans the data and prepares it for our workers. Once all messages have been sent the main worker logs the processing tasks and determines if a new temporal state is available from the source.

#### Exponential Back-Off
All workers (including main) impement an exponential back-off pattern in order to minimize/maximize polling during busy or quiet periods. Task workers implement a simple version of this while Main will also include a factor of what the current UTC time is in order to catch up with new telemetry data based on temporal states.

# Debugging Multiple Projects
![DebugMultipleProjects](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/debug-multiple-projects.png)
Since each preprocessor is composed of multiple workers in a singe solution be sure to have multiple startup projects selected as shown above.

# Event Sourcing & Temporal Queries
Having the raw telemetry data stored as blocks of time will allow for new preprocessors to come online and gather past data. You also have the option of replaying actions as well as running temporal queries.

![EventSourcing](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/event-sourcing.png)

# Storage Accounts
![StorageAccounts](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/storage-accounts.png)
The example scenario spans accross 4 seperate storage accounts.
* **Telemetry Storage** Stores raw telemetry data in temporal formats
* **Preprocessor Storage** Used to log preprocessor states and pass messages via storage queues
* **Application Storage** Stores data in a format available to the main application without additionl processing requirements
* **Data Lake Storage** Stores cleaned and merged data ready for data analysis and data science.

# Message Queues
The example scenario uses basic storage queues with polling using an exponential back-off pattern. This can be easily updated to use Service Bus for long polling or serverless triggers and webhooks within Azure Functions.

# Production Pipelines
Scalable production scenarios should break telemetry up based on logical entities in order to better manage, isolate and update telemetry scenarios. Each temporal log can then have many preprocessors each focused on one dimension per entity. For example sentiment on content or recommendations for users based on interaction data.

![ProductionPipelines](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/production-pipelines.png)




