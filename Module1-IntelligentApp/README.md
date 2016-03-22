<a name="HOLTop"></a>
# Intelligent Application #

---

<a name="Overview"></a>
## Overview ##

In this module you will build an eCommerce web site for an automotive parts supplier called "Parts Unlimited".

One of the challenges for this scenario implementation is how the data will be stored. Every part of the catalog may have its own attributes in addition to the common attributes that all parts share. Furthermore, attributes for a specific part can change the following year when a new model is released. To address this, you will use **DocumentDB**. As a JSON document store, _DocumentDB_ supports flexible schemas (NoSQL store) with no need to add indexes (is fully indexed by default) and allows you to represent data with nested properties. Also, DocumentDB is a _PaaS_ offering on Azure. That means you don't manage a virtual machine yourself, all you need to do is create a DocumentDB instance in the Azure portal and connect with any of the available drivers/SDKs.

For the web site you will also might want to track the user interactions (such as products views, additions to the cart, etc.) for further analysis. The challenge here is to process millions of events from concurrent users connected from different devices across the globe. With **Azure Event Hubs** you can process large amounts of event data from connected devices and applications. Azure Event Hubs is a managed service that ingests events with elastic scale to accommodate to  variable load profiles and the spikes caused by intermittent connectivity.

After you collect data into Event Hubs, you can store the data using a storage cluster or transform it using a real-time analytics provider. **Azure Stream Analytics** is integrated out-of-the-box with Azure Event Hubs to ingest millions of events per second. Stream Analytics processes ingested events in real-time, comparing multiple streams or comparing streams with historical values and models. It detects anomalies, transforms incoming data, triggers an alert when a specific error or condition appears in the stream, and displays this real-time data in your dashboard. For this scenario, you will use **Stream Analytics** to process and spool data to Blob Storage and Power BI.

<a name="Objectives"></a>
### Objectives ###
In this module, you'll:

- Walk through **DocumentDB** integration
- Create an **Event Hub** and integrate it into a Web App
- Use **Stream Analytics** to process data in near-realtime and spool data to **Blob Storage** and **Power BI**
- Create a sample **Power BI** dashboard

<a name="Prerequisites"></a>
### Prerequisites ###

The following is required to complete this module:

- [Visual Studio Community 2015][1] or greater
- [ASP.NET Core 1.0][2]
- [Microsoft Azure Storage Explorer][3] or any other tool to manage Azure Storage
- [Service Bus Explorer][4]

[1]: https://www.visualstudio.com/products/visual-studio-community-vs
[2]: https://docs.asp.net/en/latest/getting-started/installing-on-windows.html#install-asp-net-5-with-visual-studio
[3]: http://storageexplorer.com/
[4]: https://github.com/paolosalvatori/ServiceBusExplorer

<a name="Setup"></a>
### Setup ###
Throughout the module document, you'll be instructed to insert code blocks. For your convenience, most of this code is provided as Visual Studio Code Snippets, which you can access from within Visual Studio 2015 to avoid having to add it manually. To install the code snippets run the setup script:

1. Open Windows Explorer and browse to the module's **Source** folder.
1. Right-click **Setup.cmd** and select **Run as administrator** to launch the setup process that will configure your environment and install the Visual Studio code snippets for this module.
1. If the User Account Control dialog box is shown, confirm the action to proceed.

> **Note**: Each exercise is accompanied by a starting solution located in the **Begin** folder of the exercise, when applicable, that allows you to follow each exercise independently of the others. Please be aware that the code snippets that are added during an exercise are missing from these starting solutions and may not work until you've completed the exercise. Inside the source code for an exercise, you'll also find an **End** folder, when applicable, containing a Visual Studio solution with the code that results from completing the steps in the corresponding exercise. You can use these solutions as guidance if you need additional help as you work through this module.

---

<a name="Exercises"></a>
## Exercises ##
This module includes the following exercises:

