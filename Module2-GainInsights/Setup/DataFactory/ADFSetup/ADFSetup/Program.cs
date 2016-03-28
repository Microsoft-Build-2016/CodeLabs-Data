using Microsoft.Azure;
using Microsoft.Azure.Management.DataFactories;
using Microsoft.Azure.Management.DataFactories.Models;
using Microsoft.Azure.Management.DataFactories.Common.Models;
using Microsoft.Azure.Management.DataFactories.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADFSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            string subscriptionID = "5a53c8bc-65a8-4d8e-9755-52f6a53c545f";
            string resourceGroupName = "DataCodeLab2";
            string dataFactoryName = "jesusadf";
            string prefixNumber = "";
            string part = "";

            //////*******Put in default values here ********////////////
            string SQLDWServerName = "";
            string SQLDWDatabaseName = "";
            string hdinsightName = "";

            int DaysToRunTheFactory = -4;
            Uri resourceManagerUri = new Uri(ConfigurationManager.AppSettings["ResourceManagerEndpoint"]);
            var StartTime = new DateTime(DateTime.UtcNow.AddDays(DaysToRunTheFactory).Year, DateTime.UtcNow.AddDays(DaysToRunTheFactory).Month, DateTime.UtcNow.AddDays(DaysToRunTheFactory).Day, 0, 0, 0, DateTimeKind.Utc);
            var EndTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);

            TokenCloudCredentials aadTokenCredentials =
                new TokenCloudCredentials(
                        subscriptionID,
                        GetAuthorizationHeader());


            DataFactoryManagementClient client = new DataFactoryManagementClient(aadTokenCredentials, resourceManagerUri);


            if(part.Equals("Part1") || args[0].Equals("Part1"))
            {
                /**Creating Storage Linked Service**/

                client.LinkedServices.CreateOrUpdate(resourceGroupName, dataFactoryName,
                   new LinkedServiceCreateOrUpdateParameters()
                   {
                       LinkedService = new LinkedService()
                       {
                           Name = "HDInsightLinkedService",
                           Properties = new LinkedServiceProperties
                           (
                               new HDInsightLinkedService("https://buildlab-data" + prefixNumber + ".azurehdinsight.net", "admin", "P@ssword123")
                           )
                       }
                   }
               );

            }

            /*SQL DW Linked Service*/
            Console.WriteLine("Creating DW linked service");
            client.LinkedServices.CreateOrUpdate(resourceGroupName, dataFactoryName,
                new LinkedServiceCreateOrUpdateParameters()
                {
                    LinkedService = new LinkedService()
                    {
                        Name = "SqlDWLinkedService",
                        Properties = new LinkedServiceProperties
                        (
                            new AzureSqlDataWarehouseLinkedService("Data Source=tcp:" + SQLDWServerName + prefixNumber + ".database.windows.net,1433;Initial Catalog=SQLDWDatabaseName;User ID=dwadmin@" + SQLDWServerName + prefixNumber + ";Password=P@ssword123;Integrated Security=False;Encrypt=True;Connect Timeout=30")
                        )
                    }
                }
            );



        }

        public static string GetAuthorizationHeader()
        {
            AuthenticationResult result = null;
            var thread = new Thread(() =>
            {
                try
                {
                    var context = new AuthenticationContext(ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"] + ConfigurationManager.AppSettings["ActiveDirectoryTenantId"]);

                    result = context.AcquireToken(
                        resource: ConfigurationManager.AppSettings["WindowsManagementUri"],
                        clientId: ConfigurationManager.AppSettings["AdfClientId"],
                        redirectUri: new Uri(ConfigurationManager.AppSettings["RedirectUri"]),
                        promptBehavior: PromptBehavior.Always);
                }
                catch (Exception threadEx)
                {
                    Console.WriteLine(threadEx.Message);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AcquireTokenThread";
            thread.Start();
            thread.Join();

            if (result != null)
            {
                return result.AccessToken;
            }

            throw new InvalidOperationException("Failed to acquire token");
        }
    }
}
