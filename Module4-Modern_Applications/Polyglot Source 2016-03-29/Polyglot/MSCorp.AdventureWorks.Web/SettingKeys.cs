using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCorp.AdventureWorks.Web
{
    public static class SettingKeys
    {
        public const string OrderDatabaseConnectionString = "OrderDatabaseConnectionString";
        public const string AzureImageBlobConnectionString = "AzureImageBlobConnectionString";
        public const string AzureTableBlobConnectionString = "AzureTableBlobConnectionString";
        public const string SearchPrimaryIndexName = "SearchPrimaryIndexName";
        public const string DocumentDbDatabase = "DocumentDbDatabase";
        public const string DocumentDbCollectionSuffix = "DocumentDbCollectionSuffix";
        public const string SearchReviewIndexName = "SearchReviewIndexName";
        public const string AzureImageBlobContainerName = "AzureImageBlobContainerName";
        public const string SearchApiKey = "SearchApiKey";
        public const string SearchServiceUrl = "SearchServiceUrl";
        public const string DocumentDbUri = "DocumentDbUri";
        public const string DocumentDbAuthKey = "DocumentDbAuthKey";
    }
}
