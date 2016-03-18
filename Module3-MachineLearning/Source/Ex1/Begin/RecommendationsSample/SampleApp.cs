namespace RecommendationsSampleApp
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Sample app to show how to build a Recommendations model using the API 
    /// (http://gallery.cortanaanalytics.com/MachineLearningAPI/Recommendations-2) 
    /// The application will create a model container, add catalog and usage data, 
    /// trigger a recommendation model build and monitor the build execution
    /// 
    /// The application also shows the usage of updating model information. 
    /// 
    /// The demo assumes you haven't created a model previously. 
    /// </summary>
    public class RecommendationsSampleApp
    {
        private static string accountEmail = "[YOUR ACCOUNT EMAIL]";
        private static string accountKey = "[YOUR ACCOUNT KEY]";
        private static string modelName = "demo_cars";

        public static void Main(string[] args)
        {
            // Get your account key from https://datamarket.azure.com/account 
            // If you signed up for the service using the Azure Management Portal, you can get it there.

            // Note: if you run the app flow consecutively you need to change the model name, otherwise the invocation will fail.
            var modelTrainer = new RecommendationModelCreator(accountEmail, accountKey);

            //Create a model container
            Console.WriteLine("Creating a new model container {0}...", modelName);
            var modelId = modelTrainer.CreateModel(modelName);

            Console.WriteLine("Model '{0}' created with ID: {1}", modelName, modelId);

            // Import data to the container            
            var resourcesDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

            Console.WriteLine("Importing catalog...");
            modelTrainer.UploadCatalog(modelId, Path.Combine(resourcesDir, "catalog_small.txt"));
                       
            Console.WriteLine("Importing usage data...");
            modelTrainer.UploadUsage(modelId, Path.Combine(resourcesDir, "usage_data.txt"));
                        
            // Trigger a build to produce a recommendation model.
            Console.WriteLine("Triggering build for model '{0}'", modelId);
            var buildId = modelTrainer.Build(modelId, "build of " + DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

            Console.WriteLine("Monitoring build '{0}'", buildId);
            // monitor the current triggered build
            var status = BuildStatus.Create;
            var monitor = true;

            while (monitor)
            {
                status = modelTrainer.GetBuildStatus(modelId, buildId);

                Console.WriteLine("  Build '{0}' model '{1}': status {2}", buildId, modelId, status);
                if (status != BuildStatus.Error && status != BuildStatus.Cancelled && status != BuildStatus.Success)
                {
                    Console.WriteLine("  --> will check in 5 seconds...");
                    Thread.Sleep(5000);
                }
                else
                {
                    monitor = false;
                }
            }

            Console.WriteLine("Build {0} ended with status {1}", buildId, status);

            if (status != BuildStatus.Success)
            {
                Console.WriteLine("Build {0} did not end successfully, the sample app will stop here.", buildId);
                Console.WriteLine("Press any key to end");
                Console.ReadKey();
                return;
            }

            // The below api is more meaningful when you want to give a certain build id to be an active build.
            // currently this app has a single build which is already active.
            Console.WriteLine("Updating model description to 'PartsUnlimited model' and set active build");
            modelTrainer.UpdateModel(modelId, "PartsUnlimited model", buildId);
            
            Console.WriteLine("Press any key to end");
            Console.ReadKey();
        }
    }
}
