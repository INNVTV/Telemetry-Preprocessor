## Telemetry Preprocessor
Preprocessor component for raw telemetry data on Azure table storage.

# Preprocessor Workflow
![AuthenticationFlow](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/processing-workflow.png)

## Deployment Options
![DeploymentOptions](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/deployment-options.png)

Each preprocessor can be deployed as a serverless function app, as a container within a kubernetes cluster, within a service fabric cluster or any number of similar deplyment architectures. You can also choose to run these on premises as no incoming traffic needs to be handled.

# Consider running data preprocessing tasks on premesis
Since you will not need to handle external traffic consider handling your data preprocessing and machine learning tasks on premises. At the very least stack and pack a VM on Azure. This will give you greater control and you will avoid additional costs and upkeep required with managed services such as serverless functions or cluster managers.

## Process locally. 
## Send results to cloud storage, data lakes & databases. 

# Managed services lock-in
This is really a replacement for the data factory pattern that Azure Data Factory and similar services provide. Again these services are costly both in budget and time. They will often present limitations when you are too far down the road to back track and will often require future updates and migrations to stay up to date. **I cannot stress the importance of staying out of this cloud computing trap!** You should only consider them for high performance, high tim epreference workloads such as Real-Time Stream Analytics.

![ManagedServices](https://github.com/INNVTV/Telemetry-Preprocessor/blob/master/_docs/images/managed-services.png)

# Consider running machine learning tasks on premises
For the same reasons stated above this is a no-brainer. Even avoiding services such as DataBricks/Spark has advantages as you can more easily manage your ML pipleline in code, back them up and version them in repositories, manage seperate enviornments and decentralize development tasks. 


