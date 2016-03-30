using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Import
{
    public class ScenarioImporter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public ImportScenario ImportFromZip(Stream stream)
        {
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ReadOnlyCollection<ZipArchiveEntry> allEntries = archive.Entries;
                ZipArchiveEntry scenarioEntry = allEntries.SingleOrDefault(entry => entry.Name.Equals("scenario.json", StringComparison.OrdinalIgnoreCase));
                IEnumerable<ZipArchiveEntry> imageEntries = allEntries.Where(entry => entry.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || entry.Name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase));
                ImportScenario scenario = ParseScenarioEntry(scenarioEntry);
                AttachImageAssets(scenario, imageEntries);

                return scenario;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static ImportScenario ParseScenarioEntry(ZipArchiveEntry scenarioEntry)
        {
            if (scenarioEntry == null)
            {
                throw new FileLoadException("Import package must contain a file called \"scenario.json\" holding master data.");
            }

            using (Stream scenarioStream = scenarioEntry.Open())
            using (StreamReader reader = new StreamReader(scenarioStream))
            {
                string contents = reader.ReadToEnd();
                ImportScenario scenario = JsonConvert.DeserializeObject<ImportScenario>(contents);
                return scenario;
            }
        }

        private static void AttachImageAssets(ImportScenario scenario, IEnumerable<ZipArchiveEntry> imageEntries)
        {
            Dictionary<string, ZipArchiveEntry> images = imageEntries.ToDictionary(entry => entry.FullName.ToUpperInvariant(), entry => entry);
            Dictionary<string, Image> productImageAssets = new Dictionary<string, Image>();
            
            foreach (ProductImage productImage in scenario.ProductImages)
            {
                string key = productImage.ImagePath.ToUpperInvariant();
                if (productImageAssets.ContainsKey(key))
                {
                    productImage.Image = productImageAssets[key];
                    continue;
                }

                if (images.ContainsKey(key))
                {
                    using (Stream stream = images[key].Open())
                    {
                        Image imageAsset = new Bitmap(stream);
                        productImageAssets.Add(key, imageAsset);
                        productImage.Image = imageAsset;
                    }
                }
            }
        }

    }
}