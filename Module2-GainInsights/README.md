<a name="Top"></a>
# Gain Insights from your Data #

---

<a name="Overview"></a>
## Overview ##

Azure Data Factory is a cloud-based data integration service that orchestrates and automates the movement and transformation of data. It works across on-premises and cloud data sources and SaaS to ingest, prepare, transform, analyze, and publish your data.

Transformation activities in Azure Data Factory transform and process your raw data into predictions and insights. The transformation activity executes in a computing environment such as Azure HDInsight cluster or an Azure Batch. You will create an HDInsight Hive activity to execute a Hive query.

Azure SQL Data Warehouse is an enterprise-class, distributed database capable of processing massive volumes of relational and non-relational data. It's the industry's first cloud data warehouse that combines proven SQL capabilities with the ability to grow, shrink, and pause in seconds. SQL Data Warehouse is also deeply ingrained into Azure, and you'll use Data Factory to move generated statistics into SQL Data Warehouse and visualize it in Power BI.

In this module, you'll use Data Factory to compose services into managed data flow pipelines to transform the data ingested from the Parts Unlimited logs into meaningful data to be visualized using Power BI.

<a name="Objectives"></a>
### Objectives ###
In this module, you'll see how to:

- Create an **Azure Data Factory**
- Create a **HDInsight Hive** query to do analytics on your data
- Orchestrate an **Azure Data Factory** workflow
- Data movement from **HDInsight** to a **SQL Data Warehouse**
- Use the data in **SQL Data Warehouse** to generate **Power BI** visualizations

<a name="Prerequisites"></a>
### Prerequisites ###

The following is required to complete this module:

- [Microsoft Visual Studio Community 2015][1] or greater
- [Microsoft Azure Storage Explorer][2] or any other tool to manage Azure Storage

[1]: https://www.visualstudio.com/products/visual-studio-community-vs
[2]: http://storageexplorer.com/

<a name="Setup"></a>
### Setup ###
In order to run the exercises in this module, you'll need to create an HDI cluster, a SQL Data Warehouse, a storage account (or reuse one from previous module) and upload some asset files. You can do this manually or run the **Setup.cmd** script in the **Setup** folder as detailed below.

> **Important:** when entering the names for the storage account, SQL DW server and HDInsight server, those must be **globally unique**. To ensure this, you can use your first and last name as prefixes for the resource names. For instance: "johndoestorage", "johndoehdi" and "johndoesqlserver".


<a name="SetupScript"></a>
#### Using the setup scripts ####

1. Open Windows Explorer and browse to the module's **Setup** folder.
1. Right-click **Setup.cmd** and select **Run as Administrator** to launch the setup process.
1. If the _User Account Control_ dialog box is shown, confirm the action to proceed.
1. Select a setup operation from the menu. First generate the sample data files and then proceed with the next tasks in order.

	![Setup menu](Images/setup-menu.png?raw=true "Setup menu")

	_Setup menu_

1. If prompted, log in with credentials where you have an available Azure subscription. You will be able to select one if you have multiple subscriptions.

