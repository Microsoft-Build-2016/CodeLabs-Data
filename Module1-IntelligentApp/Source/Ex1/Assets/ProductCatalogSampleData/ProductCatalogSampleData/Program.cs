namespace ProductCatalogSampleData
{
    using System;
    using PartsUnlimited.Models;

    public class Program
    {
       static string documentDbName = "PartsUnlimited";
        //static string documentDbEndpoint = "";
        //static string documentDbAuthKey = "";
        static string documentDbCollection = "Products";

        static void Main(string[] args)
        {
            string documentDbEndpoint = args[0];
            string documentDbAuthKey = args[1];

            Console.WriteLine("Initializing DocumentDB");

            DocumentDBTestData.InitializePartsUnlimitedDatabaseAsync(documentDbName, documentDbEndpoint, documentDbAuthKey, documentDbCollection).Wait();

            Console.WriteLine("DocumentDB data is ready");
            Console.WriteLine("Press Any key to exit...");
            Console.ReadLine();
        }        
    }
}
