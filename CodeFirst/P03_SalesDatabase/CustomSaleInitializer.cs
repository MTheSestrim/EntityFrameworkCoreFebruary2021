using P03_SalesDatabase.Data;
using P03_SalesDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_SalesDatabase
{
    public static class CustomSaleInitializer
    {
        public static void SeedDatabase(SalesContext context)
        {
            Random randGenerator = new Random();

            var products = new List<Product>();
            var customers = new List<Customer>();
            var stores = new List<Store>();
            var sales = new List<Sale>();

            products = SeedProducts(randGenerator);

            customers = SeedCustomers(randGenerator);

            stores = SeedStores(randGenerator);

            sales = SeedSales(randGenerator, products, customers, stores);

            context.AddRange(products);
            context.AddRange(customers);
            context.AddRange(stores);
            context.AddRange(sales);
            context.SaveChanges();
        }

        private static List<Sale> SeedSales(Random randGenerator,
            List<Product> products, List<Customer> customers, List<Store> stores)
        {
            var sales = new List<Sale>();

            //100 Sales

            for (int i = 0; i < 100; i++)
            {
                var randProduct = products[randGenerator.Next(0, products.Count)];
                var randCustomer = customers[randGenerator.Next(0, customers.Count)];
                var randStore = stores[randGenerator.Next(0, stores.Count)];

                var currentSale = new Sale
                {
                    Product = randProduct,
                    Customer = randCustomer,
                    Store = randStore,
                };

                sales.Add(currentSale);
            }

            return sales;
        }

        private static List<Store> SeedStores(Random randGenerator)
        {
            var stores = new List<Store>();

            string[] storeNames = new string[]
            {
                    "Kaufland",
                    "Lidl",
                    "Billa",
                    "Mallbg",
                    "Muziker"
            };

            //10 stores

            for (int i = 0; i < 10; i++)
            {

                var store = new Store
                {
                    Name = storeNames[randGenerator.Next(0, storeNames.Length)]
                };

                stores.Add(store);
            }

            return stores;
        }

        private static List<Customer> SeedCustomers(Random randGenerator)
        {
            var customers = new List<Customer>();

            string[] customerFirstNames = new string[]
            {
                "Kurt",
                "Иван",
                "Крис",
                "Dave",
                "Alan",
                "Пешо",
                "Гошо",
                "Димитър",
                "Нели",
                "Дамян",
                "Tom",
                "John",
                "Pat"
            };

            string[] customerLastNames = new string[]
            {
                "Иванов",
                "Милев",
                "Тодоров",
                "Антонов",
                "Johnson",
                "Cobain",
                "Novoselic",
                "Grohl",
                "Smear",
                "Vale",
                "Armstrong",
                "Mathers",
                "Lamar",
                "Clinton",
                "De Santa",
                "Philips",
                "Miller",
                "Jackson"
            };

            string[] emailUsers = new string[]
            {
                "ivan",
                "alan",
                "floyd",
                "jimmy",
                "sim30n",
                "al4n",
                "ki113r",
                "777notsatan777",
                "667stilnot667",
                "winnieTHEpooh"
            };

            string[] emailDomains = new string[]
            {
                "@abv.bg",
                "@gmail.com",
                "@softuni.bg"
            };

            //50 customers

            for (int i = 0; i < 50; i++)
            {
                StringBuilder creditCardNumber = new StringBuilder();

                for (int j = 0; j < 4; j++)
                {
                    creditCardNumber.Append(randGenerator.Next(1000, 9999).ToString());

                    if(j != 3)
                    {
                        creditCardNumber.Append('-');
                    }
                }

                var customer = new Customer
                {
                    Name = customerFirstNames[randGenerator.Next(0, customerFirstNames.Length)] + " " +
                        customerLastNames[randGenerator.Next(0, customerLastNames.Length)],
                    Email = emailUsers[randGenerator.Next(0, emailUsers.Length)] + 
                        emailDomains[randGenerator.Next(0, emailDomains.Length)],
                    CreditCardNumber = creditCardNumber.ToString(), 
                };

                customers.Add(customer);
            }

            return customers;
        }

        private static List<Product> SeedProducts(Random randGenerator)
        {
            //Product.Name = productName + productBrand

            string[] productBrands = new string[]
            {
                "Hama",
                "Philips",
                "Panasonic",
                "Mitsubishi",
                "Gree",
                "ZTE"
            };

            string[] productNames = new string[]
            {
                "Shaver",
                "Trimmer",
                "Hair dryer",
                "Headphones",
                "Mouse",
                "Keyboard",
                "Air Conditioner",
                "Microphone",
                "Router",
                "HDMI Cable"
            };

            var products = new List<Product>();

            //80 products

            for (int i = 0; i < 80; i++)
            {
                var product = new Product
                {
                    Name = productBrands[randGenerator.Next(0, productBrands.Length)]
                    + " " + productNames[randGenerator.Next(0, productNames.Length)],
                    Quantity = randGenerator.Next(1, 30),
                    Price = randGenerator.Next(1, 3000)
                };

                products.Add(product);
            }

            return products;
        }
    }
}
