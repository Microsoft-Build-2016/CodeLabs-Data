using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Common;
using Microsoft.Practices.Unity;
using MSCorp.AdventureWorks.Core.Configuration;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;
using MSCorp.AdventureWorks.Web.Utilities;
using Unity.Mvc5;

namespace MSCorp.AdventureWorks.Web
{
    public static class UnityConfig
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is kept and used outside of this scope")]
        public static IUnityContainer RegisterComponents(DbConfig dbConfig)
        {
            Argument.CheckIfNull(dbConfig, "dbConfig");

            UnityContainer container = null;
            UnityContainer containerToReturn = null;

            try
            {
                container = new UnityContainer();

                // register all your components with the container here
                // it is NOT necessary to register your controllers

                container.RegisterInstance<IAppSettings>(dbConfig.AppSettings);

                // e.g. container.RegisterType<ITestService, TestService>();

                AzureSearchClient<PrimaryIndexEntry> searchClient = new AzureSearchClient<PrimaryIndexEntry>(
                    SettingLoader.Load("SearchServiceUrl").ToUri(), SettingLoader.Load("SearchPrimaryIndexName").Value,
                    SettingLoader.Load("SearchApiKey").Value, container.Resolve<IAppSettings>());

                AzureSearchClient <ReviewIndexEntry> reviewSearchClient = new AzureSearchClient<ReviewIndexEntry>(
                    SettingLoader.Load("SearchServiceUrl").ToUri(), SettingLoader.Load("SearchReviewIndexName").Value,
                     SettingLoader.Load("SearchApiKey").Value, container.Resolve<IAppSettings>());

                container.RegisterInstance(searchClient);
                container.RegisterInstance(reviewSearchClient);
                container.RegisterInstance(dbConfig.DiscoveryRepository);
                container.RegisterInstance<IProductRepository>(dbConfig.ProductRepository);
                container.RegisterInstance<IProductReviewRepository>(dbConfig.ProductReviewRepository);
                container.RegisterInstance<ICustomerRepository>(dbConfig.CustomerRepository);
                container.RegisterInstance<ICurrencyRepository>(dbConfig.CurrencyRepository);
                container.RegisterInstance<IOrderSummaryRepository>(dbConfig.OrderSummaryRepository);
                container.RegisterInstance<ICustomerCartRepository>(dbConfig.CustomerCartRepository);
                container.RegisterInstance<IUnitOfWorkFactory>(dbConfig.UnitOfWorkFactory);
                container.RegisterInstance(dbConfig.SqlUnitOfWork);
                container.RegisterInstance(dbConfig);   //  Normally wouldn't include this, but it is used in DemoSetupController

                // set up resolver
                UnityDependencyResolver resolver = new UnityDependencyResolver(container);
                DependencyResolver.SetResolver(resolver);

                //  set up specific resolver for WebApi
                WebApiDependencyResolver apiResolver = new WebApiDependencyResolver(container);
                WebApiConfig.SetResolver(apiResolver);

                containerToReturn = container;
                container = null;
            }
            finally
            {
                if (container != null)
                {
                    // if we got here then something failed during init, so correctly dispose the broken container
                    container.Dispose();
                }
            }


            return containerToReturn;
        }

    }
}