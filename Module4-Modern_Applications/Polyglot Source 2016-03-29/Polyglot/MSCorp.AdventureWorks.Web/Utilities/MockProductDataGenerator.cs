using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Common;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Search;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Web.Utilities
{
    public static class MockProductDataGenerator
    {

        private const string RuggedCategory = "Rugged";
        private const string SmoothCategory = "Smooth";
        private const string GearCategory = "Gear";

        private const string BikeType = "Bikes";
        private const string BikeFrameType = "Frames";
        private const string ShoesType = "Shoes";
        private const string HelmetType = "Helmets";
        private const string AccessoryType = "Accessories";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "MSCorp.AdventureWorks.Core.Domain.ProductProperty.#ctor(System.String,System.String)",
            Justification = "Generating demo data")]
        public static IEnumerable<PrimaryIndexEntry> GeneratorProducts()
        {
            IList<Product> products = new List<Product>();
            Product product = null;
            
            ProductSize bikeSize44 = new ProductSize("44", 44);
            ProductSize bikeSize48 = new ProductSize("48", 48);
            ProductSize bikeSize52 = new ProductSize("52", 52);
            ProductSize bikeSize56 = new ProductSize("56", 56);

            ProductSize smallSize = new ProductSize("S", 1);
            ProductSize mediumSize = new ProductSize("M", 2);
            ProductSize largeSize = new ProductSize("L", 3);

            JObject standardBikeProperties = JObject.Parse("{\"SizeUnit\":\"cm\"}");
            JObject emptyProperties = JObject.Parse("{}");

            /* Rugged */
            product = new Product("BK-M82S", RuggedCategory, BikeType,
                "Mountain-100 Silver", "For ultimate trail pounding", "AdventureWorks",
                new Money(3399.99m, Currency.Default), new Money(3399.99m, Currency.Default), new Percentage(0), Color.Silver,
                new[] { bikeSize44, bikeSize48, bikeSize52, bikeSize56 },
                standardBikeProperties, Enumerable.Empty<Identifier>());
            products.Add(product);

            product = new Product("BK-M82B", RuggedCategory, BikeType,
                "Mountain-100 Black", "For ultimate trail pounding", "AdventureWorks",
                new Money(3374.99m, Currency.Default), new Money(3374.99m, Currency.Default), new Percentage(0), Color.Black,
                new[] { bikeSize44, bikeSize48, bikeSize52, bikeSize56 },
                standardBikeProperties, Enumerable.Empty<Identifier>());
            products.Add(product);

            product = new Product("BK-M68S", RuggedCategory, BikeType,
                "Mountain-200 Silver", "For ultimate trail pounding", "AdventureWorks",
                new Money(2319.99m, Currency.Default), new Money(2319.99m, Currency.Default), new Percentage(0), Color.Silver,
                new[] { bikeSize44, bikeSize48 },
                standardBikeProperties, Enumerable.Empty<Identifier>());
            products.Add(product);

            product = new Product("BK-M47B", RuggedCategory, BikeType,
                "Mountain-300 Black", "For ultimate trail pounding", "AdventureWorks",
                new Money(1079.99m, Currency.Default), new Money(1079.99m, Currency.Default), new Percentage(0), Color.Black,
                new[] { bikeSize44, bikeSize48 },
                standardBikeProperties, Enumerable.Empty<Identifier>());
            products.Add(product);




            /* Smooth */
            product = new Product("BK-R93R", SmoothCategory, BikeType,
                "Road-150 Red", "For ultimate trail pounding", "AdventureWorks",
                new Money(3578.27m, Currency.Default), new Money(3578.27m, Currency.Default), new Percentage(0), Color.Red,
                new[] { bikeSize44, bikeSize48, bikeSize52, bikeSize56 },
                standardBikeProperties, Enumerable.Empty<Identifier>());
            products.Add(product);

            product = new Product("BK-R68R", SmoothCategory, BikeType,
                "Road-450 Red", "For ultimate trail pounding", "AdventureWorks",
                new Money(147.99m, Currency.Default), new Money(147.99m, Currency.Default), new Percentage(0), Color.Red,
                new[] { bikeSize44, bikeSize48, bikeSize52, bikeSize56 },
                standardBikeProperties, Enumerable.Empty<Identifier>());
            products.Add(product);

            product = new Product("BK-R50B", SmoothCategory, BikeType,
                "Road-650 Black", "For ultimate trail pounding", "AdventureWorks",
                new Money(782.99m, Currency.Default), new Money(782.99m, Currency.Default), new Percentage(0), Color.Black,
                new[] { bikeSize44, bikeSize48, bikeSize52, bikeSize56 },
                standardBikeProperties, Enumerable.Empty<Identifier>());
            products.Add(product);

            product = new Product("BK-T79Y", SmoothCategory, BikeType,
                "Touring-1000 Yellow", "For ultimate trail pounding", "AdventureWorks",
                new Money(2384.07m, Currency.Default), new Money(2384.07m, Currency.Default), new Percentage(0), Color.Yellow,
                new[] { bikeSize44, bikeSize48, bikeSize52, bikeSize56 },
                standardBikeProperties, Enumerable.Empty<Identifier>());
            products.Add(product);






            /* Gear */
            product = new Product("HL-U509-R", GearCategory, HelmetType,
                "Sport-100 Helmet, Red", "For ultimate trail pounding", "AdventureWorks",
                new Money(34.99m, Currency.Default), new Money(34.99m, Currency.Default), new Percentage(0), Color.Red, new[] { smallSize, mediumSize, largeSize },
                        emptyProperties, Enumerable.Empty<Identifier>());
            products.Add(product);

            product = new Product("LT-T990", GearCategory, AccessoryType,
                "Taillights - Battery-Powered", "Be safe, be seen", "AdventureWorks",
                new Money(13.99m, Currency.Default), new Money(13.99m, Currency.Default), new Percentage(0), Color.Black, new[] { smallSize },
                        emptyProperties, Enumerable.Empty<Identifier>());
            products.Add(product);

            product = new Product("BC-M005", GearCategory, AccessoryType,
                "Mountain Bottle Cage", "Hold your hydration", "AdventureWorks",
                new Money(125.99m, Currency.Default), new Money(125.99m, Currency.Default), new Percentage(0), Color.Black, new[] { smallSize },
                        emptyProperties, Enumerable.Empty<Identifier>());
            products.Add(product);

            product = new Product("WB-H098", GearCategory, AccessoryType,
                "Water Bottle - 30 oz.", "Urban adventurer tracks", "AdventureWorks",
                new Money(4.99m, Currency.Default), new Money(4.99m, Currency.Default), new Percentage(0), Color.Blue, new[] { smallSize },
                        emptyProperties, Enumerable.Empty<Identifier>());
            products.Add(product);




            // auto populate any empty ids
            foreach (Product item in products.Where(i => string.IsNullOrEmpty(i.Id)))
            {
                item.Id = Guid.NewGuid().ToString();
            }

            IEnumerable<PrimaryIndexEntry> indexEntries = products.Select(product1 => PrimarySearchIndexBuilder.Build(product1)).ToArray();
            return indexEntries;
        }
    }
}