1. [Walking through DocumentDB Integration](#Exercise1)
1. [Creating and integrating Event Hubs](#Exercise2)
1. [Using Stream Analytics to process your data](#Exercise3)
1. [Visualizing your data with Power BI](#Exercise4)

Estimated time to complete this module: **60 minutes**

> **Note:** When you first start Visual Studio, you must select one of the predefined settings collections. Each predefined collection is designed to match a particular development style and determines window layouts, editor behavior, IntelliSense code snippets, and dialog box options. The procedures in this module describe the actions necessary to accomplish a given task in Visual Studio when using the **General Development Settings** collection. If you choose a different settings collection for your development environment, there may be differences in the steps that you should take into account.

> ![General Development Settings](Images/vs-general-development.png?raw=true "General Development Settings")

> _General Development Settings_

<a name="Exercise1"></a>
### Exercise 1: Walking through DocumentDB Integration ###

DocumentDB is a massively scalable NoSQL system (JSON documents) that partitions data across multiple nodes, automatically replicating data 3X. The JSON documents are automatically indexed by default. 

In this exercise, you'll create and integrate the website product catalog in DocumentDB.

<a name="Ex1Task1"></a>
#### Task 1 - Creating the DocumentDB database ####

In this task, you'll create a DocumentDB account. If you already have an account you want to use, you can skip ahead to the next task.

1. Sign in to the online [Microsoft Azure portal](https://portal.azure.com/).

1. In the Jumpbar, click **New**, then **Data + Storage**, and **Azure DocumentDB**.

	![Creating a new DocumentDB account](Images/creating-a-new-documentdb-account.png?raw=true "Creating a new DocumentDB account")

	_Creating a new DocumentDB account_

1. In the **New DocumentDB account** blade, specify an **ID**, e.g. datamodule1 and a **Resource Group**; take into account that you'll want to use the same Resource Group for all the Azure services you create in this module. Finally, choose the same **Location** where the Resource Group is created.

1. Once the new DocumentDB account options are configured, click **Create**. It can take a few minutes for the DocumentDB account to be created. To check the status, you can monitor the progress on the main Dashboard or from the Notifications icon located at the top right next to the search bar.

1. Open in Visual Studio the **ProductCatalogSampleData.sln** solution located at **Source / Ex1 / Assets / ProductCatalogSampleData** folder.
  
	_DocumentDB_ exposes a _RESTful_ interface over _HTTPS_ or _TCP_, for an easier development, this project is using the client SDK available through a [NuGet package](https://www.nuget.org/packages/Microsoft.Azure.DocumentDB). The SDK can be configured to automatically handle the mapping from logical to physical location of data (at a performance cost) and it can be used in direct mode for greater performance (at the cost of more complex client code).

1. Go back to the portal and verify the DocumentDB account was created. Navigate to the **Keys** blade of your DocumentDB account; copy the **URI** and **Primary Key**.

	![Copying the DocumentDB account keys](Images/copying-documentdb-keys.png?raw=true "Copying the DocumentDB account keys")

	_Copying the DocumentDB account keys_

1. Replace the **documentDbEndpoint** and  **documentDbAuthKey** values in **Program.cs** with the DocumentDB **URI** and **Primary Key** you copied.

1. Navigate to the **DocumentDBTestData** class. This is a helper class that uses a 'Repository' to populate with sample data that is hardcoded in this class.

1. Navigate to the **ProductsRepository** class. Notice this class exposes read functions (_GetDocumentById, GetById, Find_) using LinQ extensions for DocumentDB (_Microsoft.Azure.Documents.Linq_) and write functions (_CreateAsync, UpdateAsync, CreateOrUpdateAsync, DeleteAsync_), all of them makes use (directly or through private functions) of the **DocumentClient** class (_Client_ property) which provides a client-side logical representation of the Azure DocumentDB service.

	> **Note:** in next task you will see more details about this class which is reused in the Web application.

1. **Build** the solution to trigger the download of required NuGet packages.

1. **Run** the application.

1. In the Azure Portal, go to your DocumentDB settings and click **Document Explorer**.

1. Select _PartsUnlimited_ in the **Databases** list and _Products_ in **Collections**. Then select one of the ID's below in order to see the JSON document.

	![Document Explorer](Images/documentdb-explorer.png?raw=true "Document Explorer")

	_Document Explorer_

<a name="Ex1Task2"></a>
#### Task 2 - Configuring the Parts Unlimited solution ####

The Parts Unlimited solution is an automotive parts scenario where every product may have its own attributes in addition to the common attributes that all products share. Furthermore, attributes for a specific product can change the following year when a new model is released. As a JSON document store, DocumentDB supports flexible schemas and allows you to represent data with nested properties, and thus it is well suited for storing product catalog data.

In this task you'll explore the implementation and configure the DocumentDB settings.

1. Open in Visual Studio the **PartsUnlimited.sln** solution located at **Source / Ex1 / Begin** folder.

1. Open the **ProductsRepository.cs** file located in the **PartsUnlimited.Models** project.

1. The **ReadOrCreateDatabase** method uses the internal instance of the **DocumentClient** to load an existing DocumentDB or create a new one using the [CreateDatabaseAsync](https://msdn.microsoft.com/library/microsoft.azure.documents.client.documentclient.createdatabaseasync.aspx) method. The method uses the "**Client**" property which exposes a single instance of the **DocumentClient** (a best practice). We check for the existence of the database by looking for the **databaseId** passed into the repository constructor. If the database does not exist, we call **CreateDatabaseAsync** to create it.

	````C#
	private Database ReadOrCreateDatabase()
	{
			var db = this.Client.CreateDatabaseQuery()
											.Where(d => d.Id == this.databaseId)
											.AsEnumerable()
											.FirstOrDefault();

			if (db == null)
			{
					db = this.Client.CreateDatabaseAsync(new Database { Id = this.databaseId }).Result;
			}

			return db;
	}
	````

1. Check the **ReadOrCreateCollection** method. A collection is a container of JSON documents and associated JavaScript application logic, such as stored procedures, triggers or user defined functions, and it can be created by using the [CreateDocumentCollectionQuery](https://msdn.microsoft.com/library/microsoft.azure.documents.client.documentclient.createdocumentcollectionquery.aspx) method of the **DocumentClient** class.

	````C#
	private DocumentCollection ReadOrCreateCollection(string databaseLink)
	{
			var col = this.Client.CreateDocumentCollectionQuery(databaseLink)
												.Where(c => c.Id == this.collectionId)
												.AsEnumerable()
												.FirstOrDefault();

			if (col == null)
			{
					col = this.Client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = this.collectionId }).Result;
			}

			return col;
	}
	````

1. You will use the **GetById** and **Find** methods to query DocumentDB using the [CreateDocumentQuery](https://msdn.microsoft.com/en-us/library/microsoft.azure.documents.client.documentclient.createdocumentquery.aspx) method.

	````C#
	public Product GetById(int id)
	{
			return this.Set<Product>()
							.Where(d => d.ProductId == id)
							.AsEnumerable()
							.FirstOrDefault();
	}

	public IEnumerable<Product> Find(Expression<Func<Product, bool>> predicate)
	{
			return this.Set<Product>()
							.Where(predicate)
							.AsEnumerable();
	}

	public IQueryable<T> Set<T>()
	{
			return this.Client.CreateDocumentQuery<T>(this.Collection.SelfLink);
	}
	````

1. A [document](https://azure.microsoft.com/documentation/articles/documentdb-resources/#documents) can be created using the [CreateDocumentAsync](https://msdn.microsoft.com/library/microsoft.azure.documents.client.documentclient.createdocumentasync.aspx)  method.

	````C#
	public async Task CreateAsync(Product product)
	{
			if (product.ProductId == 0)
			{
					product.ProductId = GenerateProductId();
			}

			var response = await this.Client.CreateDocumentAsync(this.Collection.SelfLink, product);
			this.EnsureSuccessStatusCode(response);
	}
	````

1. The **UpdateAsync** method uses the DocumentClient [ReplaceDocumentAsync](https://msdn.microsoft.com/library/microsoft.azure.documents.client.documentclient.replacedocumentasync.aspx) method to update and existing document. Note that the document is required.

	````C#
	public async Task UpdateAsync(Product product)
	{
			Document doc = this.GetDocumentById(product.ProductId);

			if (doc == null)
			{
					throw new Exception(string.Format(CultureInfo.InvariantCulture, "Cannot find a product with id '{0}'", product.ProductId));
			}

			var response = await this.Client.ReplaceDocumentAsync(doc.SelfLink, product);
			this.EnsureSuccessStatusCode(response);
	}
	````

1. The **DeleteAsync** method gets the document and deletes it using the client [DeleteDocumentAsync](https://msdn.microsoft.com/library/microsoft.azure.documents.client.documentclient.deletedocumentasync.aspx) method.

	````C#
	public async Task DeleteAsync(Product product)
	{
			Document doc = this.GetDocumentById(product.ProductId);

			if (doc != null)
			{
					await this.Client.DeleteDocumentAsync(doc.SelfLink);
			}
	}
	````

1. Before running the app, you will configure the DocumentDB settings. Open the **config.json** file located in the **PartsUnlimited** website.

	![config.json file](Images/config-json.png?raw=true "config.json file")

	_config.json file_

1. Update the DocumentDB **Endpoint** and **AuthKey** settings with the **URI** and **Primary Key** values that you got from the previous task.

1. Open the **ProductsRepository** from the **PartsUnlimited.Models** project and set a breakpoint inside the **GetById** function.

1. Press **F5** to start debugging the application and navigate to its local URL.

	>**Note**: If you see any CSS issue, stop the app, right-click on PartsUnlimited Dependencies and select **Restore Packages**. Then, run the grunt task **default** in the 'Task Runner Explorer' window.

1. Click on any product displayed in the home page. The breakpoint should be hit.

1. Press **F10** to step over to the caller function "Details". This is the controller action invoked by the _/details/{id}_ endpoint. As you can see, the action uses an instance of the ProductsRepository class to get the product data from DocumentDB and then display it in the corresponding view (preivously, it tries to retrieve from cache or store it if it's not cached).

	![Debugging product details](Images/debugging-get-product.png?raw=true "Debugging product details")

	_Debugging product details_

1. Stop debugging.

<a name="Exercise2"></a>
### Exercise 2: Creating and integrating Event Hubs ###

Azure Event Hubs is an event processing service that provides event and telemetry ingress to the cloud at massive scale, with low latency and high reliability. This service, used with other downstream services, is particularly useful in application instrumentation, user experience or workflow processing, and Internet of Things (IoT) scenarios.
In this exercise, you will use Azure Event Hubs to track the user behavior in your retail website when viewing a product and also when adding it to the cart.

<a name="Ex2Task1"></a>
#### Task 1 - Creating the Event Hub ####

In this task, you will create the Event Hub that will be used in the following tasks.

1. Log on to the [Azure classic portal](https://manage.windowsazure.com/), and click **NEW** at the bottom of the screen.

1. Click App Services, then **Service Bus**, then **Event Hub**, then **Quick Create**.

1. Type a name for your Event Hub, e.g. eventhubdatamodule, select your desired region, and then click **Create a new Event Hub**.

	![Creating the Event Hub](Images/creating-event-hub.png?raw=true "Creating the Event Hub")

	_Creating the Event Hub_

1. Click the namespace you just created (usually _event hub name_-ns).

1. Click the **Event Hubs** tab at the top of the page, and then click the Event Hub you just created.

1. Click the **Configure** tab at the top, add a new policy named **SendRule** with _Send_ rights, add another policy called **ReceiveRule** with _Manage, Send, Listen_ rights, and then click **Save**.

	![Creating the shared access policies](Images/creating-event-hub-policies.png?raw=true "Creating the shared access policies")

	_Creating the shared access policies_

	We created 2 Shared Access Policies and 2 Shared Access Signatures (or tokens) based off those policies. The signatures will be used to authenticate. If we had a variety of devices, we could create a different token for each device. 

1. Click the **Dashboard** tab at the top of the page, and then click **Connection Information**. Notice the tokens are part of the connection strings. Take note of the two connection strings; you'll need them later in this module.

1. Your Event Hub is now created, and you have the connection strings you need to send and receive events.

<a name="Ex2Task2"></a>
#### Task 2 - Integrating Event Hubs in the PartsUnlimited solution ####

In this task, you'll update the website so it sends events to your Event Hub, in particular when a user browses a product or adds a product to the cart.

1. Open in Visual Studio the **PartsUnlimited.sln** solution located at **Source / Ex1 / Begin** folder.

1. In Solution Explorer, right-click the solution, and then click **Manage NuGet Packages for Solution...**.

1. Search for **Microsoft Azure Service Bus**, select the NuGet package with id **WindowsAzure.ServiceBus**, check the **src\PartsUnlimited** project and click **Install**. Alternatively, you can open the Package Manager Console and run:

	````
	Install-Package WindowsAzure.ServiceBus
	````

1. Create a new API controller within the **api** folder and name it **EventsController**; replace the content of the file with the following code snippet. Make sure to update the static fields **eventHubName** and the **connectionString** with the values from the previous task.

	(Code Snippet - _IntelligentApplication - EventsController_)
	<!-- mark:1-22 -->
	````C#
	namespace PartsUnlimited.api
	{
	    using System.Text;
	    using Microsoft.AspNet.Mvc;
	    using Microsoft.ServiceBus.Messaging;

	    [Route("api/events")]
	    public class EventsController : Controller
	    {
	        private static string eventHubName = "{event hub name}";
	        private static string connectionString ="{SendRule Connection String}";

	        // POST api/values
	        [HttpPost]
	        public void Post([FromForm]string serializedEventMessage)
	        {
	            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);
	            var eventData = new EventData(Encoding.UTF8.GetBytes(serializedEventMessage));
	            eventHubClient.Send(eventData);
	        }
	    }
	}
	````

	>**Note:** This Controller method is an HTTP POST and the [FromForm] attribute tells MVC to get the value of the serializedEventMessage from the form of the request. The event is a JSON object that is sent in the following step. Additionally, the Event Hub client sends the data to the Event Hub that you created before.

1. Go to the **Details.cshtml** file located in the **Views / Store** folder and add the following code snippet at the end of the **scripts** section.

	(Code Snippet - _IntelligentApplication - DetailsViewScript_)
	<!-- mark:2-20 -->
	````JavaScript
	<script type="text/javascript">
        $(function () {
            function sendMessage(type){
                var data = {
                    type: type,
										productId : @Html.Raw(Model.ProductId),
                    title: '@Html.Raw(Model.Title)',
                    category: '@Html.Raw(Model.Category.Name)',
                };

                var message = JSON.stringify(data);
                $.post('/api/events', { 'serializedEventMessage': message });
            }

            sendMessage('view');

            $('a.btn').click(function() {
                sendMessage('add');
            });
        });
    </script>
	````

	>**Note:** This client script calls the HTTP Post method that we previously created and sends a JSON object whenever a product is viewed or added to the cart.


<a name="Ex2Task3"></a>
#### Task 3 - Verifying the website events in Event Hubs ####

In this task, you'll verify that the events are being sent to your Event Hub.

1. In Visual Studio, run the solution.

1. Once the site is loaded, navigate to a product and click in the "Add to Cart" button. You can do this for several items and each view and add to cart action will generate an event that will be sent to the Event Hub.

1. In the [Azure classic portal](https://manage.windowsazure.com/), go to **Service Bus** and select the one you created before.

1. Click the **CONNECTION INFORMATION** button below and copy the **RootManageSharedAccessKey**; you'll need it to connect via the ServiceBusExplorer in the next step.

	![Getting the service bus RootManageSharedAccessKey](Images/service-bus-connection.png?raw=true "Getting the service bus RootManageSharedAccessKey")

	_Getting the service bus RootManageSharedAccessKey_

1. Run **Service Bus Explorer**.

1. Click **File** -> **Connect**, select _Enter connection string_ from the Service Bus Namespaces list and paste the **RootManageSharedAccessKey** you copied before.

	![Connect to a Service Bus Namespace](Images/service-bus-enter-connection.png?raw=true "Connect to a Service Bus Namespace")

	_Connect to a Service Bus Namespace_

1. Go to **Event Hubs** -> **eventhubdatamodule** -> **Consumer Groups** -> **$Default** -> **Partitions** -> **00** and right click to "Create Partition Listener".

	![Creating a Partition Listener](Images/creating-partition-listener.png?raw=true "Creating a Partition Listener")

	_Creating a Partition Listener_

1. Go to the **Events** tab and click **Start**.

	![Viewing the events list](Images/viewing-events-list.png?raw=true "Viewing the events list")

	_Viewing the events list_

<a name="Exercise3"></a>
### Exercise 3: Using Stream Analytics to process your data ###

Now that we have a stream of events, you'll set up a Stream Analytics job to analyze these events in real-time.
Azure Stream Analytics (ASA) is a fully managed, cost effective real-time event processing engine that helps to unlock deep insights from data. Stream Analytics makes it easy to set up real-time analytic computations on data streaming from devices, sensors, web sites, social media, applications, infrastructure systems, and more.

<a name="Ex3Task1"></a>
#### Task 1 - Configuring and starting event generator application ####

In this task, you'll set up and run a console application that will randomly create and send events - such as add, view, checkout and remove - to your Event Hub. Later in this module, you'll visualize these events in Power BI.

1. Open in Visual Studio the **PartsUnlimitedDataGen.sln** solution located at **Source / Ex3 / Begin** folder.

1. Replace the **eventHubName** and  **eventHubConnectionString** values in **Program.cs** with the connection string and name of the **SendRule** from your Event Hub .

1. Build the solution to trigger the download of required NuGet packages.

1. Run the application.

	![Generating events](Images/events-generator.png?raw=true "Generating events")

	_Generating events_

<a name="Ex3Task2"></a>
#### Task 2 - Creating Stream Analytics job ####

In this task, you'll set up a Stream Analytics job to analyze the events in real-time.

1. In the [Azure portal](https://portal.azure.com/), click **New** > **Data + Analytics** > **Stream Analytics job**.
Specify the following values, and then click **Create**:

	- **Job Name**: Enter a job name.
	- **Region**: Select the region where you want to run the job. Consider placing the job and the event hub in the same region to ensure better performance and to ensure that you won't be paying to transfer data between regions.
	- **Storage Account**: Choose the Azure storage account that you'd like to use to store monitoring data for all Stream Analytics jobs running within this region. You have the option to choose an existing storage account or to create a new one.

	![Creating Stream Analytics job](Images/new-stream-analytics.png?raw=true "Creating Stream Analytics job")

	_Creating Stream Analytics job_

1. The new job will be shown with a status of Created. Notice that the Start button is disabled. You must configure the job **Input**, **Output**, and **Query** before you can start the job.

<a name="Ex3Task3"></a>
#### Task 3 - Specifying job Input ####

In this task, you'll specify a job Input using the Event Hub you previously created.

1. In your Stream Analytics job topology click **Inputs**, and then click **Add**. The blade to create a new Input appears on the right.

1. Type or select the following values for each setting:

	- **Input Alias**: Enter a friendly name for this job input such as CallStream. Note that you'll be using this name in the query later.
	- **Source Type**: Select Data Stream.
	- **Source**: Event Hub.
	- **Service bus namespace**: Use the one you used for the Event Hub you created.
	- **Event Hub name**: Use the one you created in the previous task.
	- **Event Hub policy name**: The receive policy name you set in the previous task when creating the Event Hub.
	- **Event hub policy key**: The key you copied in the previous task when creating the Event Hub.
	- **Event Serializer Format**: JSON.
	- **Encoding**: UTF8.

	![Creating Stream Analytics Input](Images/new-stream-analytics-input.png?raw=true "Creating Stream Analytics Input")

	_Creating Stream Analytics Input_

1. Click **Create**.

<a name="Ex3Task4"></a>
#### Task 4 - Specifying job Query ####

Stream Analytics supports a simple, declarative query model for describing transformations for real-time processing. To learn more about the language, see the [Azure Stream Analytics Query Language Reference](https://msdn.microsoft.com/library/dn834998.aspx).
In this task, you'll create a query that extracts events from your input stream.

1. Click **Query** from the main Stream Analytics job page.

1. Add the following to the code editor:

	````
	SELECT
	    *
	INTO
	    analyticsoutput
	FROM
	    CallStream
	````

1. Click **Save**.

	>**Note:** In this query you're projecting all fields in the payload of the event to the output, you could read some of them of them by using _SELECT [field name]_.

<a name="Ex3Task5"></a>
#### Task 5 - Specifying job Output ####

In this task, you'll create an output that will store the query results in Blob storage.

1. Use an existing storage account or create a new storage account by following these steps: [Create a storage account](https://azure.microsoft.com/documentation/articles/storage-create-storage-account/#create-a-storage-account).

1. Create a Container such as eventhubanalytics and set its access to Blob.

	1. Open the **Azure Storage Explorer** or the tool of your preference and configure a new storage account using the account name and key from the previous step. In _Azure Storage Explorer_, right-click on **Storage Accounts**, select **Attach External Storage...** and enter the account name and key in the dialog, then click **OK**.

	1. Create a new Blob Container with the name "**eventhubanalytics**" and "Container" access level. In _Azure Storage Explorer_ expand your account and right-click on **Blob Containers**, select **Create Blob Container** and enter "eventhubanalytics". Press enter to create the container. Then right-click on the new container and select **Set Public Access Level..** and choose **Public read access for blobs**.

1. Now, in your Stream Analytics job, click **Outputs** from the main page, and then click **Add**. The options blade requires the following information:

	- **Output alias**: Use the name you set in the previous task, e.g. analyticsoutput
	- **Sink**: Blob storage.
	- **Storage account**: Select the name of the storage account.
	- **Storage account key**: Set the account key.
	- **Container**: Select the name of the container.
	- **Path pattern**: Type in a file prefix to use when writing blob output. E.g. analyticsoutput-{date}
	- **Event Serializer Format**: We will use CSV.
	- **Delimiter**: comma (,).
	- **Encoding**: UTF8.

1. Click **Create**.

	![Creating Stream Analytics Output](Images/new-stream-analytics-output.png?raw=true "Creating Stream Analytics Output")

	_Creating Stream Analytics Output_

<a name="Ex3Task6"></a>
#### Task 6 - Starting the job for real time processing ####

In this task you'll run the Stream Analytics job and view the output in Visual Studio.

1. From the job **Dashboard**, click the **Start** button.

1. In the **Start job** blade, select Job output start time **Now**, and then click **Start** at the bottom of the blade. The job status will change to Starting; it can take several minutes to start.

1. Open the blob storage using **Azure Storage Explorer**.

1. Navigate to the container you set in the previous task.

	![Browsing container](Images/reviewing-analytics-job.png?raw=true "Browsing container")

	_Browsing container_

1. Open the csv file to see the output.

	![Reviewing Stream Analytics job output](Images/reviewing-analytics-job-output.png?raw=true "Reviewing Analytics job output")

	_Reviewing the Stream Analytics job output_

<a name="Exercise4"></a>
### Exercise 4: Visualizing your data with Power BI ###

In this exercise, you'll use Azure Stream Analytics with Microsoft Power BI. You will learn how to build a live dashboard quickly.

<a name="Ex4Task1"></a>
#### Task 1 - Adding Power BI output to Stream Analytics ####

In this task, you'll add a new output to your Stream Analytics job.

1. From the [Azure classic portal](https://manage.windowsazure.com/), go to Stream Analytics and click the one you created.

1. Click the **STOP** button below. We need to stop it in order to add a new output.

1. Click **OUTPUTS** from the top of the page, and then click **Add Output**.

1. In the **Add an Output** dialog box, select **Power BI** and then click the right button.

1. In the **Add a Microsoft Power BI output**, supply a work or school account for the Stream Analytics job output. If you already have Power BI account, select **Authorize Now**. If not, choose **Sign up now**.

	![Adding Power BI output](Images/adding-powerbi-output.png?raw=true "Adding Power BI output")

	_Adding Power BI output_

1. Next, provide the values for:

	- **Output Alias** – You can put any output alias that is easy for you to refer to. This output alias is particularly helpful if you decide to have multiple outputs for your job. In that case, you have to refer to this output in your query. For example, let’s use the output alias value “OutPowerBI”.
	- **Dataset Name** - Provide a dataset name that you want your Power BI output to have. For example, let’s use “datamodulepbi”.
	- **Table Name** - Provide a table name under the dataset of your Power BI output. Let’s say we call it “datamodulepbi”. Currently, Power BI output from Stream Analytics jobs may only have one table in a dataset.
	- **Workspace** - You can use the default, My Workspace.

1. Click **OK, Test Connection** and now your Power BI connection is completed.

1. Lastly, you should update your **Query** to use this output and start the job.

	````
	WITH AllEvents AS (
	SELECT
	    productId, title, category, type, Count(title) AS [total]
	FROM
	    CallStream
	GROUP BY
	    productId, title, category, type, TumblingWindow(minute, 10)
	)
	SELECT * INTO OutPowerBI FROM AllEvents
	SELECT * INTO analyticsoutput FROM CallStream
	````

	>**Note:** As we are grouping the results, a window type is required. See [GROUP BY](https://msdn.microsoft.com/library/azure/dn835023.aspx). The query uses a 10-minute tumbling window. The INTO clause tells Stream Analytics which of the outputs to write the data from this statement. The WITH statement is to reuse the results for different statements; in this case we could used it for both outputs but we'll keep storing all fields into the blob.

<a name="Ex4Task2"></a>
#### Task 2 - Creating the dashboard in Power BI ####

1. Go to [PowerBI.com](https://powerbi.microsoft.com/) and login with your work or school account. If the Stream Analytics job query outputs results, you'll see your dataset is already created:

1. For creating the dashboard, go to the **Dashboards** option and create a new Dashboard, e.g. My Dashboard.

1. Now click the dataset created by your Stream Analytics job (“datamodulepbi” in our current example). You will be taken to a page to create a chart on top of this dataset.

	![Power BI Workspace](Images/powerbi-workspace.png?raw=true "Power BI Workspace")

	_Power BI Workspace_

1. Select the **Table visualization** icon from the **Visualizations** menu on the right, then check all fields but productId from the **Fields** list.

1. Within the **Filters** section, click **category** Advanced filtering and select the option to show items when the value _is not blank_.

1. Apply the filter and click **Save** on the top right. You can name it "Events report".

1. You will see the new report within the **Reports** section, click on it, select **Pin Live Page** to your existing dashboard.

1. Go to your dashboard, click on the **ellipsis** button at the top-right corner of the tile and click the **pen** button to edit the _Tile details_, select the **Display last refresh time** functionality and apply the changes.

	![Power BI new report](Images/powerbi-dashboard.png?raw=true "Power BI new report")

	_Power BI new report_

1. Go back to the "datamodulepbi" dataset, click the **Funnel** icon from the **Visualization** menu and check _type_ and _total_ from the **Fields** list.

1. Click **Save** on the top right, enter a name like Events Summary and save it.

1. Lets add it to your dashboard by clicking **Pin Live Page** on the top right, select your dashboard and hit **Pin Live**.

	![Power BI Dashboard](Images/powerbi-my-dashboard.png?raw=true "Power BI Dashboard")

	_Power BI Dashboard_

---

<a name="Summary"></a>
## Summary ##

By completing this module, you should have:

- Created an **Event Hub** and integrated it into a Web App
- Walked through **DocumentDB** integration
- Used **Stream Analytics** to process data in near-realtime and spool data to **Blob Storage** and **Power BI**
- Created sample **Power BI** charts & graphs
