# Telemetry Preprocessor
Preprocessor/ETL component for raw telemetry data on Azure Table Storage and Azure Data Lake. Implements a "Data Factory" role to clean and prepare data for application use as well as for downstream data engineering and machine learning tasks.

# Preprocessor Workflow
![AuthenticationFlow](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/processing-workflow.png)
The preprocessor determines last and next temporal state to process from the available raw telemetry data. Once cleaned and prepared for processing it determines next temporal state to process until it is caught up to the current buffer before sleeping.

## Deployment Options
![DeploymentOptions](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/deployment-options.png)

Each preprocessor can be deployed as a serverless function app, as a container within a kubernetes cluster, within a service fabric cluster or any number of similar deplyment architectures. You can also choose to run these on premises as no incoming traffic needs to be handled.

# Consider running data preprocessing tasks on premesis
Since you will not need to handle external traffic consider handling your data preprocessing and machine learning tasks on premises. At the very least stack and pack a VM on Azure. This will give you greater control and you will avoid additional costs and upkeep required with managed services such as serverless functions or cluster managers.

* Process locally. 
* Send results to cloud storage, data lakes & databases. 

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



