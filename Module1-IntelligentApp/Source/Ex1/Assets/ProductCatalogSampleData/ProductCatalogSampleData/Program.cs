namespace ProductCatalogSampleData
{
    using System;
    using PartsUnlimited.Models;

    public class Program
    {
        static string documentDbName = "PartsUnlimited";
        static string documentDbEndpoint = "https://datamodule1.documents.azure.com:443/";
        static string documentDbAuthKey = "Xfa0s13Fyq6/FXuZLUyi5yS5c0JB9KD58Ech7Vl/q4bU0L7jhmOOFHipmLDFzYE5r4QNkw09+IS9SN1xLpCiJQ==";
        static string documentDbCollection = "Products";

        static void Main(string[] args)
        {
            Console.WriteLine("Initializing DocumentDB");

            DocumentDBTestData.InitializePartsUnlimitedDatabaseAsync(documentDbName, documentDbEndpoint, documentDbAuthKey, documentDbCollection).Wait();

            Console.WriteLine("DocumentDB data is ready");
            Console.ReadLine();
        }        
    }
}