1. Complete the prompted values as requested on each setup step. **Take note of the account names, keys and passwords as they will be required for the exercises**. Also, the settings will be stored in .txt files inside the "Setup" folder.

	The HDI cluster provisioning may take around 15 minutes. You can enter the [Microsoft Azure portal](https://portal.azure.com/) to check the status of the HDI cluster provisioning.

	![HDI cluster in progress](Images/setup-inprogress-hdi.png?raw=true "HDI cluster in progress")

	_HDI cluster in progress_

    **Note**: to clean your subscription just delete the 'DataCodeLab' resource group and all the associated resources.

Now you can skip the manual setup instructions and start executing the exercises.

<a name="ManualSetupHDI"></a>
#### 1- Manually creating the HDI cluster ####

1. In the [Microsoft Azure portal](https://portal.azure.com/), create a new HDI cluster (_New > Internet of Things > HDInsight_).

1. Enter a globally unique name for the cluster.

1. Select **Hadoop** cluster type.

	HDInsight allows to deploy a variety of cluster types, for different data analytics workloads. Cluster types offered are:

	- _Hadoop: for query and analysis workloads_
	- HBase: for NoSQL workloads
	- Storm: for real time event processing workloads
	- Spark: for in-memory processing, interactive queries, stream, and machines learning workloads.

1. Select an operating system for the cluster (you can choose between either Windows or Linux) and use the latest version ("Hadoop 2.7.0 (HDI 3.3)" or latest)

1. Select the same Resource Group as the used for the Storage account ("**DataCodeLab**").

1. Configure the cluster login credentials (for instance: **admin/P@ssword**) and the SSH creadetials if using Linux. No need to enable **Remote Desktop**. You will use the cluster credentails to access the Hive dashboard (for Windows) or Ambari portal (for Linux) when testing the HQL queries.

1. Select the existing storage account you created in the previous setup task for the _data source_ and use "**hdi**" for the default container name.

1. You can use 1 for the worker nodes number and the lowest pricing tiers for this module.

	![Create HDI cluster](Images/setup-create-hdi.png?raw=true "Create HDI cluster")

	_Create HDI cluster_

1. Click **Create**. The provisioning may take 15 minutes to complete.

	> **Note:** For this module we don't need to persist the cluster when deleted, so we won't configure a Hive and Oozie metastore for it. 

	> 	Using the metastore helps you to retain your Hive and Oozie metadata, so that you don't need to re-create Hive tables or Oozie jobs when you create a new cluster. By default, Hive uses an embedded Azure SQL database to store this information. The embedded database can't preserve the metadata when the cluster is deleted. 

<a name="ManualSetupUploadFiles"></a>
#### 2- Manually uploading the sample files ####

1. In the [Microsoft Azure portal](https://portal.azure.com/), create a new Storage Account or find an existing one to reuse.
 1. Make sure to use the "_Resource Manager_" for the deployment mode.
 1. Select the "_Standard Locally Redundant_" (Standard-LRS) storage account type.
 1. Create a new _Resource Group_ (for instance "**DataCodeLab**") and make sure to use the same when creating the HDI cluster, SQL Data Warehouse and Data Factory.
 1. Select a location (for instance **West US**) and make sure to use the same for the other services.
 1. Click **Create**.

	![New storage account](Images/setup-new-storage.png?raw=true "New storage account")

	_New storage account_

1. Open Windows Explorer and browse to the module's **Setup** folder.

1. Right-click **Setup.cmd** and select **Run as Administrator** to launch the setup process.

1. Execute just the operation **1** to generate the sample data files and exit the script. The files will be generated in **Setup\Assets\Logs** using date partitioned folders.

	> **Note:** Azure Data Factory supports partitioned data. You can specify a dynamic folder path and file name for time series data with the "partitionedBy" section when defining the pipeline activities, also using Data Factory macros and the system variables: SliceStart and SliceEnd, which indicate start and end times for a given data slice. For example:
	> 
	> ````JavaScript
		"folderPath": "partsunlimited/Logs/{Year}/{Month}/{Day}",
		"partitionedBy": 
		 [
			 { "name": "Year", "value": { "type": "DateTime", "date": "SliceStart", "format": "yyyy" } },
			 { "name": "Month", "value": { "type": "DateTime", "date": "SliceStart", "format": "MM" } }, 
			 { "name": "Day", "value": { "type": "DateTime", "date": "SliceStart", "format": "dd" } }
		],
	> ````

1. Go back to the Azure portal and check if the storage provisioning is complete. **Take note of the storage account name and key values from the settings pane.

	![Getting account name and key](Images/setup-storage-key.png?raw=true "Getting account name and key")

	_Getting account name and key_

1. Open the **Azure Storage Explorer** or the tool of your preference and configure a new storage account using the account name and key from the previous step. In _Azure Storage Explorer_, right-click on **Storage Accounts**, select **Attach External Storage...** and enter the account name and key in the dialog, then click **OK**.

1. Create a new Blob Container with the name "**partsunlimited**" and "Container" access level. In _Azure Storage Explorer_ expand your account and right-click on **Blob Containers**, select **Create Blob Container** and enter "partsunlimited". Press enter to create the container. Then right-click on the new container and select **Set Public Access Level..** and choose **Public read access for container and blobs**.

1. Copy the whole **Logs** folder (in _Setup\Assets_) into the **partsunlimited** container to upload all the data files. In _Azure Storage Explorer_, select the **partsunlimited** container, click **Upload** and choose **Upload folder**. In the _Upload folder_ dialog, select the **Logs** folder (from _Setup\Assets_) and then click **Upload**

1. Repeat to upload the **Scripts** folder.

> **Note:** Alternativelly, you could use the [Blob Service REST API](https://msdn.microsoft.com/en-us/library/azure/dd135733.aspx) to automate the files upload.

<a name="ManualSetupSqlDW"></a>
#### 3- Manually creating the SQL Data Warehouse ####

1. In the [Microsoft Azure portal](https://portal.azure.com/), create a new SQL Data Warehouse (_New > Data + Storage > SQL Data Warehouse_).

1. Enter "**partsunlimited**" for the database name.

1. The minimun performance of 100 DWU will be enough for this module.

1. Create a new SQL Server for hosting the Data Warehouse.
 1. Select **Create a new server** in the Server blade.
 1. Enter a globally unique name for the server.
 1. Enter a admin login name: **dwadmin**
 1. Write a password for the server login: **P@ssword123**
 1. Repeat the password to confirm it.
 1. Select the same location used for the other services.

1. Use **Blank database** for the source so a new empty database is used (it will use the "partsunlimited" name).

1. Select the resource group used for all the services: **DataCodeLab**.

	![Create SQL Data Warehouse](Images/setup-create-dw.png?raw=true "SQL Data Warehouse")

	_SQL Data Warehouse_

1. Click **Create**.

1. Once created, in the SQL Data Warehouse blade, click on the server name to open the Sql Server settings.

1. Click "Show firewall settings".

1. Click on "Add client IP" to add a rule to enable access for your machine.

	![Firewall config](Images/setup-dw-firewall.png?raw=true "Firewall config")

	_Firewall config_

1. Click **Save**.

1. In the SQL Data Warehouse blade, click **Open in Visual Studio**. In the new blade, click "Open in Visual Studio".

	![Open Data Warehouse in Visual Studio](Images/setup-open-vs.png?raw=true "Open Data Warehouse in Visual Studio")

	_Open Data Warehouse in Visual Studio_

1. Confirm you want ot switch apps if prompted.

1. In Visual Studio, enter the SQL server credentials (dwadmin/P@ssword123).

1. In the _SQL Server Object Explorer_, right-click the **partsunlimited** database and select **New Query...**

1. In the new query window add the following script to create the tables and stored procedure used in this module.

	````SQL
	CREATE TABLE dbo.ProductLogs
	 (productid int, title nvarchar(50), category nvarchar(50), type nvarchar(5), total int)
	GO
	
	CREATE TABLE dbo.ProductStats
	 (category nvarchar(50), title nvarchar(50), views int, adds int)
	GO
	
	CREATE PROCEDURE sp_populate_stats AS
	BEGIN
	 DELETE FROM dbo.ProductStats;
	 INSERT INTO dbo.ProductStats 
	 SELECT
	  category,
	  title,
	  SUM(CASE WHEN type = 'view' THEN total ELSE 0 END) AS views,
	  SUM(CASE WHEN type = 'add' THEN total ELSE 0 END) AS adds
	 FROM dbo.ProductLogs GROUP BY title, category
	END
	GO
	````

1. Click execute button (Ctrl+Shift+E) to run the query.

	![Creating schema for Data Warehouse](Images/setup-run-sql.png?raw=true "Creating schema for Data Warehouse")

	_Creating schema for Data Warehouse_

1. Close Visual Studio. You don't need to save changes.

---

<a name="Exercises"></a>
## Exercises ##
This module includes the following exercises:

1. [Creating Azure Data Factory](#Exercise1)
1. [Creating Hive code to do analytics on your data](#Exercise2)
1. [Orchestrating Azure Data Factory workflow](#Exercise3)
1. [Using the data in the warehouse to generate Power BI visualizations](#Exercise4)

Estimated time to complete this module: **60 minutes**

<a name="Exercise1"></a>
### Exercise 1: Creating Azure Data Factory ###

Azure Data Factory has a few key entities that work together to define the input and output data, processing events, and the schedule and resources required to execute the desired data flow.

- **Activities**: Activities define the actions to perform on your data. Each activity takes zero or more datasets as inputs and produces one or more datasets as outputs.
- **Pipelines**: Pipelines are a logical grouping of Activities. They are used to group activities into a unit that together perform a task (i.e. clean log).
- **Datasets**: Datasets are named references/pointers to the data you want to use as an input or an output of an Activity. Datasets identify data structures within different data stores including tables, files, folders, and documents.
- **Linked service**: Linked services define the information needed for Data Factory to connect to external resources (data stores or compute resources for hosting activities execution).

In this exercise, you'll create a Data Factory with the linked services (Azure Storage, HDI cluster and SQL Data Warehouse) and datasets for the ingest data and output stores.


![Data factory key concepts](Images/data-factory-exercises.png?raw=true "Data factory key concepts")

_Data factory key concepts_

> **Note:** If you need to skip the manual instructions to create the data factory and provisioning of the linked services, datasets and pipelines. You can run the CreateDataFactory.cmd script to quickly provision it.


<a name="Ex1Task1"></a>
#### Task 1 - Creating the data factory in Azure Portal ####

In this task, you create a data factory to orchestrate the linked services and data transformation activities.

1. On the Hub menu, select **New -> Data + Analytics -> Data Factory**.

1. Use the following values to create the new data factory:

 1. Enter a name for the data factory (for instance: **"PartsUnlimitedDataFactory"**).
 1. In the **Resource group name** blade, click **Or create new** and use **"DataCodeLab"** for the name.
 1. Make sure the **Pin to dashboard** is selected so you can have a quick access and click **Create**.

	![New data factory blade](Images/ex1task2-new-data-factory.png?raw=true "New data factory blade")

	_New data factory blade_

1. After the creation is complete, you'll find the data factory in the dashboard.

<a name="Ex1Task2"></a>
#### Task 2 - Adding the linked services ####

In this task, you'll create the linked services.

1. In the *Data Factory* blade, click **Author and deploy** tile to launch the *Editor* for the data factory.

	![Author and deploy](Images/ex1task2-author-and-deploy.png?raw=true "Author and deploy")

	_Author and deploy_

1. In the **Editor**, click **New data store** button on the toolbar and select **Azure storage** from the drop down menu. You should see the JSON template for creating an Azure storage linked service in the right pane.

	![New Azure storage data store](Images/ex1task2-new-storage.png?raw=true "New Azure storage data store")

	_New Azure storage data store_

1. Replace **<****accountname****>** and **<****accountkey****>** placeholders with the account name and key values of the Azure storage account where the Parts Unlimited logs are stored (created with Setup.cmd script or with manual steps).

	````JavaScript
	{
		 "name": "AzureStorageLinkedService",
		 "properties": {
			  "type": "AzureStorage",
			  "description": "",
			  "typeProperties": {
					"connectionString": "DefaultEndpointsProtocol=https;AccountName=<accountname>;AccountKey=<accountkey>"
			  }
		 }
	}
	````

    See [JSON Scripting Reference](http://go.microsoft.com/fwlink/?linkid=516971&clcid=0x409) for details about JSON properties.

1. Click **Deploy** on the toolbar to deploy the **StorageLinkedService**.

	![Deploy linked service](Images/ex1task2-deploy.png?raw=true "Deploy linked service")

	_Deploy linked service_

1. Confirm that you see the message *Linked service deployed successfully* on the title bar.

	![Linked service deployed successfully](Images/ex1task2-deployed.png?raw=true "Linked service deployed successfully")

	_Linked service deployed successfully_

1. Repeat to create another service for the SQL Data Warehouse server. Click **New data store** button on the toolbar and select **Azure SQL Data Warehouse** from the drop down menu.

1. Replace placeholders in the connectionString (**<****servername****>**, **<****databasename****>**, **<****username****>** and **<****password****>**) with the setting values used when creating the SQL Data Warehouse database in setup script.

	````JavaScript
	{
		 "name": "AzureSqlDWLinkedService",
		 "properties": {
			  "type": "AzureSqlDW",
			  "description": "",
			  "typeProperties": {
					"connectionString": "Data Source=tcp:<servername>.database.windows.net,1433;Initial Catalog=partsunlimited;User ID=dwadmin@<servername>;Password=P@ssword123;Integrated Security=False;Encrypt=True;Connect Timeout=30"
			  }
		 }
	}
	````

1. Click **Deploy** on the toolbar to create and deploy the **AzureSqlDWLinkedService**.

1. Now, click **New compute** button on the toolbar and select **HDInsight cluster** from the drop down menu. You should see the JSON template for creating the *HDInsight cluster linked service* in the right pane.

    **Note**: Alternatively, you can use a [On Demand HDInsight cluster](https://azure.microsoft.com/en-us/documentation/articles/data-factory-compute-linked-services/#azure-hdinsight-on-demand-linked-service), but it will take around 15 minutes to create the cluster when running the Data Factory. Use this approach only if the provided cluster is not available and make sure to specify the proper password.

1. Update the setting placeholders as shown in the snippet below. Replace the **<****hdiclustername****>** placeholder with the name you used when running the **Setup.cmd** script.

	````JavaScript
	{
        "name": "HDInsightLinkedService",
        "properties": {
            "type": "HDInsight",
            "description": "",
            "typeProperties": {
                "clusterUri": "https://<hdiclustername>.azurehdinsight.net/",
                "userName": "admin",
                "password": "P@ssword123",
                "linkedServiceName": "AzureStorageLinkedService"
            }
        }
    }
	````

1. Click **Deploy** on the toolbar to create and deploy the **HDInsightLinkedService**.

	![Linked services created](Images/ex1task2-linked-services.png?raw=true "Linked services created")

	_Linked services created_

    **Note:** if the deployment fails, it may be due to the HDI cluster is still being provisioned. Verify the status in the portal.

<a name="Ex1Task3"></a>
#### Task 3 - Creating the input and output datasets ####

In this task, you'll create the input and output tables corresponding to the linked services.

1. In the Editor for the Data Factory, click **New dataset** button on the toolbar and click **Azure Blob storage** from the drop down menu.

1. Change the **name** propety to "LogJsonFromBlob" and the **linkedServiceName** property to "AzureStorageLinkedService".

1. Remove the **structure** property. The file is in JSON format and the structure can be omitted.

1. Change the **typeProperties** to use the _/year/month/day_ partition for the blob paths.

	````JavaScript
	"typeProperties": {
		"folderPath": "partsunlimited/Logs/{Year}/{Month}/{Day}",
		"partitionedBy": 
		[
			 { "name": "Year", "value": { "type": "DateTime", "date": "SliceStart", "format": "yyyy" } },
			 { "name": "Month", "value": { "type": "DateTime", "date": "SliceStart", "format": "MM" } }, 
			 { "name": "Day", "value": { "type": "DateTime", "date": "SliceStart", "format": "dd" } }
		],
		"format": {
			"type": "TextFormat"
		}
	},
	````

1. Set the availability frequency to daily.

	````JavaScript
	"availability": {
		"frequency": "Day",
		"interval": 1
	},
	````

1. Add an **external** property to the dataset and set it to **true**. This setting informs the Data Factory service that this table is external to the data factory and not produced by an activity in the data factory.

	````JavaScript
	"external": true
	````

    The final _LogJsonFromBlob_ dataset should look like the following snippet:
    
	````JavaScript
	{
		"name": "LogJsonFromBlob",
		"properties": {
			"type": "AzureBlob",
			"linkedServiceName": "AzureStorageLinkedService",
			"typeProperties": {
				"folderPath": "partsunlimited/Logs/{Year}/{Month}/{Day}",
				"partitionedBy": 
				[
					 { "name": "Year", "value": { "type": "DateTime", "date": "SliceStart", "format": "yyyy" } },
					 { "name": "Month", "value": { "type": "DateTime", "date": "SliceStart", "format": "MM" } }, 
					 { "name": "Day", "value": { "type": "DateTime", "date": "SliceStart", "format": "dd" } }
				],
				"format": {
					"type": "TextFormat"
				}
			},
			"availability": {
				"frequency": "Day",
				"interval": 1
			},
			"external": true
		}
	}
	````

1. Click **Deploy** on the toolbar to create the input dataset.

1. Repeat the steps to create another dataset for output with the following settings:

 1. **name**: "LogCsvFromBlob"
 1. **linkedServiceName**: "AzureStorageLinkedService"
 1. structure (this file is structured using CSV format with the fields specified below):

		````JavaScript
		"structure": [
			{
				"name": "productid",
				"type": "Int32"
			},
			{
				"name": "title",
				"type": "String"
			},
			{
				"name": "category",
				"type": "String"
			},
			{
				"name": "type",
				"type": "String"
			},
			{
				"name": "total",
				"type": "Int32"
			}
		],
		````

 1. typeProperties (the format will use the defaults for CSV: comma -,- for columns delimiter and line breaks -\n- for row delimiter):

		````JavaScript
		"typeProperties": {
			"folderPath": "partsunlimited/Stats/{Year}/{Month}/{Day}",
			"partitionedBy": 
			[
				 { "name": "Year", "value": { "type": "DateTime", "date": "SliceStart", "format": "yyyy" } },
				 { "name": "Month", "value": { "type": "DateTime", "date": "SliceStart", "format": "MM" } }, 
				 { "name": "Day", "value": { "type": "DateTime", "date": "SliceStart", "format": "dd" } }
			],
			"format": {
				"type": "TextFormat"
			}
		},
		````

 1. Daily availability:

		````JavaScript
		"availability": {
			"frequency": "Day",
			"interval": 1
		}
		````

 1. Make sure the final _LogCsvFromBlob_ dataset looks like the following snippet:
 
		````JavaScript
		{
			"name": "LogCsvFromBlob",
			"properties": {
				"type": "AzureBlob",
				"linkedServiceName": "AzureStorageLinkedService",
				"structure": [
					{
						"name": "productid",
						"type": "Int32"
					},
					{
						"name": "title",
						"type": "String"
					},
					{
						"name": "category",
						"type": "String"
					},
					{
						"name": "type",
						"type": "String"
					},
					{
						"name": "total",
						"type": "Int32"
					}
				],
				"typeProperties": {
					"folderPath": "partsunlimited/Stats/{Year}/{Month}/{Day}",
					"partitionedBy": 
					[
						 { "name": "Year", "value": { "type": "DateTime", "date": "SliceStart", "format": "yyyy" } },
						 { "name": "Month", "value": { "type": "DateTime", "date": "SliceStart", "format": "MM" } }, 
						 { "name": "Day", "value": { "type": "DateTime", "date": "SliceStart", "format": "dd" } }
					],
					"format": {
						"type": "TextFormat"
					}
				},
				"availability": {
					"frequency": "Day",
					"interval": 1
				}
			}
		}
		````

 1. Click **Deploy** on the toolbar to deploy the dataset.

1. Create the dataset for the SQL Data Warehouse output using the same structure:

 1. In the Editor for the Data Factory, click **New dataset** button on the toolbar and click **Azure SQL Data Warehouse** from the drop down menu.
 1. Set the **name** to "LogsSqlDWOuput" and the **linkedServiceName** to "AzureSqlDWLinkedService"
 1. Use the same structure as the previous dataset:

		````JavaScript
		"structure": [
			{
				"name": "productid",
				"type": "Int32"
			},
			{
				"name": "title",
				"type": "String"
			},
			{
				"name": "category",
				"type": "String"
			},
			{
				"name": "type",
				"type": "String"
			},
			{
				"name": "total",
				"type": "Int32"
			}
		],
		````

 1. Set the **tableName** property to **dbo.ProductLogs**:

		````JavaScript
		"typeProperties": {
			"tableName": "dbo.ProductLogs"
		},
		````

 1. Daily availability:

		````JavaScript
		"availability": {
			"frequency": "Day",
			"interval": 1
		}
		````

 1. Make sure the final _LogsSqlDWOutput_ dataset looks like the following snippet:
 
		````JavaScript
		{
			"name": "LogsSqlDWOutput",
			"properties": {
				"type": "AzureSqlDWTable",
				"linkedServiceName": "AzureSqlDWLinkedService",
				"structure": [
					{
						"name": "productid",
						"type": "Int32"
					},
					{
						"name": "title",
						"type": "String"
					},
					{
						"name": "category",
						"type": "String"
					},
					{
						"name": "type",
						"type": "String"
					},
					{
						"name": "total",
						"type": "Int32"
					}
				],
				"typeProperties": {
					"tableName": "dbo.ProductLogs"
				},
				"availability": {
					"frequency": "Day",
					"interval": 1
				}
			}
		}
		````

 1. Click **Deploy** on the toolbar to deploy the dataset.

		![Data sets created](Images/ex1task3-datasets-created.png?raw=true "Data sets created")

		_Data sets created_

Now you finished creating the input/output datasets and the associated linked services for the pipelines of the Data Factory. In the upcoming exercises you will create the pipelines using the datasets and linked services you just created.

<a name="Exercise2"></a>
### Exercise 2: Creating Hive code to do analytics on your data ###

**HDInsight** is a cloud implementation on **Microsoft Azure** of the rapidly expanding **Apache Hadoop** technology stack that is the go-to solution for big data analysis.

**Apache Hive** is a data warehouse system for Hadoop, which enables data summarization, querying, and analysis of data by using **HiveQL** (a query language similar to SQL). Hive can be used to interactively explore your data or to create reusable batch processing jobs.

In this exercise, you'll create a **Hive activity** using HQL script code to generate the output data in the Azure Storage linked service.

<a name="Ex2Task1"></a>
#### Task 1 - Writing a Hive query to analyze the logs ####

In this task, you'll write a Hive query to generate product stats (views and cart additions) from the Parts Unlimited logs.

1. Navigate to the HDInsight server dashboard: https://**{hdiclustername}**.azurehdinsight.net/. If the HDI server was created with _Linux_ the **Ambari** portal will be loaded.

1. Log in using the username 'admin' and password 'P@ssword123'

1. If the HDI cluster was created using **Hadoop on Windows** (default when using Setup scripts), follow these steps:

	1. Open the **Hive Editor** using the link in the top bar if the HDI cluster is running _Windows_. 

	1. Write HQL code to create a table from the existing blobs and parse the JSON format. Replace the **StorageAccountName**, update the folder to be valid (where the input logs were uploaded according to the current date) and paste the snippet below:

		````
		DROP TABLE IF EXISTS LogsRaw;
		CREATE EXTERNAL TABLE LogsRaw (jsonentry string) 
		STORED AS TEXTFILE LOCATION "wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/Logs/2016/03/30";

		SELECT CAST(get_json_object(jsonentry, "$.productid") as BIGINT),
				 get_json_object(jsonentry, "$.title"),
				 get_json_object(jsonentry, "$.category"),
				 get_json_object(jsonentry, "$.type"),
				 CAST(get_json_object(jsonentry, "$.total") as BIGINT)
		FROM LogsRaw
		````

		 **Note**: The CREATE EXTERNAL TABLE command that we used here, creates an external table, the data file can be located outside the default container and does not move the data file.

		![Hive editor](Images/ex2task1-hive-editor.png?raw=true "Hive editor")

		_Hive editor_

	1. Enter a name, e.g. "**Product Stats**", for the query and click **Submit** to create a job session for the query execution.

	1. Notice the new entry in the **Job Session** table and the status. It will say Initializing and then, Running.

		![Job session state](Images/ex2task1-job-session.png?raw=true "Job session state")

		_Job session state_

	1. Wait for the status to be Completed, click the query name to view the details. Notice the **Job Output** with the resulting values.

		![Query output](Images/ex2task1-query-output.png?raw=true "Query output")

		_Query output_

1. If the HDI cluster was created using **Hadoop on Linux**, follow these steps:

	1. Select the **Hive view** in the views button at the top bar.

		![Hive view option in Ambari](Images/ex2task1-ambari-hive-option.png?raw=true "Hive view option in Ambari")

		_Hive view option in Ambari_

	1. In the new worksheet, write HQL code to create a table from the existing blobs and parse the JSON format. Replace the **StorageAccountName**, update the folder to be valid (where the input logs were uploaded according to the current date) and paste the snippet below:

		````
		DROP TABLE IF EXISTS LogsRaw;
		CREATE EXTERNAL TABLE LogsRaw (jsonentry string) 
		STORED AS TEXTFILE LOCATION "wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/Logs/2016/03/30";

		SELECT CAST(get_json_object(jsonentry, "$.productid") as BIGINT),
				 get_json_object(jsonentry, "$.title"),
				 get_json_object(jsonentry, "$.category"),
				 get_json_object(jsonentry, "$.type"),
				 CAST(get_json_object(jsonentry, "$.total") as BIGINT)
		FROM LogsRaw
		````

		 **Note**: The CREATE EXTERNAL TABLE command that we used here, creates an external table, the data file can be located outside the default container and does not move the data file.

		![Hive editor](Images/ex2task1-ambari-hive-editor.png?raw=true "Hive editor")

		_Hive editor_

	1. Click **Execute** to run the query.

	1. Wait for the status to be Succeeded, select the results tab in the "Query Process Results" panel to show the values.

		![Query output](Images/ex2task1-ambari-query-output.png?raw=true "Query output")

		_Query output_

	> **Note:** Ambari portal offers many features over the Windows dashboard. You can create advanced visualization of the data results using charts, customize the Hive settings, create customized views, among many other options. To learn more about Ambari portal go to [Manage HDInsight clusters by using the Ambari Web UI](https://azure.microsoft.com/documentation/articles/hdinsight-hadoop-manage-ambari/).

<a name="Ex2Task2"></a>
#### Task 2 - Create the HQL script for the Hive Activity ####

In this task, you'll reuse the Hive query to generate the HQL script for the Hive activity that will generate the output stats in a new storage blob.

1. Open the file located in **Setup\Assets\Scripts\logstocsv.hql** and review its content:

	````
	DROP TABLE IF EXISTS LogsRaw;
	CREATE EXTERNAL TABLE LogsRaw (jsonentry string) 
	STORED AS TEXTFILE LOCATION "${hiveconf:inputjson}";

	DROP TABLE IF EXISTS OutputTable;
	CREATE EXTERNAL TABLE OutputTable
	(
		 productid       int,
		 title           string,
		 category        string,
		 type            string,
		 total            int
	)
	ROW FORMAT DELIMITED FIELDS TERMINATED BY ','
	LINES TERMINATED BY '10'
	STORED AS TEXTFILE LOCATION '${hiveconf:outputcsv}'
	TBLPROPERTIES("skip.header.line.count"="1");

	INSERT OVERWRITE TABLE OutputTable
	SELECT CAST(get_json_object(jsonentry, "$.productid") as BIGINT) as productid,
				get_json_object(jsonentry, "$.title") as title,
				get_json_object(jsonentry, "$.category") as category,
				get_json_object(jsonentry, "$.type") as type,
				CAST(get_json_object(jsonentry, "$.total") as BIGINT) as total
	FROM LogsRaw
	````

    Notice this script is, essentially, the Hive query you wrote and tested in the previous task but it also includes an output table to store the results in tabular format (CSV).

1. This script was already uploaded to your storage during the module setup by using the **Setup.cmd** script or manual steps. Verify the HQL script was uploaded by using the an storage explorer tool such as **Azure Storage Explorer** to navigate to the **Scripts** folder in the **partsunlimited** container.

	1. Open **Azure Storage Explorer**, right click on "Storage Account" tree item and select **Attach External Storage...**

		![Attaching to external storage](Images/ex2task2-explorer-add-account.png?raw=true "Attaching to external storage")

		_Attaching to external storage_

	1. Enter the account name and key of the storage account you created/reused in the setup steps (leave the default endpoint options). Then click **OK** to add the account to Azure Explorer.

		![Enter storage account name and key](Images/ex2task2-explorer-connect-account-key.png?raw=true "Enter storage account name and key")

		_Enter storage account name and key_

	1. In the left pane navigate to the **partsunlimited** container, open the **Scripts** folder and verify the **logstocsv.hql** file is present.

		![Verify HQL script file is in place](Images/ex2task2-explorer-scripts.png?raw=true "Verify HQL script file is in place")

		_Verify HQL script file is in place_


<a name="Exercise3"></a>
### Exercise 3: **Orchestrating Azure Data Factory workflow** ###

**Azure Data Factory** enables you to compose data movement and data processing tasks as a data driven workflow. In this exercise, you'll learn how to build your first pipeline that uses HDInsight to transform and analyze the logs from the Parts Unlimited web site.

<a name="Ex3Task1"></a>
#### Task 1 - Create the pipeline that will run the Hive activity ####

**Pipeline** is a logical grouping of Activities. They are used to group activities into a unit that performs a task.

**Activities** define the actions to perform on your data. Each activity takes zero or more datasets as inputs and produces one or more datasets as output. An activity is a unit of orchestration in Azure Data Factory.

An _HDInsight Hive activity_ executes Hive queries on a _HDInsight_ cluster.

In this task, you'll create the pipeline to generate the stats output using a _HDInsight Hive activity_.

1. Go back to the Azure Portal and open the **Data Factory**.

1. In the **Author and Deploy** blade of the Data Factory, click **New pipeline** button on the toolbar (click ellipsis button if you don't see the New pipeline button).

1. Change the **name** to "JsonLogsToTabularPipeline" and set the description to "Create tabular data using Hive".

1. Set the **start** date to be 3 days before the current date (for instance: "2016-04-29T00:00:00Z").

1. Set the **end** date to be tomorrow.

1. Add a Hive activity to run the "logstocsv.hql" script located in the storage at "partsunlimited\Scripts\logtostats.hql" and pass the input (logs folder) and output (CSV folder) paths as parameters for the script. Make sure to replace the **<****StorageAccountName****>** placeholder with the storage account name:

	````JavaScript
	"activities": [
		{
			"name": "LogsToCsvHiveActivity",
			"type": "HDInsightHive",
			"linkedServiceName": "HDInsightLinkedService",
			"typeProperties": {
				"scriptPath": "partsunlimited\\Scripts\\logstocsv.hql",
				"scriptLinkedService": "AzureStorageLinkedService",
				  "defines": {
						"inputjson": "$$Text.Format('wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/Logs/{0:yyyy}/{0:MM}/{0:dd}/', SliceStart)",
						"outputcsv": "$$Text.Format('wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/Stats/{0:yyyy}/{0:MM}/{0:dd}/', SliceStart)"
				  }
			},
			"inputs": [
				{ "name": "LogJsonFromBlob" }
			],
			"outputs": [
				{ "name": "LogCsvFromBlob" }
			],
			"scheduler": {
				"frequency": "Day",
				"interval": 1
			}
		}
	],
	````

1. Click **Deploy** on the toolbar to create and deploy the pipeline.

<a name="Ex3Task2"></a>
#### Task 2 - Create a new pipeline to move the HDI output to SQL Data Warehouse ####
In this task, you'll create a new pipeline to move the Hive activity output (stored in a blob) to the SQL Data Warehouse database so it can be consumed using Power BI in the next exercise.

1. In the **Author and Deploy** blade of the Data Factory, click **New pipeline** button on the toolbar (click ellipsis button if you don't see the New pipeline button).

1. Change the **name** to "LogsToDWPipeline" and set the description to "Move blob data to SQL Data Warehouse".

1. Set the **start** date to be 2 days before the current date.

1. Set the **end** date to be tomorrow.

1. Add a Move activity to move the generated tabular blobs to the SQL Data Warehouse. Make sure to replace the **<****StorageAccountName****>** placeholder with the storage account name:

	````JavaScript
	"activities": [
		{
			"type": "Copy",
			"name": "LogsToDWActivity",
			"typeProperties": {
				"source": {
					"type": "BlobSource"
				},
				"sink": {
					"type": "SqlDWSink"
				}
			},
			"inputs": [
				{
					"name": "LogCsvFromBlob"
				}
			],
			"outputs": [
				{
					"name": "LogsSqlDWOutput"
				}
			],
			"scheduler": {
				"frequency": "Day",
				"interval": 1
			}
		}
	],
	````

1. Click **Deploy** on the toolbar to create and deploy the pipeline.

1. In the Data Factory blade, click **Diagram**, you'll see how the parts were connected and the details.

	![Updated diagram](Images/ex3task2-diagram.png?raw=true "Updated diagram")

	_Updated diagram_

    You can zoom in, zoom out, zoom to 100%, zoom to fit, automatically position pipelines and tables, and show lineage information (highlights upstream and downstream items of selected items). You can double-click an object (input/output table or pipeline) to see its properties.

<a name="Ex3Task3"></a>
#### Task 3 - Monitoring the Data Factory ####

With the Monitoring and Managing app you can easily monitor and manage your data factory pipelines, this app:

- Presents information in a simple way and provide powerful insights into your pipelines.
- Improves navigation and debug ability of pipelines in the data factory by enabling different views of the data factory.
- Enables batch actions in data factory to improve developer productivity.
- Allows you to create and enable email notifications on various events with simple steps (ex. failure).

In this task, you'll explore the features of the Monitoring App using the data factory you created.

1. In the _Summary_ section of the Data Factory blade, click on **Monitoring App** to launch the monitoring app in a new tab.

	![Open Monitoring App](Images/ex3task3-open-monitoring-app.png?raw=true "Open Monitoring App")

	_Open Monitoring App_

1. Review the Data Factoy view with full status details. You can easily view the active pipelines and the slices/widnows status.

	![Monitoring App](Images/ex3task3-monitoring-app.png?raw=true "Monitoring App")

	_Monitoring App_

1. Click on the glasses icon at the left pane. Notice you can switch the system view to see the activity windows filtered by state.

	![Monitoring Views](Images/ex3task3-monitoring-views.png?raw=true "Monitoring Views")

	_Monitoring Views_

	This section enables pre-canned list of views for the data factories with the Diagram view as the center of the universe viz. recent activity windows, failed activity windows, in-progress activity windows.

1. Double-click on the top margin of the **LogCsvFromBlob** dataset. Notice a calendar shows up with the activity windows highligted. Click on the calendar' dates to see how it highlights the corresponding activity windows.

	![Activity Calendar](Images/ex3task3-activity-calendar.png?raw=true "Activity Calendar")

	_Activity Calendar_

	You have the ability to view the data factory in terms of activity windows in pipelines. Makes it easier to monitor, browse activity windows with fewer clicks. Showcases activity run details with all the attempts corresponding to each activity window. Allows you to view activity executions in calendar view, or a hover over a dataset.

1. You can also filter and sort the activity windows in an intuitive way by clicking on the column headers.

	![Sorting and filtering activity windows](Images/ex3task3-sort-filter.png?raw=true "Sorting and filtering activity windows")

	_Sorting and filtering activity windows_

1. You can stop, pause and resume the pipelines using the buttons below each pipeline.

	![Pause pipeline](Images/ex3task3-pause.png?raw=true "Pause pipeline")

	_Pause pipeline_

	If you use the **Resource Explorer** on the left pane, you can select multiple pipelines to execute the puase/resume action.

There are more features to continue exploring in the Monitoring App. You can also configure _alerts_ to create email notifications on various events (failure, success, etc.), re-run failed activities and even configure the app layout by dragging the panes.

<a name="Ex3Task4"></a>
#### Task 4 - Executing Stored Procedure in SQL Data Warehouse ####
In this task you will add a new pipeline to run the stored procedure that populates the ProductStats table in the SQL Data Warehouse.

You can use the SQL Server Stored Procedure activity in a Data Factory pipeline to invoke a stored procedure.

1. Create a new dataset for the ProductStats table that will be the output of the new pipeline.
 1. In the Editor for the Data Factory, click **New dataset** button on the toolbar and click **Azure SQL Data Warehouse** from the drop down menu.
 1. Set the **name** to "StatsSqlDWOuput" and the **linkedServiceName** to "AzureSqlDWLinkedService"
 1. Set the **tableName** property to **dbo.ProductStats**:

		````JavaScript
		"typeProperties": {
			"tableName": "dbo.ProductLogs"
		},
		````

 1. Daily availability:

		````JavaScript
		"availability": {
			"frequency": "Day",
			"interval": 1
		}
		````

 1. Make sure the final _LogsSqlDWOutput_ dataset looks like the following snippet:
 
		````JavaScript
		{
			 "name": "StatsSqlDWOutput",
			 "properties": {
				  "published": false,
				  "type": "AzureSqlDWTable",
				  "linkedServiceName": "AzureSqlDWLinkedService",
				  "typeProperties": {
						"tableName": "dbo.ProductStats"
				  },
				  "availability": {
						"frequency": "Day",
						"interval": 1
				  }
			 }
		}
		````

 1. Click **Deploy** on the toolbar to create and deploy the new dataset.

1. Now, create the pipeline to run the **sp_populate_stats** stored procedure.
 1. In the **Author and Deploy** blade of the Data Factory, click **New pipeline** button on the toolbar (click ellipsis button if you don't see the New pipeline button).
 1. Change the **name** to "PopulateProductStatsPipeline" and set the description to "Run stored procedure to populate product stats".
 1. Set the **start** date to be 2 days before the current date.
 1. Set the **end** date to be tomorrow.
 1. Add a _SqlServerStoredProcedure_ activity to to run the **sp_populate_stats** stored procedure from the SQL Data Warehouse. Make sure to replace the **<****StorageAccountName****>** placeholder with the storage account name:

		````JavaScript
		"activities": [
			{
				 "type": "SqlServerStoredProcedure",
				 "typeProperties": {
					  "storedProcedureName": "sp_populate_stats"
				 },
				 "outputs": [
					  {
							"name": "StatsSqlDWOutput"
					  }
				 ],
				 "scheduler": {
					  "frequency": "Day",
					  "interval": 1
				 },
				 "name": "SprocActivitySample"
			}
		],
		````

 1. Click **Deploy** on the toolbar to create and deploy the pipeline.

 1. In the Data Factory blade, click **Diagram**, you'll see the new pipeline.

	![New pipeline to run stored procedure](Images/ex3task4-sp-pipeline.png?raw=true "New pipeline to run stored procedure")

	_New pipeline to run stored procedure_

1. Make sure the output dataset slices are ran so the ProductStats table is populated and you can proceed with the next exercise to consume the just populated table.

	![New pipeline output slices](Images/ex3task4-sp-pipeline-slices.png?raw=true "New pipeline output slices")

	_New pipeline output slices_

1. Now that the data was processed by the stored procedure and populated the _ProductStats_ table, you can now pause the Data Warehouse. Navigate to the SQL Data Warehouse blade and click **Pause**.

	![Pause SQL Data Warehouse](Images/ex3task4-pause-dw.png?raw=true "Pause SQL Data Warehouse")

	_Pause SQL Data Warehouse_

1. Confirm you want to pause the data warehouse.

	Unique to SQL Data Warehouse is the ability to _pause_ and _resume_ compute on demand. If the team will not be using the Data Warehouse instance for a period of time, like nights, weekends, certain holidays or for any other reason, you can pause the Data Warehouse instance for that period of time and pick up where you left off when you return. 

	> **Note:** Since storage is separate from compute, your storage is unaffected by pause and you can continue having access to it. For instance, using Power BI as you will do in next exercise.

<a name="Exercise4"></a>
### Exercise 4: Using the data in the warehouse to generate Power BI visualizations ###

Power BI and SQL Data Warehouse allow you to quickly and easily create data visualizations, even at terabyte scale, or contains both relational and non-relational aspects.

<a name="Ex4Task1"></a>
#### Task 1 - Open the Data Warehouse in PowerBI ####
The easiest way to move between your _SQL Data Warehouse_ and _Power BI_ is with the **Open in Power BI** button. This button allows you to seamlessly begin creating new dashboards in Power BI.

In this task, you'll open Power BI and connect to the SQL Data Warehouse from the Azure Portal.

1. Navigate to the SQL Data Warehouse instance in the Azure Portal.

1. Click the **Open in Power BI** button.

	![Open in Power BI](Images/ex4task1-open-powerbi.png?raw=true "Open in Power BI")

	_Open in Power BI_

1. Sign in using an organization account.

1. You will be directed to the SQL Data Warehouse connection page, with the information from your SQL Data Warehouse pre-populated.

	![Connect to Azure SQL Data Warehouse](Images/ex4task1-connect-azure.png?raw=true "Connect to Azure SQL Data Warehouse")

	_Connect to Azure SQL Data Warehouse_

1. Click **Next** and enter your SQL Data Warehouse username and password (it should be **P@ssword123** if you did not change the script), then click **Sign in**.

	![Server credentials](Images/ex4task1-server-credentials.png?raw=true "Server credentials")

	_Server credentials_

1. Once the connection is made, the partsunlimited dataset will appear on the left blade.

<a name="Ex4Task2"></a>
#### Task 2 - Create Reports in Power BI ####
A **Power BI report** is one or more pages of visualizations (charts and graphs). A **dashboard** is a single canvas that contains one or more tiles that you can use to connect to multiple datasets and display visualizations from many different reports.

In this task, you'll create a report based on the SQL Data Warehouse dataset you created.

1. Click the **Azure SQL Data Warehouse** tile from your dashboard to create a new report.

	![Azure SQL Data Warehouse tile](Images/ex4task2-dw-tile.png?raw=true "Azure SQL Data Warehouse tile")

	_Azure SQL Data Warehouse tile_

	In the navigation pane, your reports are listed under the **Reports** heading. Each listed report represents one or more pages of visualizations based on one or more of the underlying datasets.

1. In the _Fields_ pane, check **category** and **views** from the **ProductStats** table.

	![Product fields](Images/ex4task2-fields.png?raw=true "Product fields")

	_Product fields_

1. You will see a table showing the product categories with the total views.

	![Categories view table](Images/ex4task2-product-views-table.png?raw=true "Categories view table")

	_Categories view table_

1. Select the **Stacked columns chart** in the _Visualizations_ pane.

	![Stacked columns visualization](Images/ex4task2-chart-visualization.png?raw=true "Stacked columns visualization")

	_Stacked columns chart visualization_

	![Categories view chart](Images/ex4task2-product-views-chart.png?raw=true "Categories view chart")

	_Categories view chart_

	> **Note:** Chart values will differ since the values are randomly generated when creating the input data files in the setup script.

1. Now lets create a new chart, click the canvas, outside the table, and check the fields **title** and **views**.

1. Select the **Pie chart** visualization.

	![Products pie chart](Images/ex4task2-categories-pie.png?raw=true "Products pie chart")

	_Products pie chart_

    You can continue creating charts using any combination of fields and also apply filters and other operations. Then, you can change the report layout, add labels, shapes and other visual objects. To learn more about Power BI reports visit [Reports in Power BI](https://powerbi.microsoft.com/en-us/documentation/powerbi-service-reports/)

1. When you're done with your report, go to **File** > **Save** and enter a name for your report.

	![Save report](Images/ex4task2-save-report.png?raw=true "Save report")

	_Save report_

---

<a name="Summary"></a>
## Summary ##

By completing this module, you should have:

- Created an **Azure Data Factory**
- Created a **Hive** query to do analytics on your data
- Orchestrated an **Azure Data Factory** workflow
- Moved Data from **HDInsight** to **SQL Data Warehouse**
- Used the data in **SQL Data Warehouse** to generate **Power BI** visualizations
